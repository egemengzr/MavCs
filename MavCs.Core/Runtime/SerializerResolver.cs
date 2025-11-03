using System.Collections.Concurrent;
using System.Reflection;

using MavCs.Core.Messages;

namespace MavCs.Tests.Runtime;

internal static class SerializerResolver
{
    private static readonly ConcurrentDictionary<Type, object?> _cache = new();

    public static object? GetSerializer(Type messageType)
    {
        if (_cache.TryGetValue(messageType, out var hit))
            return hit;

        var asm = typeof(MavMessageAttribute).Assembly;
        var serializerName = messageType.Name.Replace("Message", "Serializer");
        var serializerType = asm.GetTypes().FirstOrDefault(t => t.Name == serializerName);

        var instance = serializerType is null ? null : Activator.CreateInstance(serializerType);
        _cache[messageType] = instance;
        return instance;
    }

    public static (uint msgId, byte crcExtra) GetMetadata(Type messageType)
    {
        var attr = messageType.GetCustomAttribute<MavMessageAttribute>();
        if (attr is null)
            throw new InvalidOperationException($"{messageType.Name} has no [MavMessage] attribute.");
        return (attr.Id, attr.CrcExtra);
    }
}
