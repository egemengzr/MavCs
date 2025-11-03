using System.Buffers;

using MavCs.Core.Messages;
using MavCs.Core.Registry;
using MavCs.Core.Runtime;

namespace MavCs.Tests.Integrations;

public class AutoEncoderTests
{
    [Fact]
    public void Encoder_AutoDiscovers_Serializer_Heartbeat_V1()
    {
        var msg = new HeartbeatMessage()
        {
            Type = 6,
            Autopilot = 8,
            BaseMode = 0x81,
            CustomMode = 0x11223344u,
            SystemStatus = 4,
            MavlinkVersion = 3
        };

        var encoder = new MavLinkEncoder(new KnownMessages());
        var buf = new ArrayBufferWriter<byte>();

        int written = encoder.WriteV1(msg, sequence: 20, systemId: 1, componentId: 1, output: buf);

        var decoder = new MavLinkDecoder(new KnownMessages());
        Assert.True(decoder.TryReadFrame(buf.WrittenSpan, out var frame, out _));

        var factory = new MavMessageFactory();
        Assert.True(factory.TryDeserializeFrame(frame!, out var obj));
        Assert.IsType<HeartbeatMessage>(obj);
    }
    
    [Fact]
    public void Encoder_AutoDiscovers_Serializer_SysStatus_V1()
    {
        var msg = new SysStatusMessage
        {
            OnboardControlSensorsPresent = 1,
            OnboardControlSensorsEnabled = 2,
            OnboardControlSensorsHealth = 3,
            VoltageBattery = 11000,
            BatteryRemaining = 75
        };

        var encoder = new MavLinkEncoder(new KnownMessages());
        var buf = new ArrayBufferWriter<byte>();

        encoder.WriteV1(msg, sequence: 1, systemId: 1, componentId: 1, output: buf);

        var decoder = new MavLinkDecoder(new KnownMessages());
        Assert.True(decoder.TryReadFrame(buf.WrittenSpan, out var frame, out _));

        var factory = new MavMessageFactory();
        Assert.True(factory.TryDeserializeFrame(frame!, out var obj));
        Assert.IsType<SysStatusMessage>(obj);
    }

}
