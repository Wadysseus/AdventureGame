using System;
using System.Collections.Generic;

namespace Game.Core.Messaging
{
  public interface IMessageHub
  {
    void Send(Message message);
    void Attach(Object registrant);
    void Detach(Object registrant);
    void Attach(IMessageHub registrant);
    void Detach(IMessageHub registrant);
    bool RelaysTo(IMessageHub hub);
  }

  public class MessageHub : IMessageHub
  {
    private Dictionary<Type, List<ListenerMethodInfo>> Streams = new Dictionary<Type, List<ListenerMethodInfo>>();
    private List<IMessageHub> Relays = new List<IMessageHub>();

    public void Send(Message message)
    {
      SendToListeners(message);
      SendToRelays(message);
    }

    private void SendToRelays(Message message)
    {
      foreach (var hub in Relays)
        hub.Send(message);
    }

    private void SendToListeners(Message message)
    {
      var message_type = message.GetType();

      if (!Streams.ContainsKey(message_type))
        return;

      ListenerMethodInfo[] listeners = new ListenerMethodInfo[Streams[message_type].Count];
      Streams[message_type].CopyTo(listeners);

      foreach (var listener in listeners)
        listener.Callback.Invoke(listener.Caller, new object[] { message });

      foreach (var component in message.Components)
        if (component != null)
          Send(component);
    }

    public void Attach(Object registrant)
    {
      var listener_methods = Cache.GetListenerMethodInfo(registrant.GetType());

      if (listener_methods == null)
        return;

      foreach (var info in listener_methods)
      {
        if (!Streams.ContainsKey(info.ParameterType))
          Streams.Add(info.ParameterType, new List<ListenerMethodInfo>());

        if (Streams[info.ParameterType].Find(x => x.Caller == registrant) != null)
          return;

        Streams[info.ParameterType].Add(new ListenerMethodInfo
        {
          Caller = registrant,
          Callback = info.Callback,
          ParameterType = info.ParameterType,
        });
      }
    }

    public void Attach(IMessageHub registrant)
    {
      if (registrant.RelaysTo(this))
        throw new InvalidOperationException("Attempted to create a circular relay between hubs!");

      Relays.Add(registrant);
    }

    public void Detach(Object registrant)
    {
      var listener_methods = Cache.GetListenerMethodInfo(registrant.GetType());

      if (listener_methods == null)
        return;

      foreach (var info in listener_methods)
      {
        if (!Streams.ContainsKey(info.ParameterType))
          return;

        var target = Streams[info.ParameterType].Find(x => x.Caller == registrant);
        Streams[info.ParameterType].Remove(target);

        if (Streams[info.ParameterType].Count == 0)
          Streams.Remove(info.ParameterType);
      }
    }

    public void Detach(IMessageHub registrant)
    {
      Relays.Remove(registrant);
    }

    public bool RelaysTo(IMessageHub hub)
    {
      return Relays.Contains(hub);
    }
  }
}
