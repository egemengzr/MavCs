using MavCs.Core.Messages;
using MavCs.Core.Serialization;

namespace MavCs.Tests.MessageTests;

public class SystemStatusTests
{
    [Fact]
    public void BatteryStatus_Roundtrip()
    {
        var src = new BatteryStatusMessage
        {
            EnergyConsumed = 5000,
            TimeRemaining = 1200,
            Voltages = new ushort[] { 4200, 4190, 4210, 0, 0, 0, 0, 0, 0, 0 },
            CurrentBattery = 1500,
            Id = 1,
            BatteryFunction = 0,
            Type = 0,
            BatteryRemaining = 85
        };

        Span<byte> buf = stackalloc byte[36]; // Min length for standard
        var ser = new BatteryStatusSerializer();
        
        ser.Write(src, buf);
        var dst = ser.Read(buf);

        Assert.Equal(src.EnergyConsumed, dst.EnergyConsumed);
        Assert.Equal(src.Voltages[0], dst.Voltages![0]);
        Assert.Equal(src.Voltages[2], dst.Voltages![2]);
        Assert.Equal(src.BatteryRemaining, dst.BatteryRemaining);
    }

    [Fact]
    public void AutopilotVersion_Roundtrip()
    {
        var src = new AutopilotVersionMessage
        {
            Capabilities = 0xFF,
            Uid = 9988776655,
            FlightSwVersion = 10100,
            VendorId = 0x26AC,
            ProductId = 0x0010,
            FlightCustomVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 },
            MiddlewareCustomVersion = new byte[8],
            OsCustomVersion = new byte[8]
        };

        Span<byte> buf = stackalloc byte[60];
        var ser = new AutopilotVersionSerializer();

        ser.Write(src, buf);
        var dst = ser.Read(buf);

        Assert.Equal(src.Uid, dst.Uid);
        Assert.Equal(src.FlightCustomVersion[0], dst.FlightCustomVersion![0]);
    }
}
