using System.Buffers;
using MavCs.Core.Protocol;
using MavCs.Core.Runtime;
using Xunit;

namespace MavCs.Tests;

public class FrameV1Tests
{
    private static byte GetExtra(uint msgId) => msgId switch
    {
        0 => 50, // Heartbeat
        _ => 0
    };

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
        FrameV1.Write(frame, buffer, GetExtra);

        var decoder = new MavLinkDecoder(GetExtra);
        bool ok = decoder.TryReadFrame(buffer.WrittenSpan, out var parsed, out int consumed);
        
        Assert.True(ok);
        Assert.Equal(buffer.WrittenCount, consumed);
        var f = Assert.IsType<FrameV1>(parsed);
        Assert.Equal(frame.MessageId, f.MessageId);
        Assert.Equal(frame.Payload, f.Payload);
    }
}
