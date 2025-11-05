using MavCs.Core.Messages;
using MavCs.Core.Serialization;

namespace MavCs.Tests.MessageTests;

public class GlobalPositionIntSerializerTests
{
    [Fact]
    public void GlobalPositionInt_Payload_Roundtrip()
    {
        var src = new GlobalPositionIntMessage
        {
            TimeBootMs = 1,
            Lat = 2,
            Lon = 3,
            Alt = 4,
            RelativeAlt = 5,
            Vx = 6,
            Vy = 7,
            Vz = 8,
            Hdg = 9
        };

        Span<byte> buf = stackalloc byte[GlobalPositionIntSerializer.PayloadLength];
        var ser = new GlobalPositionIntSerializer();
        int n = ser.Write(src, buf);
        Assert.Equal(GlobalPositionIntSerializer.PayloadLength, n);

        var dst = ser.Read(buf);
        Assert.Equal(src.TimeBootMs, dst.TimeBootMs);
        Assert.Equal(src.Lat, dst.Lat);
        Assert.Equal(src.Lon, dst.Lon);
        Assert.Equal(src.Alt, dst.Alt);
        Assert.Equal(src.RelativeAlt, dst.RelativeAlt);
        Assert.Equal(src.Vx, dst.Vx);
        Assert.Equal(src.Vy, dst.Vy);
        Assert.Equal(src.Vz, dst.Vz);
        Assert.Equal(src.Hdg, dst.Hdg);
    }
}
