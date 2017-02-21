namespace Game.Core.Messaging
{
  using System;
  using System.Runtime.Serialization;

  public class CannotRegisterType : Exception, ISerializable
  {
    public CannotRegisterType()
    {
    }

    public CannotRegisterType(string message) : base(message)
    {
    }

    public CannotRegisterType(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected CannotRegisterType(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}
