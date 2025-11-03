using System;
using System.Buffers;
using System.Threading.Tasks;
using MavCs.Core.Messages;
using MavCs.Core.Registry;
using MavCs.Core.Runtime;
using MavCs.Core.Transport;

namespace MavCs.LiveTest;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("MavCs Live Test – UDP Heartbeat with ArduPilot");
        Console.WriteLine("Listening on 14550, sending to 14551...");
        Console.WriteLine("Make sure sim_vehicle.py is running!\n");

        await using var udp = new MavLinkUdpTransport(
            localPort: 14550,       // listen to vehicle
            remoteHost: "127.0.0.1",
            remotePort: 14551       // send to vehicle
        );
        
        Console.WriteLine($"Listening at {udp.LocalEndpoint}");

        // Listen incoming frames
        udp.FrameReceived += (s, frame) =>
        {
            var hex = BitConverter.ToString(frame.Span[..Math.Min(frame.Length, 12)].ToArray());
            Console.WriteLine($"⬅️ Raw: {hex}");
            
            var decoder = new MavLinkDecoder(new KnownMessages());
            if (decoder.TryReadFrame(frame.Span, out var parsed, out _))
            {
                var factory = new MavMessageFactory();
                if (factory.TryDeserializeFrame(parsed!, out var msg))
                    Console.WriteLine($"⬅️ Decoded: {msg?.GetType().Name}");
            }
            else
            {
                Console.WriteLine($"⬅️ Failed to parse ({frame.Length} bytes)");
            }
        };

        await udp.StartAsync();

        // Send our own heartbeat every 1s
        var encoder = new MavLinkEncoder(new KnownMessages());
        var buf = new ArrayBufferWriter<byte>();
        var seq = 0;

        while (true)
        {
            buf.Clear();
            var hb = new HeartbeatMessage
            {
                Type = 6,
                Autopilot = 8,
                BaseMode = 0x81,
                CustomMode = 0x11223344u,
                SystemStatus = 4,
                MavlinkVersion = 3
            };
            encoder.WriteV1(hb, sequence: (byte)(seq++), systemId: 255, componentId: 190, output: buf);
            await udp.SendAsync(buf.WrittenMemory);
            Console.WriteLine("➡️ Sent Heartbeat");
            await Task.Delay(1000);
        }
    }
}
