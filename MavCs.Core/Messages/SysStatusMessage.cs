namespace MavCs.Core.Messages;

// MAVLink message: SYS_STATUS (ID: 1, CRC_EXTRA: 124)
[MavMessage(Id = 1u, CrcExtra = 124, Name = "SYS_STATUS")]
public sealed class SysStatusMessage
{
    public uint OnboardControlSensorsPresent { get; set; }
    public uint OnboardControlSensorsEnabled { get; set; }
    public uint OnboardControlSensorsHealth  { get; set; }
    public ushort Load                       { get; set; }
    public ushort VoltageBattery             { get; set; }
    public short  CurrentBattery             { get; set; }
    public sbyte  BatteryRemaining           { get; set; }
    public ushort DropRateComm               { get; set; }
    public ushort ErrorsComm                 { get; set; }
    public ushort ErrorsCount1               { get; set; }
    public ushort ErrorsCount2               { get; set; }
    public ushort ErrorsCount3               { get; set; }
    public ushort ErrorsCount4               { get; set; }
}
