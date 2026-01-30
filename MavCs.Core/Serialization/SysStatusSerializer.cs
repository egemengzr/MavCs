using MavCs.Core.Abstractions;
using MavCs.Core.Messages;

namespace MavCs.Core.Serialization;

public sealed class SysStatusSerializer : IMessageSerializer<SysStatusMessage>
{
    public const int PayloadLength = 31;

    public int Write(SysStatusMessage msg, Span<byte> dst)
    {
        var w = new LESpanWriter(dst);
        
        w.WriteUInt32(msg.OnboardControlSensorsPresent);
        w.WriteUInt32(msg.OnboardControlSensorsEnabled);
        w.WriteUInt32(msg.OnboardControlSensorsHealth);
        
        w.WriteUInt16(msg.Load);
        w.WriteUInt16(msg.VoltageBattery);
        w.WriteInt16(msg.CurrentBattery);
        w.WriteSByte(msg.BatteryRemaining);

        w.WriteUInt16(msg.DropRateComm);
        w.WriteUInt16(msg.ErrorsComm);
        w.WriteUInt16(msg.ErrorsCount1);
        w.WriteUInt16(msg.ErrorsCount2);
        w.WriteUInt16(msg.ErrorsCount3);
        w.WriteUInt16(msg.ErrorsCount4);

        return w.BytesWritten;
    }

    public SysStatusMessage Read(ReadOnlySpan<byte> src)
    {
        var r = new LESpanReader(src);
        var m = new SysStatusMessage();
        
        m.OnboardControlSensorsPresent = r.ReadUInt32();
        m.OnboardControlSensorsEnabled = r.ReadUInt32();
        m.OnboardControlSensorsHealth  = r.ReadUInt32();

        m.Load             = r.ReadUInt16();
        m.VoltageBattery   = r.ReadUInt16();
        m.CurrentBattery   = r.ReadInt16();
        m.BatteryRemaining = r.ReadSByte();

        m.DropRateComm   = r.ReadUInt16();
        m.ErrorsComm     = r.ReadUInt16();
        m.ErrorsCount1   = r.ReadUInt16();
        m.ErrorsCount2   = r.ReadUInt16();
        m.ErrorsCount3   = r.ReadUInt16();
        m.ErrorsCount4   = r.ReadUInt16();

        return m;
    }
}
