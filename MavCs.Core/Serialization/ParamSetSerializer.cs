using System.Text;

using MavCs.Core.Abstractions;
using MavCs.Core.Messages;

namespace MavCs.Core.Serialization;

public sealed class ParamSetSerializer : IMessageSerializer<ParamSetMessage>
{
    public const int PayloadLength = 23;

    public int Write(ParamSetMessage message, Span<byte> dst)
    {
        if (dst.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(dst));
        var w = new LESpanWriter(dst);
        
        w.WriteFloat(message.ParamValue);
        w.WriteByte(message.TargetSystem);
        w.WriteByte(message.TargetComponent);

        byte[] strBytes = Encoding.ASCII.GetBytes(message.ParamId ?? "");
        for (int i = 0; i < 16; i++) w.WriteByte(i < strBytes.Length ? strBytes[i] : (byte)0);

        w.WriteByte(message.ParamType);

        return PayloadLength;
    }

    public ParamSetMessage Read(ReadOnlySpan<byte> src)
    {
        if (src.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(src));
        var r = new LESpanReader(src);

        var msg = new ParamSetMessage
        {
            ParamValue = r.ReadFloat(),
            TargetSystem = r.ReadByte(),
            TargetComponent = r.ReadByte()
        };

        Span<byte> strBuffer = stackalloc byte[16];
        for (int i = 0; i < 16; i++) strBuffer[i] = r.ReadByte();
        msg.ParamId = Encoding.ASCII.GetString(strBuffer).TrimEnd('\0');

        msg.ParamType = r.ReadByte();

        return msg;
    }
}
