using System;

namespace MavCs.Core.Messages;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class MavMessageAttribute : Attribute
{
    public uint Id { get; init; }
    
    public byte CrcExtra { get; init; }

    public string Name { get; init; } = string.Empty;
}
