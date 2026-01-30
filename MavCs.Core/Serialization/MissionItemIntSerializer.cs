using MavCs.Core.Abstractions;
using MavCs.Core.Messages;

namespace MavCs.Core.Serialization;

public sealed class MissionItemIntSerializer : IMessageSerializer<MissionItemIntMessage>
{
    public const int PayloadLength = 38;

    public int Write(MissionItemIntMessage message, Span<byte> dst)
    {
        if (dst.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(dst));
        var w = new LESpanWriter(dst);

        w.WriteFloat(message.Param1);
        w.WriteFloat(message.Param2);
        w.WriteFloat(message.Param3);
        w.WriteFloat(message.Param4);
        w.WriteInt32(message.X);
        w.WriteInt32(message.Y);
        w.WriteFloat(message.Z);
        w.WriteUInt16(message.Seq);
        w.WriteUInt16(message.Command);
        w.WriteByte(message.TargetSystem);
        w.WriteByte(message.TargetComponent);
        w.WriteByte(message.Frame);
        w.WriteByte(message.Current);
        w.WriteByte(message.Autocontinue);
        w.WriteByte(message.MissionType);

        return PayloadLength;
    }

    public MissionItemIntMessage Read(ReadOnlySpan<byte> src)
    {
        if (src.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(src));
        var r = new LESpanReader(src);

        return new MissionItemIntMessage
        {
            Param1 = r.ReadFloat(),
            Param2 = r.ReadFloat(),
            Param3 = r.ReadFloat(),
            Param4 = r.ReadFloat(),
            X = r.ReadInt32(),
            Y = r.ReadInt32(),
            Z = r.ReadFloat(),
            Seq = r.ReadUInt16(),
            Command = r.ReadUInt16(),
            TargetSystem = r.ReadByte(),
            TargetComponent = r.ReadByte(),
            Frame = r.ReadByte(),
            Current = r.ReadByte(),
            Autocontinue = r.ReadByte(),
            MissionType = r.ReadByte()
        };
    }
}
