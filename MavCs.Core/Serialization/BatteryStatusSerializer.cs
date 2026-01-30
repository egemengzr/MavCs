using MavCs.Core.Abstractions;
using MavCs.Core.Messages;
using MavCs.Core.Serialization;

namespace MavCs.Core.Serialization;

public sealed class BatteryStatusSerializer : IMessageSerializer<BatteryStatusMessage>
{
    public const int PayloadLength = 36;

    public int Write(BatteryStatusMessage message, Span<byte> dst)
    {
        if (dst.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(dst));
        var w = new LESpanWriter(dst);

        w.WriteInt32(message.EnergyConsumed);
        w.WriteInt32(message.TimeRemaining);

        if (message.Voltages != null && message.Voltages.Length >= 10)
        {
            for (int i = 0; i < 10; i++) w.WriteUInt16(message.Voltages[i]);
        }
        else
        {
            for (int i = 0; i < 10; i++) w.WriteUInt16(0xFFFF);
        }

        w.WriteInt16(message.CurrentBattery);
        w.WriteByte(message.Id);
        w.WriteByte(message.BatteryFunction);
        w.WriteByte(message.Type);
        w.WriteSByte(message.BatteryRemaining);

        return PayloadLength;
    }

    public BatteryStatusMessage Read(ReadOnlySpan<byte> src)
    {
        if (src.Length < PayloadLength) throw new ArgumentOutOfRangeException(nameof(src));
        var r = new LESpanReader(src);

        var msg = new BatteryStatusMessage
        {
            EnergyConsumed = r.ReadInt32(),
            TimeRemaining = r.ReadInt32(),
            Voltages = new ushort[10]
        };

        for (int i = 0; i < 10; i++) msg.Voltages[i] = r.ReadUInt16();

        msg.CurrentBattery = r.ReadInt16();
        msg.Id = r.ReadByte();
        msg.BatteryFunction = r.ReadByte();
        msg.Type = r.ReadByte();
        msg.BatteryRemaining = r.ReadSByte();

        return msg;
    }
}
