namespace MavCs.Core.Abstractions;

/// <summary>
/// Provides metadata for MAVLink messages (e.g., CRC extra).
/// This is the single source of truth contributors will update via PRs.
/// </summary>
public interface IMessageRegistry
{
    byte? GetCrcExtra(uint messageId);
}
