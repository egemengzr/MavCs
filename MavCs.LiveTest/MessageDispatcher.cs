namespace MavCs.LiveTest;

public class MessageDispatcher
{
    private readonly Dictionary<Type, Action<object>> _handlers = new();
    
    public void Subscribe<TMessage>(Action<TMessage> handler)
    {
        _handlers[typeof(TMessage)] = msg => handler((TMessage)msg);
    }

    public void Dispatch(object message)
    {
        if (message == null) return;
        
        var type = message.GetType();
        
        if (_handlers.TryGetValue(type, out var handler))
        {
            handler(message);
        }
        else
        {
            Console.WriteLine($"⚠️  No handler for: {type.Name}");
        }
    }
}
