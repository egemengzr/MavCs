using System.Buffers;

using MavCs.Core.Messages;
using MavCs.Core.Protocol;
using MavCs.Core.Registry;
using MavCs.Core.Runtime;
using MavCs.Core.Serialization;

namespace MavCs.Tests.Integrations;

public class MavLinkEncoderTests
{
    [Fact]
    public void Encoder_V1_Heartbeat_Roundtrip()
    {
        var msg = new HeartbeatMessage
        {
            Type = 6,
            Autopilot = 8,
            BaseMode = 0x81,
            CustomMode = 0x11223344u,
            SystemStatus = 4,
            MavlinkVersion = 3
        };

        var registry = new KnownMessages();
        var encoder = new MavLinkEncoder(registry);
        var ser = new HeartbeatSerializer();
        var buf = new ArrayBufferWriter<byte>();

        int written = encoder.WriteV1(msg, ser, 0, 7, 1, 1, buf);

        var decoder = new MavLinkDecoder(registry);
        bool ok = decoder.TryReadFrame(buf.WrittenSpan, out var parsed, out int consumed);
        Assert.True(ok);
        Assert.Equal(written, consumed);

        var frame = Assert.IsType<FrameV1>(parsed);
        var read = ser.Read(frame.Payload);
        
        Assert.Equal(msg.CustomMode, read.CustomMode);
        Assert.Equal(msg.Type, read.Type);
        Assert.Equal(msg.Autopilot, read.Autopilot);
        Assert.Equal(msg.BaseMode, read.BaseMode);
        Assert.Equal(msg.SystemStatus, read.SystemStatus);
        Assert.Equal(msg.MavlinkVersion, read.MavlinkVersion);
    }

    [Fact]
    public void Encoder_V2_Heartbeat_Roundtrip()
    {
        var msg = new HeartbeatMessage
        {
            Type = 6,
            Autopilot = 8,
            BaseMode = 0x81,
            CustomMode = 0x11223344u,
            SystemStatus = 4,
            MavlinkVersion = 3
        };

        var registry = new KnownMessages();
        var encoder = new MavLinkEncoder(registry);
        var ser = new HeartbeatSerializer();
        var buf = new ArrayBufferWriter<byte>();

        int written = encoder.WriteV2(msg, ser, 0, 9, 1, 1, buf);

        var decoder = new MavLinkDecoder(registry);
        bool ok = decoder.TryReadFrame(buf.WrittenSpan, out var parsed, out int consumed);
        Assert.True(ok);
        Assert.Equal(written, consumed);
        
        var frame = Assert.IsType<FrameV2>(parsed);
        var read  = ser.Read(frame.Payload);

        Assert.Equal(msg.CustomMode, read.CustomMode);
        Assert.Equal(msg.Type, read.Type);
        Assert.Equal(msg.Autopilot, read.Autopilot);
        Assert.Equal(msg.BaseMode, read.BaseMode);
        Assert.Equal(msg.SystemStatus, read.SystemStatus);
        Assert.Equal(msg.MavlinkVersion, read.MavlinkVersion);
    }
}
