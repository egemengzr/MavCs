using MavCs.Core.Abstractions;
using MavCs.Core.Messages;

namespace MavCs.Core.Serialization;

public sealed class HomePositionSerializer : IMessageSerializer<HomePositionMessage>
{
    public const int PayloadLength = 52;

    public int Write(HomePositionMessage message, Span<byte> dst)
    {
        if (dst.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(dst));
        var w = new LESpanWriter(dst);

        w.WriteInt32(message.Latitude);
        w.WriteInt32(message.Longitude);
        w.WriteInt32(message.Altitude);
        w.WriteFloat(message.X);
        w.WriteFloat(message.Y);
        w.WriteFloat(message.Z);
        
        if (message.Q != null && message.Q.Length >= 4)
        {
            for (int i = 0; i < 4; i++) w.WriteFloat(message.Q[i]);
        }
        else
        {
            for (int i = 0; i < 4; i++) w.WriteFloat(0);
        }

        w.WriteFloat(message.ApproachX);
        w.WriteFloat(message.ApproachY);
        w.WriteFloat(message.ApproachZ);

        return PayloadLength;
    }

    public HomePositionMessage Read(ReadOnlySpan<byte> src)
    {
        if (src.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(src));
        var r = new LESpanReader(src);

        var msg = new HomePositionMessage
        {
            Latitude = r.ReadInt32(),
            Longitude = r.ReadInt32(),
            Altitude = r.ReadInt32(),
            X = r.ReadFloat(),
            Y = r.ReadFloat(),
            Z = r.ReadFloat(),
            Q = new float[4]
        };

        for (int i = 0; i < 4; i++) msg.Q[i] = r.ReadFloat();

        msg.ApproachX = r.ReadFloat();
        msg.ApproachY = r.ReadFloat();
        msg.ApproachZ = r.ReadFloat();

        return msg;
    }
}
