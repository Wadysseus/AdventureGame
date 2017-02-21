namespace Game.Core.Messaging
{
  using System.Collections.Generic;

  public abstract class Message
  {
    public List<Message> Components = new List<Message>();
  }
}
