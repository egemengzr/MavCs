using MavCs.Core.Abstractions;
using MavCs.Core.Messages;

namespace MavCs.Core.Serialization;

public sealed class MissionCountSerializer : IMessageSerializer<MissionCountMessage>
{
    public const int PayloadLength = 5;

    public int Write(MissionCountMessage message, Span<byte> dst)
    {
        if (dst.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(dst));
        var w = new LESpanWriter(dst);

        w.WriteUInt16(message.Count);
        w.WriteByte(message.TargetSystem);
        w.WriteByte(message.TargetComponent);
        w.WriteByte(message.MissionType);

        return PayloadLength;
    }

    public MissionCountMessage Read(ReadOnlySpan<byte> src)
    {
        if (src.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(src));
        var r = new LESpanReader(src);

        return new MissionCountMessage
        {
            Count = r.ReadUInt16(),
            TargetSystem = r.ReadByte(),
            TargetComponent = r.ReadByte(),
            MissionType = r.ReadByte()
        };
    }
}
