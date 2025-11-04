using System;
using System.Buffers;
using System.Threading.Tasks;
using MavCs.Core.Messages;
using MavCs.Core.Protocol;
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
            var hex = BitConverter.ToString(frame.Span.ToArray());
            Console.WriteLine($"⬅️ Raw: {hex}");

            var registry = new KnownMessages();
            var decoder  = new MavLinkDecoder(registry);

            if (decoder.TryReadFrame(frame.Span, out var parsed, out _))
            {
                var factory = new MavMessageFactory();
                if (factory.TryDeserializeFrame(parsed!, out var msg))
                    Console.WriteLine($"⬅️ 👑👑👑👑👑 Decoded: {msg?.GetType().Name}");
            }
            else
            {
                try
                {
                    var span = frame.Span;
                    if (span.Length < 4) { Console.WriteLine($"⬅️ Failed to parse ({span.Length} bytes)"); return; }

                    // Frame sonundaki CRC (LE)
                    ushort crcFrame = (ushort)(span[span.Length - 2] | (span[span.Length - 1] << 8));
                    byte magic = span[0];
                    ushort calc = Crc.Reset();
                    byte len = span[1];

                    bool v1 = magic == 0xFE;
                    bool v2 = magic == 0xFD;

                    byte crcExtraUsed = 0;
                    uint msgId = 0;

                    if (v1)
                    {
                        // v1 header: [len, seq, sysid, compid, msgid]  => 5 byte
                        const int hdrSizeV1 = 5;
                        // CRC kapsamı: len..msgid (5 byte)
                        calc = Crc.AccumulateSpan(calc, span.Slice(1, hdrSizeV1));
                        // payload
                        calc = Crc.AccumulateSpan(calc, span.Slice(1 + hdrSizeV1, len));
                        // crc_extra
                        msgId = span[5];
                        crcExtraUsed = registry.GetCrcExtra(msgId) ?? (byte)0;
                        calc = Crc.AccumulateByte(calc, crcExtraUsed);

                        calc = Crc.Finalize(calc);
                    }
                    else if (v2)
                    {
                        // v2 header alanları (len hariç): incompat, compat, seq, sys, comp, msgid(3) => 8 byte
                        const int headerV2NoLen = 8;
                        // CRC kapsamı: incompat..msgid(3)
                        calc = Crc.AccumulateSpan(calc, span.Slice(2, headerV2NoLen));
                        // payload
                        int payloadStart = 1 + 1 + headerV2NoLen; // magic(0) + len(1) + header(8)
                        calc = Crc.AccumulateSpan(calc, span.Slice(payloadStart, len));
                        // crc_extra
                        msgId = (uint)(span[7] | (span[8] << 8) | (span[9] << 16));
                        crcExtraUsed = registry.GetCrcExtra(msgId) ?? (byte)0;
                        calc = Crc.AccumulateByte(calc, crcExtraUsed);

                        calc = Crc.Finalize(calc);
                    }
                    else
                    {
                        Console.WriteLine($"⬅️ Failed to parse ({span.Length} bytes) | Unknown magic=0x{magic:X2}");
                        return;
                    }

                    Console.WriteLine(
                        $"⬅️ Failed to parse ({span.Length} bytes) | ver={(v1 ? "v1" : "v2")} msgId={msgId} " +
                        $"crc_extra={(crcExtraUsed == 0 ? "0 (unknown?)" : $"0x{crcExtraUsed:X2}")} " +
                        $"CRC(frame)=0x{crcFrame:X4} CRC(calc)=0x{calc:X4}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⬅️ Failed to parse ({frame.Length} bytes) | CRC debug error: {ex.Message}");
                }
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
