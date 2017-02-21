using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Core.Messaging
{
  internal static class Cache
  {
    internal static Dictionary<Type, List<ListenerMethodInfo>> Listeners = new Dictionary<Type, List<ListenerMethodInfo>>();

    internal static List<ListenerMethodInfo> GetListenerMethodInfo(Type type)
    {
      if (!Listeners.ContainsKey(type) && !TryRegisterListener(type))
        return null;

      return Listeners[type];
    }

    private static bool TryRegisterListener(Type type)
    {
      bool registration_success = false;

      foreach (var method in type.GetMethods())
      {
        var parameters = method.GetParameters();

        if (!HasOneMessageParameter(parameters))
          continue;

        if (!Listeners.ContainsKey(type))
          Listeners.Add(type, new List<ListenerMethodInfo>());

        Listeners[type].Add(new ListenerMethodInfo
        {
          Caller = null,
          Callback = method,
          ParameterType = parameters[0].ParameterType,
        });

        registration_success = true;
      }

      if (registration_success)
        return true;

      throw new
        CannotRegisterType(
          String.Format(
            "Messenger cannot register {0}! No valid methods detected. "
            + "\nValid methods are public, return void, and have one parameter that inherits from Messaging.Message",
            typeof(Message).ToString()
        ));
    }

    private static bool HasOneMessageParameter(ParameterInfo[] parameters)
    {
      return (parameters.Length > 0 && typeof(Message).IsAssignableFrom(parameters[0].ParameterType));
    }
  }
}
