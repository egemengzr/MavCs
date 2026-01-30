using MavCs.Core.Messages;
using MavCs.Core.Serialization;

namespace MavCs.Tests.MessageTests;

public class NavigationTests
{
    [Fact]
    public void GpsRawInt_Roundtrip()
    {
        var src = new GpsRawIntMessage
        {
            TimeUsec = 123456789,
            Lat = 410000000, // 41.0 deg
            Lon = 290000000, // 29.0 deg
            Alt = 100000,    // 100m
            Eph = 150,
            Epv = 200,
            Vel = 1500,
            Cog = 3500,
            FixType = 3,
            SatellitesVisible = 12
        };
        
        Span<byte> buf = stackalloc byte[30]; 
        var ser = new GpsRawIntSerializer(); 
        
        ser.Write(src, buf);
        var dst = ser.Read(buf);

        Assert.Equal(src.TimeUsec, dst.TimeUsec);
        Assert.Equal(src.Lat, dst.Lat);
        Assert.Equal(src.Lon, dst.Lon);
        Assert.Equal(src.FixType, dst.FixType);
        Assert.Equal(src.SatellitesVisible, dst.SatellitesVisible);
    }

    [Fact]
    public void VfrHud_Roundtrip()
    {
        var src = new VfrHudMessage
        {
            Airspeed = 25.5f,
            Groundspeed = 22.1f,
            Alt = 150.0f,
            Climb = 1.5f,
            Heading = 180,
            Throttle = 75
        };

        Span<byte> buf = stackalloc byte[20];
        var ser = new VfrHudSerializer();
        
        ser.Write(src, buf);
        var dst = ser.Read(buf);

        Assert.Equal(src.Airspeed, dst.Airspeed);
        Assert.Equal(src.Heading, dst.Heading);
        Assert.Equal(src.Throttle, dst.Throttle);
    }

    [Fact]
    public void HomePosition_Roundtrip()
    {
        var src = new HomePositionMessage
        {
            Latitude = 411234567,
            Longitude = 291234567,
            Altitude = 50000,
            X = 10.5f,
            Y = 5.5f,
            Z = -2.0f,
            Q = new float[] { 1.0f, 0.0f, 0.0f, 0.0f },
            ApproachX = 0,
            ApproachY = 0,
            ApproachZ = -1
        };

        Span<byte> buf = stackalloc byte[52];
        var ser = new HomePositionSerializer();
        
        ser.Write(src, buf);
        var dst = ser.Read(buf);

        Assert.Equal(src.Latitude, dst.Latitude);
        Assert.Equal(src.Q[0], dst.Q![0]);
        Assert.Equal(src.ApproachZ, dst.ApproachZ);
    }
}
