using MavCs.Core.Protocol;
using Xunit;

namespace MavCs.Tests.Applets;

public class CrcTests
{
    [Fact]
    public void Crc_Computes_KnownVector()
    {
        // Known MAVLink CRC for "123456789" = 0x906E (standard X25 check value)
        var data = System.Text.Encoding.ASCII.GetBytes("123456789");

        ushort crc = Crc.Reset();
        crc = Crc.AccumulateSpan(crc, data);
        crc = Crc.Finalize(crc);

        Assert.Equal(0x906E, crc);  // ✅ standard check value
    }
}
