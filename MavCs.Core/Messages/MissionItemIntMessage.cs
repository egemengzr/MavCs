namespace MavCs.Core.Messages;

[MavMessage(Id = 73u, CrcExtra = 38, Name = "MISSION_ITEM_INT")]
public sealed class MissionItemIntMessage
{
    // Wire Order: param1..4, x, y, z, seq, command, target_system, target_component, frame, current, autocontinue, mission_type
    public float Param1 { get; set; }
    public float Param2 { get; set; }
    public float Param3 { get; set; }
    public float Param4 { get; set; }
    public int X { get; set; } // Latitude
    public int Y { get; set; } // Longitude
    public float Z { get; set; } // Altitude
    public ushort Seq { get; set; }
    public ushort Command { get; set; }
    public byte TargetSystem { get; set; }
    public byte TargetComponent { get; set; }
    public byte Frame { get; set; }
    public byte Current { get; set; }
    public byte Autocontinue { get; set; }
    public byte MissionType { get; set; }
}
