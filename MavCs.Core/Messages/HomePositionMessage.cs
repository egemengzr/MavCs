namespace MavCs.Core.Messages;

[MavMessage(Id = 242u, CrcExtra = 104, Name = "HOME_POSITION")]
public sealed class HomePositionMessage
{
    // Wire Order: latitude, longitude, altitude, x, y, z, q[4], approach_x, approach_y, approach_z
    public int Latitude { get; set; }
    public int Longitude { get; set; }
    public int Altitude { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float[]? Q { get; set; } // float[4]
    public float ApproachX { get; set; }
    public float ApproachY { get; set; }
    public float ApproachZ { get; set; }
}
