using MavCs.Core.Messages;
using MavCs.Core.Serialization;

namespace MavCs.Tests.MessageTests;

public class StatustextSerializerTests
{
    [Fact]
    public void Statustext_Payload_Roundtrip()
    {
        var src = new StatustextMessage
        {
            Severity = 0,
            Text = "hello_world",
            Id = 2,
            ChunkSeq = 4
        };

        Span<byte> buf = stackalloc byte[StatustextSerializer.PayloadLength];
        var ser = new StatustextSerializer();
        int n = ser.Write(src, buf);
        Assert.Equal(StatustextSerializer.PayloadLength, n);

        var dst = ser.Read(buf);
        Assert.Equal(src.Severity, dst.Severity);
        Assert.Equal(src.Text, dst.Text);
        Assert.Equal(src.Id, dst.Id);
        Assert.Equal(src.ChunkSeq, dst.ChunkSeq);
    }
}
