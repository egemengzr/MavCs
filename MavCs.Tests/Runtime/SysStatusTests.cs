using System.Buffers;

using MavCs.Core.Messages;
using MavCs.Core.Registry;
using MavCs.Core.Runtime;
using MavCs.Core.Serialization;

namespace MavCs.Tests.Runtime;

public class SysStatusTests
{
    [Fact]
    public void SysStatus_AutoDiscovered_Roundtrip()
    {
        var msg = new SysStatusMessage()
        {
            OnboardControlSensorsPresent = 0xAA55AA55,
            OnboardControlSensorsEnabled = 0x01020304,
            OnboardControlSensorsHealth = 0xFFFFFFFF,
            Load = 1234,
            VoltageBattery = 11000,
            CurrentBattery = -250,
            BatteryRemaining = 85,
            DropRateComm = 1,
            ErrorsComm = 2,
            ErrorsCount1 = 3,
            ErrorsCount2 = 4,
            ErrorsCount3 = 5,
            ErrorsCount4 = 6
        };

        var encoder = new MavLinkEncoder(new KnownMessages());
        var buf = new ArrayBufferWriter<byte>();

        encoder.WriteV1(msg, new SysStatusSerializer(), 1u, 7, 1, 1, buf);

        var decoder = new MavLinkDecoder(new KnownMessages());
        bool ok = decoder.TryReadFrame(buf.WrittenSpan, out var parsed, out _);
        Assert.True(ok);

        var factory = new MavMessageFactory();
        bool found = factory.TryDeserializeFrame(parsed!, out var obj);
        Assert.True(found);

        var read = Assert.IsType<SysStatusMessage> (obj);
        Assert.Equal(msg.VoltageBattery, read.VoltageBattery);
        Assert.Equal(msg.CurrentBattery, read.CurrentBattery);
        Assert.Equal(msg.BatteryRemaining, read.BatteryRemaining);
    }
}
