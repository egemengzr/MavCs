using MavCs.Core.Abstractions;
using MavCs.Core.Messages;

namespace MavCs.Core.Serialization;

public sealed class CommandLongSerializer : IMessageSerializer<CommandLongMessage>
{
    public const int PayloadLength = 33;

    public int Write(CommandLongMessage message, Span<byte> dst)
    {
        if (dst.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(dst));
        var w = new LESpanWriter(dst);

        w.WriteFloat(message.Param1);
        w.WriteFloat(message.Param2);
        w.WriteFloat(message.Param3);
        w.WriteFloat(message.Param4);
        w.WriteFloat(message.Param5);
        w.WriteFloat(message.Param6);
        w.WriteFloat(message.Param7);
        w.WriteUInt16(message.Command);
        w.WriteByte(message.TargetSystem);
        w.WriteByte(message.TargetComponent);
        w.WriteByte(message.Confirmation);

        return PayloadLength;
    }

    public CommandLongMessage Read(ReadOnlySpan<byte> src)
    {
        if (src.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(src));
        var r = new LESpanReader(src);

        return new CommandLongMessage
        {
            Param1 = r.ReadFloat(),
            Param2 = r.ReadFloat(),
            Param3 = r.ReadFloat(),
            Param4 = r.ReadFloat(),
            Param5 = r.ReadFloat(),
            Param6 = r.ReadFloat(),
            Param7 = r.ReadFloat(),
            Command = r.ReadUInt16(),
            TargetSystem = r.ReadByte(),
            TargetComponent = r.ReadByte(),
            Confirmation = r.ReadByte()
        };
    }
}
