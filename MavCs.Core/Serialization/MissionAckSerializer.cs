using MavCs.Core.Abstractions;
using MavCs.Core.Messages;

namespace MavCs.Core.Serialization;

public sealed class MissionAckSerializer : IMessageSerializer<MissionAckMessage>
{
    public const int PayloadLength = 4;

    public int Write(MissionAckMessage message, Span<byte> dst)
    {
        if (dst.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(dst));
        var w = new LESpanWriter(dst);

        w.WriteByte(message.TargetSystem);
        w.WriteByte(message.TargetComponent);
        w.WriteByte(message.Type);
        w.WriteByte(message.MissionType);

        return PayloadLength;
    }

    public MissionAckMessage Read(ReadOnlySpan<byte> src)
    {
        if (src.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(src));
        var r = new LESpanReader(src);

        return new MissionAckMessage
        {
            TargetSystem = r.ReadByte(),
            TargetComponent = r.ReadByte(),
            Type = r.ReadByte(),
            MissionType = r.ReadByte()
        };
    }
}
