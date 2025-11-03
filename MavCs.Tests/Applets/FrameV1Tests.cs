using System.Buffers;
using MavCs.Core.Protocol;
using MavCs.Core.Runtime;
using MavCs.Core.Registry;
using Xunit;

namespace MavCs.Tests;

public class FrameV1Tests
{
    [Fact]
    public void V1_Roundtrip_Succeeds()
    {
        var frame = new FrameV1
        {
            Sequence = 1,
            SystemId = 1,
            ComponentId = 1,
            MessageId = 0, // HEARTBEAT
            Payload = new byte[] { 0x01, 0x02, 0x03 }
        };

        var buffer = new ArrayBufferWriter<byte>();
        FrameV1.Write(frame, buffer, id => 50);

        var decoder = new MavLinkDecoder(new KnownMessages());
        bool ok = decoder.TryReadFrame(buffer.WrittenSpan, out var parsed, out int consumed);
        
        Assert.True(ok);
        Assert.Equal(buffer.WrittenCount, consumed);
        var f = Assert.IsType<FrameV1>(parsed);
        Assert.Equal(frame.MessageId, f.MessageId);
        Assert.Equal(frame.Payload, f.Payload);
    }
}
