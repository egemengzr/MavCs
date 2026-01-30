using System.Text;

using MavCs.Core.Abstractions;
using MavCs.Core.Messages;

namespace MavCs.Core.Serialization;

public sealed class ParamValueSerializer : IMessageSerializer<ParamValueMessage>
{
    public const int PayloadLength = 25;

    public int Write(ParamValueMessage message, Span<byte> dst)
    {
        if (dst.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(dst));
        var w = new LESpanWriter(dst);

        w.WriteFloat(message.ParamValue);
        w.WriteUInt16(message.ParamCount);
        w.WriteUInt16(message.ParamIndex);
        
        byte[] strBytes = Encoding.ASCII.GetBytes(message.ParamId ?? "");
        for(int i=0; i<16; i++) w.WriteByte(i < strBytes.Length ? strBytes[i] : (byte)0);

        w.WriteByte(message.ParamType);

        return PayloadLength;
    }

    public ParamValueMessage Read(ReadOnlySpan<byte> src)
    {
        if (src.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(src));
        var r = new LESpanReader(src);

        var msg = new ParamValueMessage
        {
            ParamValue = r.ReadFloat(),
            ParamCount = r.ReadUInt16(),
            ParamIndex = r.ReadUInt16()
        };

        Span<byte> strBuffer = stackalloc byte[16];
        for (int i = 0; i < 16; i++) strBuffer[i] = r.ReadByte();
        msg.ParamId = Encoding.ASCII.GetString(strBuffer).TrimEnd('\0');

        msg.ParamType = r.ReadByte();
        return msg;
    }
}
