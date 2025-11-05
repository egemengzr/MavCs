using System.Text;

using MavCs.Core.Abstractions;
using MavCs.Core.Messages;

namespace MavCs.Core.Serialization;

public class StatustextSerializer : IMessageSerializer<StatustextMessage>
{
    public const int PayloadLength = 54;

    public int Write(StatustextMessage message, Span<byte> dst)
    {
        if (dst.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(dst));

        var w = new LESpanWriter(dst);
        
        // 0: severity
        w.WriteByte(message.Severity);

        // 1–50: text[50] (char array, no null termination)
        var bytes = Encoding.ASCII.GetBytes(message.Text ?? string.Empty);
        var padded = new byte[50];
        var len = Math.Min(bytes.Length, 50);
        Array.Copy(bytes, padded, len);
        w.WriteBytes(padded);

        // 51–52: id (uint16)
        w.WriteUInt16(message.Id);

        // 53: chunk_seq (uint8)
        w.WriteByte(message.ChunkSeq);

        return PayloadLength;
    }

    public StatustextMessage Read(ReadOnlySpan<byte> src)
    {
        if (src.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(src));
        
        var r = new LESpanReader(src);
        var msg = new StatustextMessage();

        // 0: severity
        msg.Severity = r.ReadByte();

        // 1–50: text[50]
        var textBytes = r.ReadBytes(50);
        int len = textBytes.IndexOf((byte)0);
        if (len < 0) len = textBytes.Length;
        msg.Text = System.Text.Encoding.ASCII.GetString(textBytes.Slice(0, len));

        // 51–52: id
        msg.Id = r.ReadUInt16();

        // 53: chunk_seq
        msg.ChunkSeq = r.ReadByte();

        return msg;
    }
}
