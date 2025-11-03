using System.Collections.Concurrent;
using System.Reflection;

using MavCs.Core.Abstractions;
using MavCs.Core.Messages;

namespace MavCs.Core.Registry;

public sealed class KnownMessages : IMessageRegistry
{
    private static readonly ConcurrentDictionary<uint, byte> _crcExtra = new();
    private static bool _initialized;

    public KnownMessages()
    {
        EnsureInitialized();
    }

    public byte? GetCrcExtra(uint messageId)
        => _crcExtra.TryGetValue(messageId, out var extra) ? extra : null;

    private static void EnsureInitialized()
    {
        if (_initialized) return;
        // extend this to scan those assemblies as well.
        Assembly asm = typeof(MavMessageAttribute).Assembly;

        foreach (Type t in asm.GetTypes())
        {
            var attr = t.GetCustomAttribute<MavMessageAttribute>();
            if (attr is null) continue;
            
            // If duplicate IDs appear, last one wins (but contributors shouldn't duplicate).
            _crcExtra[attr.Id] = attr.CrcExtra;
        }

        _initialized = true;
    }
}
