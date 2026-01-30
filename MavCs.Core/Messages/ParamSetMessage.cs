namespace MavCs.Core.Messages;

[MavMessage(Id = 23u, CrcExtra = 168, Name = "PARAM_SET")]
public sealed class ParamSetMessage
{
    // Wire Order: param_value, target_system, target_component, param_id[16], param_type
    public float ParamValue { get; set; }
    public byte TargetSystem { get; set; }
    public byte TargetComponent { get; set; }
    public string? ParamId { get; set; }
    public byte ParamType { get; set; }
}
