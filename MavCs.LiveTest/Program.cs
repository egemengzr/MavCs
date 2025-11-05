using System.Buffers;
using System.Diagnostics;

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

        using var cts = new CancellationTokenSource();
        var ct = cts.Token;

        await using var udp = new MavLinkUdpTransport(
            host: "127.0.0.1",
            remotePort: 14551,   // send to vehicle
            localPort: 14550     // listen from vehicle
        );

        await udp.StartAsync(ct);
        Console.WriteLine("Listening...");

        _ = Task.Run(async () =>
        {
            await foreach (var data in udp.ReceiveAsync(ct))
            {
                OnFrame(data);
            }
        }, ct);

        // ===========================
        // ✳️ TEST PLAN
        // ===========================
        // await SendHeartbeatLoop(udp, ct);
        // await SendSysStatusLoop(udp, ct);
        await SendStatustextLoop(udp, ct);
        // await SendCustomTest(udp, ct);
    }

    // ============ 🔹 FRAME HANDLER ================
    private static void OnFrame(ReadOnlyMemory<byte> frame)
    {
        var hex = BitConverter.ToString(frame.Span.ToArray());
        Console.WriteLine($"⬅️ Raw: {hex}");

        var decoder = new MavLinkDecoder(Registry);
        if (decoder.TryReadFrame(frame.Span, out var parsed, out _))
        {
            var factory = new MavMessageFactory();
            if (parsed is not null && factory.TryDeserializeFrame(parsed, out var msg) && msg is not null)
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
            else
            {
                Console.WriteLine("⬅️ Decoding failed (factory).");
            }
        }
        else
        {
            Console.WriteLine($"⬅️ Failed to parse ({frame.Length} bytes)");
        }
    }

    // ============ 🔹 TEST CASES ================

    private static async Task SendHeartbeatLoop(MavLinkUdpTransport udp, CancellationToken ct)
    {
        Console.WriteLine("💓 Starting HEARTBEAT loop...");
        byte seq = 0;

        while (!ct.IsCancellationRequested)
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
            await udp.SendAsync(Buf.WrittenMemory, ct);
            Console.WriteLine("➡️ Sent HEARTBEAT");
            await Task.Delay(1000, ct);
        }
    }

    private static async Task SendSysStatusLoop(MavLinkUdpTransport udp, CancellationToken ct)
    {
        Console.WriteLine("🔋 Starting SYS_STATUS loop...");
        byte seq = 0;

        while (!ct.IsCancellationRequested)
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
            await udp.SendAsync(Buf.WrittenMemory, ct);
            Console.WriteLine("➡️ Sent SYS_STATUS");
            await Task.Delay(1000, ct);
        }
    }

    private static async Task SendStatustextLoop(MavLinkUdpTransport udp, CancellationToken ct)
    {
        Console.WriteLine(" Starting STATUSTEXT loop");
        byte seq = 0;

        while (!ct.IsCancellationRequested)
        {
            var sys = new StatustextMessage
            {
                Severity = 5,
                Text = "testatestatestatestatestatestatestatestatestatesta",
                Id = 25,
                ChunkSeq = seq
            };
            
            Buf.Clear();
            Encoder.WriteV2(sys, sequence: seq++, systemId: 255, componentId: 190, output: Buf);
            await udp.SendAsync(Buf.WrittenMemory, ct);
            Console.WriteLine(" Sent STATUSTEXT");
            await Task.Delay(1000, ct);
        }
    }

    private static async Task SendCustomTest(MavLinkUdpTransport udp, CancellationToken ct)
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
        await udp.SendAsync(Buf.WrittenMemory, ct);

        Console.WriteLine("✅ Sent custom HEARTBEAT once");
    }
}
