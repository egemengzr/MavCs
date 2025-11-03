using System.Buffers;

using MavCs.Core.Messages;
using MavCs.Core.Registry;
using MavCs.Core.Runtime;
using MavCs.Core.Serialization;

namespace MavCs.Tests.Integrations;

public class MessageFactoryTests
{
    [Fact]
    public void Factory_Deserializes_Heartbeat_From_FrameV1()
    {
        // 1. Create message and encode
        var msg = new HeartbeatMessage
        {
            Type = 6,
            Autopilot = 8,
            BaseMode = 0x81,
            CustomMode = 0x11223344u,
            SystemStatus = 4,
            MavlinkVersion = 3
        };

        var encoder = new MavLinkEncoder(new KnownMessages());
        var ser = new HeartbeatSerializer();
        var buf = new ArrayBufferWriter<byte>();

        encoder.WriteV1(msg, ser, 0u, 11, 1, 1, buf);
        
        // 2. Decode frame
        var decoder = new MavLinkDecoder(new KnownMessages());
        bool ok = decoder.TryReadFrame(buf.WrittenSpan, out var parsed, out _);
        Assert.True(ok);

        // 3. Use factory to deserialize into typed object
        var factory = new MavMessageFactory();
        bool found = factory.TryDeserializeFrame(parsed!, out var obj);
        Assert.True(found);
        var read = Assert.IsType<HeartbeatMessage>(obj);

        Assert.Equal(msg.Type, read.Type);
        Assert.Equal(msg.Autopilot, read.Autopilot);
        Assert.Equal(msg.CustomMode, read.CustomMode);
        Assert.Equal(msg.BaseMode, read.BaseMode);
        Assert.Equal(msg.SystemStatus, read.SystemStatus);
        Assert.Equal(msg.MavlinkVersion, read.MavlinkVersion);
    }
}
