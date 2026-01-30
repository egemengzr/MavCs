using MavCs.Core.Messages;
using MavCs.Core.Serialization;

namespace MavCs.Tests.MessageTests;

/*
 *
 * public uint OnboardControlSensorsPresent { get; set; }
   public uint OnboardControlSensorsEnabled { get; set; }
   public uint OnboardControlSensorsHealth  { get; set; }
   public ushort Load                       { get; set; }
   public ushort VoltageBattery             { get; set; }
   public short  CurrentBattery             { get; set; }
   public sbyte  BatteryRemaining           { get; set; }
   public ushort DropRateComm               { get; set; }
   public ushort ErrorsComm                 { get; set; }
   public ushort ErrorsCount1               { get; set; }
   public ushort ErrorsCount2               { get; set; }
   public ushort ErrorsCount3               { get; set; }
   public ushort ErrorsCount4               { get; set; }
 */
public class SysStatusSerializerTests
{
    [Fact]
    public void SysStatus_Payload_Roundtrip()
    {
        var src = new SysStatusMessage
        {
            OnboardControlSensorsPresent = 0,
            OnboardControlSensorsEnabled = 1,
            OnboardControlSensorsHealth = 2,
            Load = 3,
            VoltageBattery = 4,
            CurrentBattery = 5,
            BatteryRemaining = 6,
            DropRateComm = 7,
            ErrorsComm = 8,
            ErrorsCount1 = 9,
            ErrorsCount2 = 10,
            ErrorsCount3 = 11,
            ErrorsCount4 = 12
        };

        Span<byte> buf = stackalloc byte[SysStatusSerializer.PayloadLength];
        var ser = new SysStatusSerializer();
        int n = ser.Write(src, buf);
        Assert.Equal(SysStatusSerializer.PayloadLength, n);

        var dst = ser.Read(buf);
        Assert.Equal(src.OnboardControlSensorsPresent, dst.OnboardControlSensorsPresent);
        Assert.Equal(src.OnboardControlSensorsEnabled, dst.OnboardControlSensorsEnabled);
        Assert.Equal(src.OnboardControlSensorsHealth, dst.OnboardControlSensorsHealth);
        Assert.Equal(src.Load, dst.Load);
        Assert.Equal(src.VoltageBattery, dst.VoltageBattery);
        Assert.Equal(src.CurrentBattery, dst.CurrentBattery);
        Assert.Equal(src.BatteryRemaining, dst.BatteryRemaining);
        Assert.Equal(src.DropRateComm, dst.DropRateComm);
        Assert.Equal(src.ErrorsComm, dst.ErrorsComm);
        Assert.Equal(src.ErrorsCount1, dst.ErrorsCount1);
        Assert.Equal(src.ErrorsCount2, dst.ErrorsCount2);
        Assert.Equal(src.ErrorsCount3, dst.ErrorsCount3);
        Assert.Equal(src.ErrorsCount4, dst.ErrorsCount4);
    }
}
