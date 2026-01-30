using MavCs.Core.Abstractions;
using MavCs.Core.Messages;
using MavCs.Core.Serialization;

namespace MavCs.Core.Serialization;
public sealed class GpsRawIntSerializer : IMessageSerializer<GpsRawIntMessage>
{
    public const int PayloadLength = 30;

    public int Write(GpsRawIntMessage message, Span<byte> dst)
    {
        if (dst.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(dst));
        var w = new LESpanWriter(dst);

        w.WriteUInt64(message.TimeUsec);
        w.WriteInt32(message.Lat);
        w.WriteInt32(message.Lon);
        w.WriteInt32(message.Alt);
        w.WriteUInt16(message.Eph);
        w.WriteUInt16(message.Epv);
        w.WriteUInt16(message.Vel);
        w.WriteUInt16(message.Cog);
        w.WriteByte(message.FixType);
        w.WriteByte(message.SatellitesVisible);

        return PayloadLength;
    }

    public GpsRawIntMessage Read(ReadOnlySpan<byte> src)
    {
        if (src.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(src));
        var r = new LESpanReader(src);

        return new GpsRawIntMessage
        {
            TimeUsec = r.ReadUInt64(),
            Lat = r.ReadInt32(),
            Lon = r.ReadInt32(),
            Alt = r.ReadInt32(),
            Eph = r.ReadUInt16(),
            Epv = r.ReadUInt16(),
            Vel = r.ReadUInt16(),
            Cog = r.ReadUInt16(),
            FixType = r.ReadByte(),
            SatellitesVisible = r.ReadByte()
        };
    }
}
