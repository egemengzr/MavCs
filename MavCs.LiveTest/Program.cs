using System.Buffers;
using MavCs.Core.Messages;
using MavCs.Core.Registry;
using MavCs.Core.Runtime;
using MavCs.Core.Transport;

namespace MavCs.LiveTest;

class Program
{
    private static readonly KnownMessages Registry = new();
    private static readonly MavLinkEncoder Encoder = new(Registry);
    private static readonly ArrayBufferWriter<byte> Buf = new();

    static async Task Main()
    {
        Console.WriteLine("🚀 MavCs Live Test Suite");
        Console.WriteLine("→ UDP link: 127.0.0.1:14550 ⇄ 127.0.0.1:14551\n");

        await using var udp = new MavLinkUdpTransport(
            localPort: 14550,       // listen to vehicle
            remoteHost: "127.0.0.1",
            remotePort: 14551       // send to vehicle
        );

        Console.WriteLine($"Listening at {udp.LocalEndpoint}");
        udp.FrameReceived += OnFrameReceived;
        await udp.StartAsync();

        // ===========================
        // ✳️ TEST PLAN
        // ===========================
        
        // await SendHeartbeatLoop(udp);
        await SendSysStatusLoop(udp);
        // await SendCustomTest(udp);
    }

    // ============ 🔹 EVENT HANDLER ================
    private static void OnFrameReceived(object? sender, ReadOnlyMemory<byte> frame)
    {
        var hex = BitConverter.ToString(frame.Span.ToArray());
        Console.WriteLine($"⬅️ Raw: {hex}");

        var decoder = new MavLinkDecoder(Registry);
        if (decoder.TryReadFrame(frame.Span, out var parsed, out _))
        {
            var factory = new MavMessageFactory();
            if (factory.TryDeserializeFrame(parsed!, out var msg))
            {
                switch (msg)
                {
                    case HeartbeatMessage hb:
                        Console.WriteLine($"⬅️ HEARTBEAT sys={parsed.SystemId} comp={parsed.ComponentId} type={hb.Type}");
                        break;

                    case SysStatusMessage ss:
                        Console.WriteLine($"⬅️ SYS_STATUS vbat={ss.VoltageBattery}mV load={ss.Load / 10.0:F1}% batt={ss.BatteryRemaining}%");
                        break;

                    default:
                        Console.WriteLine($"⬅️ Decoded: {msg.GetType().Name}");
                        break;
                }
            }
        }
        else
        {
            Console.WriteLine($"⬅️ Failed to parse ({frame.Length} bytes)");
        }
    }

    // ============ 🔹 TEST CASES ================

    private static async Task SendHeartbeatLoop(MavLinkUdpTransport udp)
    {
        Console.WriteLine("💓 Starting HEARTBEAT loop...");
        byte seq = 0;

        while (true)
        {
            var hb = new HeartbeatMessage
            {
                Type = 6,
                Autopilot = 8,
                BaseMode = 0x81,
                CustomMode = 0x11223344u,
                SystemStatus = 4,
                MavlinkVersion = 3
            };

            Buf.Clear();
            Encoder.WriteV2(hb, sequence: seq++, systemId: 255, componentId: 190, output: Buf);
            await udp.SendAsync(Buf.WrittenMemory);
            Console.WriteLine("➡️ Sent HEARTBEAT");
            await Task.Delay(1000);
        }
    }

    private static async Task SendSysStatusLoop(MavLinkUdpTransport udp)
    {
        Console.WriteLine("🔋 Starting SYS_STATUS loop...");
        byte seq = 0;

        while (true)
        {
            var sys = new SysStatusMessage
            {
                OnboardControlSensorsPresent = 0,
                OnboardControlSensorsEnabled = 0,
                OnboardControlSensorsHealth = 0,
                Load = 150,              // 15.0%
                VoltageBattery = 12000,  // 12V
                CurrentBattery = 120,    // 1.2A
                BatteryRemaining = 85
            };

            Buf.Clear();
            Encoder.WriteV2(sys, sequence: seq++, systemId: 255, componentId: 190, output: Buf);
            await udp.SendAsync(Buf.WrittenMemory);
            Console.WriteLine("➡️ Sent SYS_STATUS");
            await Task.Delay(1000);
        }
    }

    private static async Task SendCustomTest(MavLinkUdpTransport udp)
    {
        Console.WriteLine("🧩 Running custom one-shot test...");

        var msg = new HeartbeatMessage
        {
            Type = 6,
            Autopilot = 8,
            BaseMode = 0x80,
            SystemStatus = 4,
            MavlinkVersion = 3
        };

        Buf.Clear();
        Encoder.WriteV1(msg, sequence: 1, systemId: 1, componentId: 1, output: Buf);
        await udp.SendAsync(Buf.WrittenMemory);

        Console.WriteLine("✅ Sent custom HEARTBEAT once");
    }
}
