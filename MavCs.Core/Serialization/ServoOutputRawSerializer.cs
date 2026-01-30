using MavCs.Core.Abstractions;
using MavCs.Core.Messages;

namespace MavCs.Core.Serialization;

public sealed class ServoOutputRawSerializer : IMessageSerializer<ServoOutputRawMessage>
{
    public const int PayloadLength = 21;

    public int Write(ServoOutputRawMessage message, Span<byte> dst)
    {
        if (dst.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(dst));
        var w = new LESpanWriter(dst);

        w.WriteUInt32(message.TimeUsec);
        w.WriteUInt16(message.Servo1Raw);
        w.WriteUInt16(message.Servo2Raw);
        w.WriteUInt16(message.Servo3Raw);
        w.WriteUInt16(message.Servo4Raw);
        w.WriteUInt16(message.Servo5Raw);
        w.WriteUInt16(message.Servo6Raw);
        w.WriteUInt16(message.Servo7Raw);
        w.WriteUInt16(message.Servo8Raw);
        w.WriteByte(message.Port);

        return PayloadLength;
    }

    public ServoOutputRawMessage Read(ReadOnlySpan<byte> src)
    {
        if (src.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(src));
        var r = new LESpanReader(src);

        return new ServoOutputRawMessage
        {
            TimeUsec = r.ReadUInt32(),
            Servo1Raw = r.ReadUInt16(),
            Servo2Raw = r.ReadUInt16(),
            Servo3Raw = r.ReadUInt16(),
            Servo4Raw = r.ReadUInt16(),
            Servo5Raw = r.ReadUInt16(),
            Servo6Raw = r.ReadUInt16(),
            Servo7Raw = r.ReadUInt16(),
            Servo8Raw = r.ReadUInt16(),
            Port = r.ReadByte()
        };
    }
}
