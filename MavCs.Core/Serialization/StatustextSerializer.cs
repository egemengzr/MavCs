using System.Text;

using MavCs.Core.Abstractions;
using MavCs.Core.Messages;

namespace MavCs.Core.Serialization;

public sealed class StatustextSerializer : IMessageSerializer<StatustextMessage>
{
    public const int PayloadLength = 54; 

    public int Write(StatustextMessage message, Span<byte> dst)
    {
        if (dst.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(dst));
        var w = new LESpanWriter(dst);

        w.WriteByte(message.Severity);
        
        byte[] strBytes = Encoding.ASCII.GetBytes(message.Text ?? "");
        for (int i = 0; i < 50; i++) 
            w.WriteByte(i < strBytes.Length ? strBytes[i] : (byte)0);

        w.WriteUInt16(message.Id);
        w.WriteByte(message.ChunkSeq);

        return PayloadLength;
    }

    public StatustextMessage Read(ReadOnlySpan<byte> src)
    {
        Span<byte> buffer = stackalloc byte[PayloadLength];
        buffer.Clear(); 

        src.Slice(0, Math.Min(src.Length, PayloadLength)).CopyTo(buffer);

        var r = new LESpanReader(buffer);

        var msg = new StatustextMessage
        {
            Severity = r.ReadByte()
        };

        Span<byte> strBuffer = stackalloc byte[50];
        for (int i = 0; i < 50; i++) strBuffer[i] = r.ReadByte();
        msg.Text = Encoding.ASCII.GetString(strBuffer).TrimEnd('\0');

        msg.Id = r.ReadUInt16();
        msg.ChunkSeq = r.ReadByte();

        return msg;
    }
}
