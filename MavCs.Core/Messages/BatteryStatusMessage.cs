namespace MavCs.Core.Messages;

[MavMessage(Id = 147u, CrcExtra = 154, Name = "BATTERY_STATUS")]
public sealed class BatteryStatusMessage
{
    // Wire Order: energy_consumed, time_remaining, voltages[10], current_battery, id, battery_function, type, battery_remaining, charge_state (+ extensions for faults if MAVLink 2)
    public int EnergyConsumed { get; set; }
    public int TimeRemaining { get; set; }
    public ushort[]? Voltages { get; set; } // ushort[10]
    public short CurrentBattery { get; set; }
    public byte Id { get; set; }
    public byte BatteryFunction { get; set; }
    public byte Type { get; set; }
    public sbyte BatteryRemaining { get; set; }
}
