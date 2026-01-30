using MavCs.Core.Abstractions;
using MavCs.Core.Messages;

namespace MavCs.Core.Serialization;

public sealed class AutopilotVersionSerializer : IMessageSerializer<AutopilotVersionMessage>
{
    public const int PayloadLength = 60;

    public int Write(AutopilotVersionMessage message, Span<byte> dst)
    {
        if (dst.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(dst));
        var w = new LESpanWriter(dst);

        w.WriteUInt64(message.Capabilities);
        w.WriteUInt64(message.Uid);
        w.WriteUInt32(message.FlightSwVersion);
        w.WriteUInt32(message.MiddlewareSwVersion);
        w.WriteUInt32(message.OsSwVersion);
        w.WriteUInt32(message.BoardVersion);
        w.WriteUInt16(message.VendorId);
        w.WriteUInt16(message.ProductId);

        for(int i=0; i<8; i++) 
            w.WriteByte(message.FlightCustomVersion != null && i < message.FlightCustomVersion.Length 
                ? message.FlightCustomVersion[i] : (byte)0);

        for(int i=0; i<8; i++) 
            w.WriteByte(message.MiddlewareCustomVersion != null && i < message.MiddlewareCustomVersion.Length 
                ? message.MiddlewareCustomVersion[i] : (byte)0);

        for(int i=0; i<8; i++) 
            w.WriteByte(message.OsCustomVersion != null && i < message.OsCustomVersion.Length 
                ? message.OsCustomVersion[i] : (byte)0);

        return PayloadLength;
    }

    public AutopilotVersionMessage Read(ReadOnlySpan<byte> src)
    {
        if (src.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(src));
        var r = new LESpanReader(src);

        var msg = new AutopilotVersionMessage
        {
            Capabilities = r.ReadUInt64(),
            Uid = r.ReadUInt64(),
            FlightSwVersion = r.ReadUInt32(),
            MiddlewareSwVersion = r.ReadUInt32(),
            OsSwVersion = r.ReadUInt32(),
            BoardVersion = r.ReadUInt32(),
            VendorId = r.ReadUInt16(),
            ProductId = r.ReadUInt16(),
            FlightCustomVersion = new byte[8],
            MiddlewareCustomVersion = new byte[8],
            OsCustomVersion = new byte[8]
        };

        for (int i = 0; i < 8; i++) msg.FlightCustomVersion[i] = r.ReadByte();
        for (int i = 0; i < 8; i++) msg.MiddlewareCustomVersion[i] = r.ReadByte();
        for (int i = 0; i < 8; i++) msg.OsCustomVersion[i] = r.ReadByte();

        return msg;
    }
}
