namespace MavCs.Core.Messages;

[MavMessage(Id = 65u, CrcExtra = 118, Name = "RC_CHANNELS")]
public sealed class RcChannelsMessage
{
    // Wire Order: time_boot_ms, chan1_raw .. chan18_raw, port, rssi
    public uint TimeBootMs { get; set; }
    public ushort Chan1Raw { get; set; }
    public ushort Chan2Raw { get; set; }
    public ushort Chan3Raw { get; set; }
    public ushort Chan4Raw { get; set; }
    public ushort Chan5Raw { get; set; }
    public ushort Chan6Raw { get; set; }
    public ushort Chan7Raw { get; set; }
    public ushort Chan8Raw { get; set; }
    public ushort Chan9Raw { get; set; }
    public ushort Chan10Raw { get; set; }
    public ushort Chan11Raw { get; set; }
    public ushort Chan12Raw { get; set; }
    public ushort Chan13Raw { get; set; }
    public ushort Chan14Raw { get; set; }
    public ushort Chan15Raw { get; set; }
    public ushort Chan16Raw { get; set; }
    public ushort Chan17Raw { get; set; }
    public ushort Chan18Raw { get; set; }
    public byte Port { get; set; }
    public byte Rssi { get; set; }
}
