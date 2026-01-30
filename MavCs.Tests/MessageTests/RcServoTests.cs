using MavCs.Core.Messages;
using MavCs.Core.Serialization;

namespace MavCs.Tests.MessageTests;

public class RcServoTests
{
    [Fact]
    public void RcChannels_Roundtrip()
    {
        var src = new RcChannelsMessage
        {
            TimeBootMs = 55000,
            Chan1Raw = 1500,
            Chan2Raw = 1500,
            Chan3Raw = 1000, 
            Chan4Raw = 1500,
            Chan18Raw = 2000,
            Port = 1,
            Rssi = 254
        };

        Span<byte> buf = stackalloc byte[42];
        var ser = new RcChannelsSerializer();
        
        ser.Write(src, buf);
        var dst = ser.Read(buf);

        Assert.Equal(src.TimeBootMs, dst.TimeBootMs);
        Assert.Equal(src.Chan3Raw, dst.Chan3Raw);
        Assert.Equal(src.Chan18Raw, dst.Chan18Raw);
        Assert.Equal(src.Rssi, dst.Rssi);
    }

    [Fact]
    public void ServoOutputRaw_Roundtrip()
    {
        var src = new ServoOutputRawMessage
        {
            TimeUsec = 1000000,
            Servo1Raw = 1400,
            Servo8Raw = 1600,
            Port = 0
        };

        Span<byte> buf = stackalloc byte[21];
        var ser = new ServoOutputRawSerializer();

        ser.Write(src, buf);
        var dst = ser.Read(buf);

        Assert.Equal(src.Servo1Raw, dst.Servo1Raw);
        Assert.Equal(src.Servo8Raw, dst.Servo8Raw);
    }
}
