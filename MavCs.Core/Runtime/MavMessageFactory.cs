using System.Collections.Concurrent;
using System.Reflection;

using MavCs.Core.Abstractions;
using MavCs.Core.Messages;
using MavCs.Core.Protocol;
using MavCs.Core.Serialization;

namespace MavCs.Core.Runtime;

public sealed class MavMessageFactory : IMessageFactory
{
    private readonly ConcurrentDictionary<uint, Func<ReadOnlySpan<byte>, object>> _deserialized = new();
    private bool _initialzed;
    
    public MavMessageFactory()
    {
        DiscoverMessages();
    }
    
    // Scans loaded assemblies for message classes with [MavMessage] and their serializers
    private void DiscoverMessages()
    {
        if (this._initialzed) return;
        this._initialzed = true;

        var asm = typeof(MavMessageAttribute).Assembly;

        foreach (var type in asm.GetTypes())
        {
            var attr = type.GetCustomAttribute<MavMessageAttribute>();
            if (attr is null)
                continue;
            
            // Expected serializer class name convention
            var serializerName = type.Name.Replace("Message", "Serializer");
            var serializerType = asm.GetTypes()
                .FirstOrDefault(t => t.Name == serializerName);
            if (serializerType == null)
                continue;
            
            // Find a Read(ReadOnlySpan<byte>) method
            var readMethod = serializerType.GetMethod("Read", new[] { typeof(ReadOnlySpan<byte>) });
            if (readMethod == null)
                continue;
            
            // Create an instance of serializer 
            var serializerInstance = Activator.CreateInstance(serializerType);
            if (serializerInstance == null)
                continue;
            
            // Create delegate safely without boxing ReadOnlySpan
            Func<ReadOnlySpan<byte>, object> func;

            try
            {
                func = (Func<ReadOnlySpan<byte>, object>)Delegate.CreateDelegate(
                    typeof(Func<ReadOnlySpan<byte>, object>),
                    serializerInstance,
                    readMethod
                );
            }
            catch
            {
                // Fallback for non-refstruct methods
                func = span =>
                {
                    var tmp = span.ToArray();
                    return readMethod.Invoke(serializerInstance, new object?[] { tmp })!;
                };
            }

            this._deserialized[attr.Id] = func;
        }
    }

    public bool TryDeserializeFrame(FrameBase frame, out object? message)
    {
        if (this._deserialized.TryGetValue(frame.MessageId, out var reader))
        {
            message = reader(frame.Payload);
            return true;
        }

        message = null;
        return false;
    }
}
