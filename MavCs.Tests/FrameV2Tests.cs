using System.Buffers;
using MavCs.Core.Protocol;
using MavCs.Core.Runtime;
using Xunit;

namespace MavCs.Tests;

public class FrameV2Tests
{
    private static byte GetExtra(uint msgId) => msgId switch
    {
        0 => 50, // Heartbeat
        _ => 0
    };

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
        FrameV2.Write(frame, buffer, GetExtra);

        var decoder = new MavLinkDecoder(GetExtra);
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
