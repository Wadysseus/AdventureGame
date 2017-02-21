using AdventureGame.Input;
using Game.Core.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;

namespace Game.Core.Test.Input
{
  [TestClass]
  public class InputParsingServiceTests
  {
    private Mock<IMessageHub> _mockMessageHub;
    private IInputParsingService _service;

    private int _callbackCount;
    private string[] _testCommandWords = { "a", "b", "barf" };
    private void TestAction(string[] actionWords)
    {
      ++_callbackCount;
    }

    [TestInitialize]
    public void Init()
    {
      _mockMessageHub = new Mock<IMessageHub>();
      _service = new InputParsingService(_mockMessageHub.Object);
      _callbackCount = 0;
    }

    [TestMethod]
    public void RegisterCommand_RegistersAllCommandWords()
    {
      //Arrange
      //Act
      _service.RegisterCommand(_testCommandWords, TestAction);
      //Assert
      var result = _service.GetCommandWords();

      var matchCount = result
        .Where(word => _testCommandWords.Any(x => x == word))
        .Count();

      Assert.AreEqual(_testCommandWords.Count(), matchCount);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void RegisterCommand_CannotRegisterCommandWordMoreThanOnce()
    {
      //Arrange
      string[] _duplicatedWord = { _testCommandWords.First() };

      //Act
      _service.RegisterCommand(_testCommandWords, TestAction);
      _service.RegisterCommand(_duplicatedWord, TestAction);
    }

    [TestMethod]
    public void Parse_ExecutesActionForAllCommandWords()
    {
      _service.RegisterCommand(_testCommandWords, TestAction);

      foreach (var word in _testCommandWords)
        _service.Parse(word);

      Assert.AreEqual(_callbackCount, _testCommandWords.Count());
    }

    [TestMethod]
    public void Parse_WhenNoMatchingCommandWord_SendsParseInputFailedMessage()
    {
      _service.Parse("nomatch");
      _mockMessageHub.Verify(x => x.Send(It.IsAny<ParseInputFailed>()));
    }
  }
}
