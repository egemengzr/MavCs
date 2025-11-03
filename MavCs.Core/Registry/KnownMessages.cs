using System.Collections.Concurrent;
using MavCs.Core.Abstractions;

namespace MavCs.Core.Registry;

public sealed class KnownMessages : IMessageRegistry
{
    // Thread-safe for hot-reload or reflection-based population.
    private static readonly ConcurrentDictionary<uint, byte> _crcExtra = new()
    {
        [0u] = 50, // Heartbeat
    };

    public byte? GetCrcExtra(uint messageId)
        => _crcExtra.TryGetValue(messageId, out var extra) ? extra : null;
}
