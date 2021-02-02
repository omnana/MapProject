using System.Collections.Generic;

public delegate void MessageHandler<T>(object sender, MessageArgs<T> args);
public class MessageAggregator<T>
{
    private readonly Dictionary<string, MessageHandler<T>> messages = new Dictionary<string, MessageHandler<T>>();

    public static readonly MessageAggregator<T> Instance = new MessageAggregator<T>();

    private MessageAggregator()
    {

    }

    public void Subscribe(string name, MessageHandler<T> handler)
    {
        if (!messages.ContainsKey(name))
        {
            messages.Add(name, handler);
        }
        else
        {
            messages[name] += handler;
        }

    }
    
    public void Publish(string name, object sender, MessageArgs<T> args)
    {
        if (messages.ContainsKey(name) && messages[name] != null)
        {
            //转发
            messages[name](sender, args);
        }
    }
}
