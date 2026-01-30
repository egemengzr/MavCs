using MavCs.Core.Abstractions;
using MavCs.Core.Messages;

namespace MavCs.Core.Serialization;

public sealed class AttitudeSerializer : IMessageSerializer<AttitudeMessage>
{
    public const int PayloadLength = 28;

    public int Write(AttitudeMessage message, Span<byte> dst)
    {
        if (dst.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(dst));
        var w = new LESpanWriter(dst);
        
        // Wire Order
        w.WriteUInt32(message.TimeBootMs);
        w.WriteFloat(message.Roll);
        w.WriteFloat(message.Pitch);
        w.WriteFloat(message.Yaw);
        w.WriteFloat(message.RollSpeed);
        w.WriteFloat(message.PitchSpeed);
        w.WriteFloat(message.YawSpeed);

        return w.BytesWritten;
    }

    public AttitudeMessage Read(ReadOnlySpan<byte> src)
    {
        if (src.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(src));

        var r = new LESpanReader(src);

        var msg = new AttitudeMessage
        {
            TimeBootMs = r.ReadUInt32(),
            Roll = r.ReadFloat(),
            Pitch = r.ReadFloat(),
            Yaw = r.ReadFloat(),
            RollSpeed = r.ReadFloat(),
            PitchSpeed = r.ReadFloat(),
            YawSpeed = r.ReadFloat()
        };

        return msg;
    }
}
