using System.Buffers;

using MavCs.Core.Messages;
using MavCs.Core.Registry;
using MavCs.Core.Runtime;
using MavCs.Core.Serialization;

namespace MavCs.Tests.Runtime;

public class MavMessageFactoryReflectionTests
{
    [Fact]
    public void Factory_AutoDiscovers_Heartbeat()
    {
        var encoder = new MavLinkEncoder(new KnownMessages());
        var ser = new HeartbeatSerializer();
        var buf = new ArrayBufferWriter<byte>();

        var msg = new HeartbeatMessage
        {
            Type = 6,
            Autopilot = 8,
            BaseMode = 0x81,
            CustomMode = 0x11223344u,
            SystemStatus = 4,
            MavlinkVersion = 3
        };

        encoder.WriteV1(msg, ser, 0u, 12, 1, 1, buf);

        var decoder = new MavLinkDecoder(new KnownMessages());
        bool ok = decoder.TryReadFrame(buf.WrittenSpan, out var parsed, out _);
        Assert.True(ok);

        var factory = new MavMessageFactory();
        bool found = factory.TryDeserializeFrame(parsed!, out var obj);
        Assert.True(found);

        var result = Assert.IsType<HeartbeatMessage>(obj);
        Assert.Equal(msg.CustomMode, result.CustomMode);
        Assert.Equal(msg.Type, result.Type);
        Assert.Equal(msg.Autopilot, result.Autopilot);
        Assert.Equal(msg.BaseMode, result.BaseMode);
        Assert.Equal(msg.SystemStatus, result.SystemStatus);
    }
}
