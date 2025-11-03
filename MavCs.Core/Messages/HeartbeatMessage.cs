using MavCs.Core.Protocol;

namespace MavCs.Core.Messages;

[MavMessage(Id = 0u, CrcExtra = 50, Name = "HEARTBEAT")]
public sealed class HeartbeatMessage
{  
    // Vehicle or component type
    public byte Type { get; set; }
    
    // Autopilot type
    public byte Autopilot { get; set; }
    
    // System mode bitmap
    public byte BaseMode { get; set; }
    
    // A bitfield for use for autopilot-specific flags
    public uint CustomMode { get; set; }
    
    // System status flag
    public byte SystemStatus { get; set; }
    
    // MAVLink version (added automatically by protocol).
    // Usually 3 for MAVLink v2, not written by the user.
    public byte MavlinkVersion { get; set; } = 3;
}
