using MavCs.Core.Abstractions;
using MavCs.Core.Messages;

namespace MavCs.Core.Serialization;

public sealed class GlobalPositionIntSerializer : IMessageSerializer<GlobalPositionIntMessage>
{
    public const int PayloadLength = 28;

    public int Write(GlobalPositionIntMessage message, Span<byte> dst)
    {
        if (dst.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(dst));
        var w = new LESpanWriter(dst);
        
        // Wire Order
        w.WriteUInt32(message.TimeBootMs);
        w.WriteInt32(message.Lat);
        w.WriteInt32(message.Lon);
        w.WriteInt32(message.Alt);
        w.WriteInt32(message.RelativeAlt);
        w.WriteInt16(message.Vx);
        w.WriteInt16(message.Vy);
        w.WriteInt16(message.Vz);
        w.WriteUInt16(message.Hdg);

        return w.BytesWritten;
    }

    public GlobalPositionIntMessage Read(ReadOnlySpan<byte> src)
    {
        if (src.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(src));
        var r = new LESpanReader(src);

        var msg = new GlobalPositionIntMessage
        {
            TimeBootMs = r.ReadUInt32(),
            Lat = r.ReadInt32(),
            Lon = r.ReadInt32(),
            Alt = r.ReadInt32(),
            RelativeAlt = r.ReadInt32(),
            Vx = r.ReadInt16(),
            Vy = r.ReadInt16(),
            Vz = r.ReadInt16(),
            Hdg = r.ReadUInt16()
        };

        return msg;
    }
}
