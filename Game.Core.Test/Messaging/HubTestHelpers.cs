using Game.Core.Messaging;

namespace Game.Core.Test.Messaging
{
  public class TestMessage : Message { }
  public class TestMessageDerived : TestMessage { }
  public class ComponentMessage : Message
  {
    public ComponentMessage()
    {
      Components.Add(new TestMessage());
    }
  }

  public class TestListener
  {
    public int componentmessage_callbacks = 0;
    public int testmessage_callbacks = 0;
    public int testmessage_derived_callbacks = 0;

    public void OnTestMessage(TestMessage t)
    {
      ++testmessage_callbacks;
    }

    public void OnTestMessageDerived(TestMessageDerived t)
    {
      ++testmessage_derived_callbacks;
    }

    public void OnComponentMessage(ComponentMessage t)
    {
      ++componentmessage_callbacks;
    }
  }

  public class UnregisterableListener
  {

  }
}
