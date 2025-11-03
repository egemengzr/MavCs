using Xunit;

namespace MavCs.Tests;

public class HelloTests
{
    [Fact]
    public void BasicSanityCheck()
    {
        int sum = 2 + 2;
        Assert.Equal(4, sum);
    }
}