namespace MavCs.Core.Messages;

[MavMessage(Id = 20u, CrcExtra = 214, Name = "PARAM_REQUEST_READ")]
public sealed class ParamRequestReadMessage
{
    // Wire Order: param_index, target_system, target_component, param_id[16]
    public short ParamIndex { get; set; }
    public byte TargetSystem { get; set; }
    public byte TargetComponent { get; set; }
    public string? ParamId { get; set; }
}
