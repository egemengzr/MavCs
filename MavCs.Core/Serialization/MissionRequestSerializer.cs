using MavCs.Core.Abstractions;
using MavCs.Core.Messages;

namespace MavCs.Core.Serialization;

public sealed class MissionRequestSerializer : IMessageSerializer<MissionRequestMessage>
{
    public const int PayloadLength = 5;

    public int Write(MissionRequestMessage message, Span<byte> dst)
    {
        if (dst.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(dst));
        var w = new LESpanWriter(dst);

        w.WriteUInt16(message.Seq);
        w.WriteByte(message.TargetSystem);
        w.WriteByte(message.TargetComponent);
        w.WriteByte(message.MissionType);

        return PayloadLength;
    }

    public MissionRequestMessage Read(ReadOnlySpan<byte> src)
    {
        if (src.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(src));
        var r = new LESpanReader(src);

        return new MissionRequestMessage
        {
            Seq = r.ReadUInt16(),
            TargetSystem = r.ReadByte(),
            TargetComponent = r.ReadByte(),
            MissionType = r.ReadByte()
        };
    }
}
