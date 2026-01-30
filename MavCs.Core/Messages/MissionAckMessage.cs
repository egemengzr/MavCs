namespace MavCs.Core.Messages;

[MavMessage(Id = 47u, CrcExtra = 153, Name = "MISSION_ACK")]
public sealed class MissionAckMessage
{
    // Wire Order: target_system, target_component, type, mission_type
    public byte TargetSystem { get; set; }
    public byte TargetComponent { get; set; }
    public byte Type { get; set; }
    public byte MissionType { get; set; }
}
