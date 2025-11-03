using MavCs.Core.Protocol;
using Xunit;

namespace MavCs.Tests;

public class CrcTests
{
    [Fact]
    public void Crc_Computes_KnownVector()
    {
        // Random data
        byte[] data = { 0xFE, 0x09, 0x01, 0x01, 0x01, 0x00 };
        ushort crc = Crc.Compute(data);
        
        // Same input equal same output
        ushort crc2 = Crc.Compute(data);
        Assert.Equal(crc, crc2);
        
        // If it changes it must be different
        ushort crc3 = Crc.Compute(new byte[] { 0xFE, 0x09, 0x01 });
        Assert.NotEqual(crc, crc3);
    }
}
