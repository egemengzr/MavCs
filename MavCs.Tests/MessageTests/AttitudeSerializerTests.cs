using MavCs.Core.Messages;
using MavCs.Core.Serialization;

namespace MavCs.Tests.MessageTests;

public class AttitudeSerializerTests
{
    [Fact]
    public void Attitude_Payload_Roundtrip()
    {
        var src = new AttitudeMessage
        {
            TimeBootMs = 0,
            Roll = (float)0.1,
            Pitch = (float)0.2,
            Yaw = (float)0.3,
            RollSpeed = (float)0.4,
            PitchSpeed = (float)0.5,
            YawSpeed = (float)0.6
        };

        Span<byte> buf = stackalloc byte[AttitudeSerializer.PayloadLength];
        var ser = new AttitudeSerializer();
        int n = ser.Write(src, buf);
        Assert.Equal(AttitudeSerializer.PayloadLength, n);

        var dst = ser.Read(buf);
        Assert.Equal(src.TimeBootMs, dst.TimeBootMs);
        Assert.Equal(src.Roll, dst.Roll);
        Assert.Equal(src.Pitch, dst.Pitch);
        Assert.Equal(src.Yaw, dst.Yaw);
        Assert.Equal(src.RollSpeed, dst.RollSpeed);
        Assert.Equal(src.PitchSpeed, dst.PitchSpeed);
        Assert.Equal(src.YawSpeed, dst.YawSpeed);
    }
}
