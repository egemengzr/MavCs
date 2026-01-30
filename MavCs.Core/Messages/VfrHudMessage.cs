namespace MavCs.Core.Messages;

[MavMessage(Id = 74u, CrcExtra = 20, Name = "VFR_HUD")]
public sealed class VfrHudMessage
{
    // Wire Order: airspeed, groundspeed, alt, climb, heading, throttle
    public float Airspeed { get; set; }
    public float Groundspeed { get; set; }
    public float Alt { get; set; }
    public float Climb { get; set; }
    public short Heading { get; set; }
    public ushort Throttle { get; set; }
}
