using System.Reflection;

using MavCs.Core.Messages;

namespace MavCs.Core.Runtime
{
    /// Lightweight hub that fans out decoded message objects
    /// to type-based and/or MAVLink Id-based handlers.
    internal sealed class MessageDispatcher
    {
        private readonly Dictionary<Type, List<Delegate>> _byType = new();
        private readonly Dictionary<uint, List<Action<object>>> _byId   = new();

        // --- Register by CLR type (preferred when you know the C# class) ---
        public void Register<T>(Action<T> handler) where T : class
        {
            var type = typeof(T);
            if (!_byType.TryGetValue(type, out var list))
                _byType[type] = list = new List<Delegate>();
            list.Add(handler);
        }

        // --- Register by MAVLink message Id (useful if you don't want to reference the C# class) ---
        public void Register(uint messageId, Action<object> handler)
        {
            if (!_byId.TryGetValue(messageId, out var list))
                _byId[messageId] = list = new List<Action<object>>();
            list.Add(handler);
        }

        // --- Dispatch using the decoded CLR object ---
        public void Dispatch(object msg)
        {
            var t = msg.GetType();

            // 1) Type-based handlers
            if (_byType.TryGetValue(t, out var typed))
                foreach (var h in typed)
                    h.DynamicInvoke(msg);

            // 2) Id-based handlers (via [MavMessage(Id=...)] on the class)
            if (TryGetMavId(t, out var id) && _byId.TryGetValue(id, out var byId))
                foreach (var h in byId)
                    h(msg);
        }

        // --- Dispatch using known (id, payload) pair ---
        public void Dispatch(uint messageId, object payload)
        {
            // Id-based
            if (_byId.TryGetValue(messageId, out var byId))
                foreach (var h in byId)
                    h(payload);

            // Also try type-based (if consumer registered by CLR type)
            var t = payload.GetType();
            if (_byType.TryGetValue(t, out var typed))
                foreach (var h in typed)
                    h.DynamicInvoke(payload);
        }

        private static bool TryGetMavId(Type t, out uint id)
        {
            // Your attribute lives under Messages/MavMessageAttribute.cs
            var attr = t.GetCustomAttribute(typeof(MavMessageAttribute)) as MavMessageAttribute;
            if (attr is not null) { id = attr.Id; return true; }
            id = 0; return false;
        }
    }
}
