using Xunit;

namespace MavCs.Tests.Applets;

public class HelloTests
{
    [Fact]
    public void BasicSanityCheck()
    {
        int sum = 2 + 2;
        Assert.Equal(4, sum);
    }
}
