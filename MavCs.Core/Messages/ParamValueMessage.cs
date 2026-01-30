namespace MavCs.Core.Messages;

[MavMessage(Id = 22u, CrcExtra = 220, Name = "PARAM_VALUE")]
public sealed class ParamValueMessage
{
    // Wire Order: param_value, param_count, param_index, param_id[16], param_type
    public float ParamValue { get; set; }
    public ushort ParamCount { get; set; }
    public ushort ParamIndex { get; set; }
    public string? ParamId { get; set; }
    public byte ParamType { get; set; }
}
