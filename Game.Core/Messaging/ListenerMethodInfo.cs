namespace Game.Core.Messaging
{
  using System;
  using System.Reflection;

  public class ListenerMethodInfo
  {
    public object Caller { get; set; }
    public MethodInfo Callback { get; set; }
    public Type ParameterType { get; set; }
  }
}
