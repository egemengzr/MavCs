using System.Reflection;

using MavCs.Core.Messages;
using MavCs.Core.Registry;

namespace MavCs.Tests;

public class RegistryTests
{
    [Theory]
    [InlineData(0, 50)]
    [InlineData(253, 83)]

public void KnownMessages_Should_Contain_Standard_CRCs(uint id, byte expectedCrc)
    {
        var registry = new KnownMessages();
        var crc = registry.GetCrcExtra(id);
        
        Assert.NotNull(crc);
        Assert.Equal(expectedCrc, crc);
    }

    [Fact]
    public void Registry_Should_Match_All_Implemented_Message_Attributes()
    {
        
        var registry = new KnownMessages();
        var assembly = typeof(HeartbeatMessage).Assembly;
        
        var messageTypes = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<MavMessageAttribute>() != null);

        foreach (var type in messageTypes)
        {
            var attr = type.GetCustomAttribute<MavMessageAttribute>();
            
            byte? registryCrc = registry.GetCrcExtra(attr.Id);
            
            Assert.True(registryCrc.HasValue, 
                $"Message '{type.Name}' (ID: {attr.Id}) is implemented in code but missing in KnownMessages registry! Did you forget to run MavCs.Tools?");
            
            Assert.Equal(attr.CrcExtra, registryCrc.Value);
        }
    }
}
