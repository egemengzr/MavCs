using MavCs.Core.Registry;

namespace MavCs.Tests.Applets;

public class RegistryTest
{
    [Fact]
    public void KnownMessages_Resolves_Heartbeat_CrcExtra()
    {
        var reg = new KnownMessages();
        byte? extra = reg.GetCrcExtra(0u);
        Assert.True(extra.HasValue);
        Assert.Equal((byte)50, extra.Value);
    }
}
