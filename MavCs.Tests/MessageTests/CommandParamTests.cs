using MavCs.Core.Messages;
using MavCs.Core.Serialization;

namespace MavCs.Tests.MessageTests;

public class CommandParamTests
{
    [Fact]
    public void CommandLong_Roundtrip()
    {
        var src = new CommandLongMessage
        {
            Command = 400, // MAV_CMD_COMPONENT_ARM_DISARM
            Param1 = 1.0f, // ARM
            TargetSystem = 1,
            TargetComponent = 1,
            Confirmation = 0
        };

        Span<byte> buf = stackalloc byte[33];
        var ser = new CommandLongSerializer();

        ser.Write(src, buf);
        var dst = ser.Read(buf);

        Assert.Equal(src.Command, dst.Command);
        Assert.Equal(src.Param1, dst.Param1);
    }

    [Fact]
    public void ParamRequestRead_Roundtrip()
    {
        var src = new ParamRequestReadMessage
        {
            ParamIndex = -1,
            TargetSystem = 1,
            TargetComponent = 1,
            ParamId = "WP_RADIUS" 
        };

        Span<byte> buf = stackalloc byte[20];
        var ser = new ParamRequestReadSerializer();

        ser.Write(src, buf);
        var dst = ser.Read(buf);

        Assert.Equal(src.ParamId, dst.ParamId);
        Assert.Equal(src.ParamIndex, dst.ParamIndex);
    }

    [Fact]
    public void ParamValue_Roundtrip()
    {
        var src = new ParamValueMessage
        {
            ParamId = "FLTMODE1",
            ParamValue = 5.0f,
            ParamType = 6,
            ParamCount = 100,
            ParamIndex = 10
        };

        Span<byte> buf = stackalloc byte[25];
        var ser = new ParamValueSerializer();

        ser.Write(src, buf);
        var dst = ser.Read(buf);

        Assert.Equal(src.ParamValue, dst.ParamValue);
        Assert.Equal(src.ParamId, dst.ParamId);
    }
}
