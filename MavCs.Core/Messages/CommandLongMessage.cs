namespace MavCs.Core.Messages;

[MavMessage(Id = 76u, CrcExtra = 152, Name = "COMMAND_LONG")]
public sealed class CommandLongMessage
{
    // Wire Order: param1..param7, command, target_system, target_component, confirmation
    public float Param1 { get; set; }
    public float Param2 { get; set; }
    public float Param3 { get; set; }
    public float Param4 { get; set; }
    public float Param5 { get; set; }
    public float Param6 { get; set; }
    public float Param7 { get; set; }
    public ushort Command { get; set; }
    public byte TargetSystem { get; set; }
    public byte TargetComponent { get; set; }
    public byte Confirmation { get; set; }
}
