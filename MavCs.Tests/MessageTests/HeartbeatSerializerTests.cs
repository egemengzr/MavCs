using MavCs.Core.Messages;
using MavCs.Core.Serialization;

namespace MavCs.Tests.Applets;

public class HeartbeatSerializerTests
{
    [Fact]
    public void Heartbeat_Payload_Roundtrip()
    {
        var src = new HeartbeatMessage
        {
            CustomMode = 0x11223344u,
            Type = 6,
            Autopilot = 8,
            BaseMode = 0x81,
            SystemStatus = 4,
            MavlinkVersion = 3
        };

        Span<byte> buf = stackalloc byte[HeartbeatSerializer.PayloadLength];
        var ser = new HeartbeatSerializer();
        int n = ser.Write(src, buf);
        Assert.Equal(HeartbeatSerializer.PayloadLength, n);
        
        var dst = ser.Read(buf);
        Assert.Equal(src.CustomMode,     dst.CustomMode);
        Assert.Equal(src.Type,           dst.Type);
        Assert.Equal(src.Autopilot,      dst.Autopilot);
        Assert.Equal(src.BaseMode,       dst.BaseMode);
        Assert.Equal(src.SystemStatus,   dst.SystemStatus);
        Assert.Equal(src.MavlinkVersion, dst.MavlinkVersion);
    }
}
