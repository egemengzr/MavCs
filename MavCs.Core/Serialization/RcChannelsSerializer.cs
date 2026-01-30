using MavCs.Core.Abstractions;
using MavCs.Core.Messages;

namespace MavCs.Core.Serialization;

public sealed class RcChannelsSerializer : IMessageSerializer<RcChannelsMessage>
{
    public const int PayloadLength = 42;

    public int Write(RcChannelsMessage message, Span<byte> dst)
    {
        if (dst.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(dst));
        var w = new LESpanWriter(dst);

        w.WriteUInt32(message.TimeBootMs);
        w.WriteUInt16(message.Chan1Raw);
        w.WriteUInt16(message.Chan2Raw);
        w.WriteUInt16(message.Chan3Raw);
        w.WriteUInt16(message.Chan4Raw);
        w.WriteUInt16(message.Chan5Raw);
        w.WriteUInt16(message.Chan6Raw);
        w.WriteUInt16(message.Chan7Raw);
        w.WriteUInt16(message.Chan8Raw);
        w.WriteUInt16(message.Chan9Raw);
        w.WriteUInt16(message.Chan10Raw);
        w.WriteUInt16(message.Chan11Raw);
        w.WriteUInt16(message.Chan12Raw);
        w.WriteUInt16(message.Chan13Raw);
        w.WriteUInt16(message.Chan14Raw);
        w.WriteUInt16(message.Chan15Raw);
        w.WriteUInt16(message.Chan16Raw);
        w.WriteUInt16(message.Chan17Raw);
        w.WriteUInt16(message.Chan18Raw);
        w.WriteByte(message.Port);
        w.WriteByte(message.Rssi);

        return PayloadLength;
    }

    public RcChannelsMessage Read(ReadOnlySpan<byte> src)
    {
        if (src.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(src));
        var r = new LESpanReader(src);

        return new RcChannelsMessage
        {
            TimeBootMs = r.ReadUInt32(),
            Chan1Raw = r.ReadUInt16(),
            Chan2Raw = r.ReadUInt16(),
            Chan3Raw = r.ReadUInt16(),
            Chan4Raw = r.ReadUInt16(),
            Chan5Raw = r.ReadUInt16(),
            Chan6Raw = r.ReadUInt16(),
            Chan7Raw = r.ReadUInt16(),
            Chan8Raw = r.ReadUInt16(),
            Chan9Raw = r.ReadUInt16(),
            Chan10Raw = r.ReadUInt16(),
            Chan11Raw = r.ReadUInt16(),
            Chan12Raw = r.ReadUInt16(),
            Chan13Raw = r.ReadUInt16(),
            Chan14Raw = r.ReadUInt16(),
            Chan15Raw = r.ReadUInt16(),
            Chan16Raw = r.ReadUInt16(),
            Chan17Raw = r.ReadUInt16(),
            Chan18Raw = r.ReadUInt16(),
            Port = r.ReadByte(),
            Rssi = r.ReadByte()
        };
    }
}
