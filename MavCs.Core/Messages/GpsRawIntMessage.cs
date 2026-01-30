namespace MavCs.Core.Messages;

[MavMessage(Id = 24u, CrcExtra = 24, Name = "GPS_RAW_INT")]
public sealed class GpsRawIntMessage
{
    // Wire Order: time_usec, lat, lon, alt, eph, epv, vel, cog, fix_type, satellites_visible
    public ulong TimeUsec { get; set; }
    public int Lat { get; set; }
    public int Lon { get; set; }
    public int Alt { get; set; }
    public ushort Eph { get; set; }
    public ushort Epv { get; set; }
    public ushort Vel { get; set; }
    public ushort Cog { get; set; }
    public byte FixType { get; set; }
    public byte SatellitesVisible { get; set; }
}
