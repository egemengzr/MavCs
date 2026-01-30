namespace MavCs.Core.Messages;

[MavMessage(Id = 148u, CrcExtra = 178, Name = "AUTOPILOT_VERSION")]
public sealed class AutopilotVersionMessage
{
    // Wire Order: capabilities, uid, flight_sw_version, middleware_sw_version, os_sw_version, board_version, vendor_id, product_id, flight_custom_version[8], middleware_custom_version[8], os_custom_version[8]
    public ulong Capabilities { get; set; }
    public ulong Uid { get; set; }
    public uint FlightSwVersion { get; set; }
    public uint MiddlewareSwVersion { get; set; }
    public uint OsSwVersion { get; set; }
    public uint BoardVersion { get; set; }
    public ushort VendorId { get; set; }
    public ushort ProductId { get; set; }
    public byte[]? FlightCustomVersion { get; set; }      // byte[8]
    public byte[]? MiddlewareCustomVersion { get; set; }  // byte[8]
    public byte[]? OsCustomVersion { get; set; }          // byte[8]
}
