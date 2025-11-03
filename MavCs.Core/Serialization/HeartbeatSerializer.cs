using MavCs.Core.Abstractions;
using MavCs.Core.Messages;

namespace MavCs.Core.Serialization;

// Binary payload (de)serializer for HEARTBEAT (id=0).
// Wire order (MAVLink): custom_mode (u32), type (u8), autopilot (u8), base_mode (u8), system_status (u8), mavlink_version (u8).
// Total length: 4 + 1*5 = 9 bytes.

public sealed class HeartbeatSerializer : IMessageSerializer<HeartbeatMessage>
{
    public const int PayloadLength = 9;

    public int Write(HeartbeatMessage message, Span<byte> dst)
    {
        if (dst.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(dst));
        var w = new LESpanWriter(dst);
        
        // Wire order
        w.WriteUIint32(message.CustomMode);
        w.WriteByte(message.Type);
        w.WriteByte(message.Autopilot);
        w.WriteByte(message.BaseMode);
        w.WriteByte(message.SystemStatus);
        w.WriteByte(message.MavlinkVersion);

        return w.BytesWritten;  // should be 9
    }

    public HeartbeatMessage Read(ReadOnlySpan<byte> src)
    {
        if (src.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(src));
        var r = new LESpanReader(src);

        var msg = new HeartbeatMessage
        {
            CustomMode = r.ReadUInt32(),
            Type = r.ReadByte(),
            Autopilot = r.ReadByte(),
            BaseMode = r.ReadByte(),
            SystemStatus = r.ReadByte(),
            MavlinkVersion = r.ReadByte(),
        };

        return msg;
    }
}
