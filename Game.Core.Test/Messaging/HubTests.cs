using Game.Core.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Game.Core.Test.Messaging
{
  [TestClass]
  public class HubTests
  {
    private IMessageHub hub;
    private TestListener listener;

    [TestInitialize]
    public void Init()
    {
      hub = new MessageHub();
      listener = new TestListener();
    }

    [TestMethod]
    public void Attach()
    {
      hub.Attach(listener);
      var message = new TestMessage();
      hub.Send(message);

      Assert.AreEqual(1, listener.testmessage_callbacks);
    }

    [TestMethod]
    public void Attach_Repeated()
    {
      hub.Attach(listener);
      hub.Attach(listener);
      hub.Attach(listener);
      hub.Send(new TestMessage());

      Assert.AreEqual(1, listener.testmessage_callbacks);
    }

    [TestMethod]
    public void Detach()
    {
      hub.Attach(listener);
      hub.Send(new TestMessage());
      hub.Detach(listener);
      hub.Send(new TestMessage());

      Assert.AreEqual(1, listener.testmessage_callbacks);
    }

    [TestMethod]
    public void Detach_Repeated()
    {
      hub.Attach(listener);
      hub.Send(new TestMessage());
      hub.Detach(listener);
      hub.Detach(listener);
      hub.Detach(listener);
      hub.Send(new TestMessage());

      Assert.AreEqual(1, listener.testmessage_callbacks);
    }

    [TestMethod]
    public void Detach_BeforeAttach()
    {
      hub.Detach(listener);
      hub.Send(new TestMessage());

      Assert.AreEqual(0, listener.testmessage_callbacks);
    }

    [TestMethod]
    public void Send_RoutesInheritedTypesToInheritedTypeOnly()
    {
      hub.Attach(listener);
      hub.Send(new TestMessage());
      hub.Send(new TestMessageDerived());

      Assert.IsTrue(listener.testmessage_callbacks == 1 && listener.testmessage_derived_callbacks == 1);
    }

    [TestMethod]
    public void Send_SendsComponentMessages()
    {
      hub.Attach(listener);
      hub.Send(new TestMessage());
      hub.Send(new ComponentMessage());

      Assert.IsTrue(listener.testmessage_callbacks == 2 && listener.componentmessage_callbacks == 1);
    }

    [TestMethod]
    public void Attach_AttachesMultipleInstancesOfSameType()
    {
      var second_listener = new TestListener();
      hub.Attach(listener);
      hub.Attach(second_listener);
      hub.Send(new TestMessage());

      Assert.AreEqual(1, listener.testmessage_callbacks);
      Assert.AreEqual(1, second_listener.testmessage_callbacks);
    }

    [TestMethod]
    [ExpectedException(typeof(CannotRegisterType))]
    public void Attach_ThrowsExceptionWhenCannotRegisterType()
    {
      hub.Attach(new UnregisterableListener());
    }

    [TestMethod]
    [Timeout(60000)]
    [Ignore]
    public void StressTest_Basic()
    {
      uint count = 1000000;
      hub.Attach(listener);

      for (uint i = 0; i < count; ++i)
        hub.Send(new TestMessage());

      Assert.AreEqual(count, listener.testmessage_callbacks);
    }

    [TestMethod]
    [Ignore]
    [Timeout(60000)]
    public void StressTest_Component()
    {
      uint count = 1000000;
      hub.Attach(listener);

      for (uint i = 0; i < count; ++i)
      {
        hub.Send(new ComponentMessage());
      }

      Assert.AreEqual(count, listener.testmessage_callbacks);
    }

    [TestMethod]
    [Ignore]
    [Timeout(60000)]
    public void StressTest_Derived()
    {
      uint count = 1000000;
      hub.Attach(listener);

      for (uint i = 0; i < count; ++i)
        hub.Send(new TestMessageDerived());

      Assert.AreEqual(count, listener.testmessage_derived_callbacks);
    }

    [TestMethod]
    public void Attach_CanAttachHubToAnotherHub()
    {
      var otherHub = new MessageHub();
      hub.Attach(otherHub);
      otherHub.Attach(listener);

      hub.Send(new TestMessage());

      Assert.AreEqual(1, listener.testmessage_callbacks);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Attach_ThrowsExceptionWhenCircularRelay()
    {
      var otherHub = new MessageHub();
      hub.Attach(otherHub);
      otherHub.Attach(hub);
    }
  }
}
