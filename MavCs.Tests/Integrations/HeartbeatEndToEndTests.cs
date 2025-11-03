using System.Buffers;

using MavCs.Core.Messages;
using MavCs.Core.Protocol;
using MavCs.Core.Registry;
using MavCs.Core.Runtime;
using MavCs.Core.Serialization;

namespace MavCs.Tests.Integrations;

public class HeartbeatEndToEndTests
{
    [Fact]
    public void V1_Heartbeat_EndToEnd_Roundtrip()
    {
        var message = new HeartbeatMessage
        {
            Type = 6,
            Autopilot = 8,
            BaseMode = 0x81,
            CustomMode = 0x11223344u,
            SystemStatus = 4,
            MavlinkVersion = 3
        };

        var serializer = new HeartbeatSerializer();
        Span<byte> payload = stackalloc byte[HeartbeatSerializer.PayloadLength];
        serializer.Write(message, payload);

        var frame = new FrameV1()
        {
            Sequence = 42,
            SystemId = 1,
            ComponentId = 1,
            MessageId = 0, // Heartbeat
            Payload = payload.ToArray()
        };

        var buffer = new ArrayBufferWriter<byte>();
        FrameV1.Write(frame, buffer, id => 50);
        
        // decode
        var decoder = new MavLinkDecoder(new KnownMessages());
        bool ok = decoder.TryReadFrame(buffer.WrittenSpan, out var parsed, out int consumed);
        
        Assert.True(ok);
        Assert.Equal(buffer.WrittenCount, consumed);

        var f = Assert.IsType<FrameV1>(parsed);
        Assert.Equal((byte)0, f.MessageId);
        
        // deserialize payload
        var readMsg = serializer.Read(f.Payload);
        Assert.Equal(message.CustomMode, readMsg.CustomMode);
        Assert.Equal(message.Type, readMsg.Type);
        Assert.Equal(message.Autopilot, readMsg.Autopilot);
        Assert.Equal(message.BaseMode, readMsg.BaseMode);
        Assert.Equal(message.SystemStatus, readMsg.SystemStatus);
        Assert.Equal(message.MavlinkVersion, readMsg.MavlinkVersion);
    }

    [Fact]
    public void V2_Heartbeat_EndToEnd_Roundtrip()
    {
        var message = new HeartbeatMessage
        {
            Type = 6,
            Autopilot = 8,
            BaseMode = 0x81,
            CustomMode = 0x11223344u,
            SystemStatus = 4,
            MavlinkVersion = 3
        };

        var serializer = new HeartbeatSerializer();
        Span<byte> payload = stackalloc byte[HeartbeatSerializer.PayloadLength];
        serializer.Write(message, payload);

        var frame = new FrameV2
        {
            IncompatFlags = 0,
            CompatFlags = 0,
            Sequence = 99,
            SystemId = 1,
            ComponentId = 1,
            MessageId = 0,
            Payload = payload.ToArray()
        };

        var buffer = new ArrayBufferWriter<byte>();
        FrameV2.Write(frame, buffer, id => 50);

        var decoder = new MavLinkDecoder(new KnownMessages());
        bool ok = decoder.TryReadFrame(buffer.WrittenSpan, out var parsed, out int consumed);
        
        Assert.True(ok);
        Assert.Equal(buffer.WrittenCount, consumed);

        var f = Assert.IsType<FrameV2>(parsed);
        var readMsg = serializer.Read(f.Payload);
        Assert.Equal(message.CustomMode, readMsg.CustomMode);
        Assert.Equal(message.Type, readMsg.Type);
        Assert.Equal(message.Autopilot, readMsg.Autopilot);
        Assert.Equal(message.BaseMode, readMsg.BaseMode);
        Assert.Equal(message.SystemStatus, readMsg.SystemStatus);
        Assert.Equal(message.MavlinkVersion, readMsg.MavlinkVersion);
    }
}
