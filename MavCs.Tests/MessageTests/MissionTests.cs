using MavCs.Core.Messages;
using MavCs.Core.Serialization;

namespace MavCs.Tests.MessageTests;

public class MissionTests
{
    [Fact]
    public void MissionItemInt_Roundtrip()
    {
        var src = new MissionItemIntMessage
        {
            Seq = 0,
            Command = 16,
            Frame = 3,
            X = 410000000,
            Y = 290000000,
            Z = 50,
            Current = 1,
            Autocontinue = 1,
            MissionType = 0
        };

        Span<byte> buf = stackalloc byte[38];
        var ser = new MissionItemIntSerializer();

        ser.Write(src, buf);
        var dst = ser.Read(buf);

        Assert.Equal(src.X, dst.X);
        Assert.Equal(src.Command, dst.Command);
        Assert.Equal(src.Current, dst.Current);
    }

    [Fact]
    public void MissionRequest_Ack_Count_Roundtrip()
    {
        var countSrc = new MissionCountMessage { Count = 5, TargetSystem = 1, MissionType = 0 };
        var serCount = new MissionCountSerializer();
        Span<byte> bufCount = stackalloc byte[5];
        serCount.Write(countSrc, bufCount);
        var dstCount = serCount.Read(bufCount);
        Assert.Equal(countSrc.Count, dstCount.Count);
        
        var reqSrc = new MissionRequestMessage { Seq = 2, TargetSystem = 1 };
        var serReq = new MissionRequestSerializer();
        Span<byte> bufReq = stackalloc byte[5];
        serReq.Write(reqSrc, bufReq);
        var dstReq = serReq.Read(bufReq);
        Assert.Equal(reqSrc.Seq, dstReq.Seq);
        
        var ackSrc = new MissionAckMessage { Type = 0, TargetSystem = 1 };
        var serAck = new MissionAckSerializer();
        Span<byte> bufAck = stackalloc byte[4];
        serAck.Write(ackSrc, bufAck);
        var dstAck = serAck.Read(bufAck);
        Assert.Equal(ackSrc.Type, dstAck.Type);
    }
}
