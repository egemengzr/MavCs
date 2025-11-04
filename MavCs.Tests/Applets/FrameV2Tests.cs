using System.Buffers;
using MavCs.Core.Protocol;
using MavCs.Core.Runtime;
using MavCs.Core.Registry;
using Xunit;

namespace MavCs.Tests.Applets;

public class FrameV2Tests
{
    [Fact]
    public void V2_Roundtrip_Succeeds()
    {
        var frame = new FrameV2
        {
            IncompatFlags = 0,
            CompatFlags = 0,
            Sequence = 42,
            SystemId = 1,
            ComponentId = 1,
            MessageId = 0, // Heartbeat
            Payload = new byte[] { 0x10, 0x20, 0x30, 0x40 }
        };

        var buffer = new ArrayBufferWriter<byte>();
        FrameV2.Write(frame, buffer, id => 50);

        var decoder = new MavLinkDecoder(new KnownMessages());
        bool ok = decoder.TryReadFrame(buffer.WrittenSpan, out var parsed, out int consumed);
        
        Assert.True(ok);
        Assert.Equal(buffer.WrittenCount, consumed);

        var f = Assert.IsType<FrameV2>(parsed);
        Assert.Equal(frame.MessageId, f.MessageId);
        Assert.Equal(frame.Payload, f.Payload);
        Assert.Equal(frame.Sequence, f.Sequence);
        Assert.Equal(frame.SystemId, f.SystemId);
        Assert.Equal(frame.ComponentId, f.ComponentId);
    }
}
