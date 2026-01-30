namespace MavCs.Core.Messages;

[MavMessage(Id = 40u, CrcExtra = 230, Name = "MISSION_REQUEST")]
public sealed class MissionRequestMessage
{
    // Wire Order: seq, target_system, target_component, mission_type
    public ushort Seq { get; set; }
    public byte TargetSystem { get; set; }
    public byte TargetComponent { get; set; }
    public byte MissionType { get; set; }
}
