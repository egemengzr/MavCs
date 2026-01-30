namespace MavCs.Core.Messages;

[MavMessage(Id = 33, CrcExtra = 104, Name = "GLOBAL_POSITION_INT")]
public sealed class GlobalPositionIntMessage
{
    public uint TimeBootMs { get; set; }
    public int Lat { get; set; }
    public int Lon { get; set; }
    public int Alt { get; set; }
    public int RelativeAlt { get; set; }
    public short Vx { get; set; }
    public short Vy { get; set; }
    public short Vz { get; set; }
    public ushort Hdg { get; set; }
}
