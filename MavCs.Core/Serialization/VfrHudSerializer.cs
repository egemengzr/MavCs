using MavCs.Core.Abstractions;
using MavCs.Core.Messages;

namespace MavCs.Core.Serialization;

public sealed class VfrHudSerializer : IMessageSerializer<VfrHudMessage>
{
    public const int PayloadLength = 20;

    public int Write(VfrHudMessage message, Span<byte> dst)
    {
        if (dst.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(dst));
        var w = new LESpanWriter(dst);

        w.WriteFloat(message.Airspeed);
        w.WriteFloat(message.Groundspeed);
        w.WriteFloat(message.Alt);
        w.WriteFloat(message.Climb);
        w.WriteInt16(message.Heading);
        w.WriteUInt16(message.Throttle);

        return PayloadLength;
    }

    public VfrHudMessage Read(ReadOnlySpan<byte> src)
    {
        if (src.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(src));
        var r = new LESpanReader(src);

        return new VfrHudMessage
        {
            Airspeed = r.ReadFloat(),
            Groundspeed = r.ReadFloat(),
            Alt = r.ReadFloat(),
            Climb = r.ReadFloat(),
            Heading = r.ReadInt16(),
            Throttle = r.ReadUInt16()
        };
    }
}
