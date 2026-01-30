namespace MavCs.Core.Messages;

[MavMessage(Id = 44u, CrcExtra = 221, Name = "MISSION_COUNT")]
public sealed class MissionCountMessage
{
    // Wire Order: count, target_system, target_component, mission_type
    public ushort Count { get; set; }
    public byte TargetSystem { get; set; }
    public byte TargetComponent { get; set; }
    public byte MissionType { get; set; }
}
