using System.Text;

using MavCs.Core.Abstractions;
using MavCs.Core.Messages;

namespace MavCs.Core.Serialization;

public sealed class ParamRequestReadSerializer : IMessageSerializer<ParamRequestReadMessage>
{
    public const int PayloadLength = 20;

    public int Write(ParamRequestReadMessage message, Span<byte> dst)
    {
        if (dst.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(dst));
        var w = new LESpanWriter(dst);

        w.WriteInt16(message.ParamIndex);
        w.WriteByte(message.TargetSystem);
        w.WriteByte(message.TargetComponent);
        
        byte[] strBytes = Encoding.ASCII.GetBytes(message.ParamId ?? "");
        for (int i = 0; i < 16; i++) w.WriteByte(i < strBytes.Length ? strBytes[i] : (byte)0);

        return PayloadLength;
    }

    public ParamRequestReadMessage Read(ReadOnlySpan<byte> src)
    {
        if (src.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(src));
        var r = new LESpanReader(src);

        var msg = new ParamRequestReadMessage
        {
            ParamIndex = r.ReadInt16(),
            TargetSystem = r.ReadByte(),
            TargetComponent = r.ReadByte()
        };

        Span<byte> strBuffer = stackalloc byte[16];
        for (int i = 0; i < 16; i++) strBuffer[i] = r.ReadByte();
        msg.ParamId = Encoding.ASCII.GetString(strBuffer).TrimEnd('\0');

        return msg;
    }
}
