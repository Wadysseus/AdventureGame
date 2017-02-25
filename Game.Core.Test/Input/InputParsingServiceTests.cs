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
    private string[] _argsPassed;
    private void TestAction(string[] actionWords)
    {
      ++_callbackCount;
      _argsPassed = actionWords;
    }

    [TestInitialize]
    public void Init()
    {
      _mockMessageHub = new Mock<IMessageHub>();
      _service = new InputParsingService(_mockMessageHub.Object);
      _callbackCount = 0;
      _argsPassed = new string[] { };
    }

    [TestMethod]
    public void RegisterCommand_RegistersAllCommandWords()
    {
      _service.RegisterCommand(_testCommandWords, TestAction);
      var result = _service.GetCommandWords();

      var matchCount = result
        .Where(word => _testCommandWords.Any(x => x == word))
        .Count();

      Assert.AreEqual(_testCommandWords.Count(), matchCount);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void RegisterCommand_ThrowsException_WhenWordAlreadyRegistered()
    {
      string[] _duplicatedWord = { _testCommandWords.First() };

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
    public void Parse_SendsParseInputFailedMessage_WhenNoMatchingCommandWord()
    {
        _service.Parse("nomatch");
        _mockMessageHub.Verify(mockHub => mockHub.Send(It.IsAny<ParseInputFailed>()));
    }

    [TestMethod]
    public void Parse_TokenizesAndPassesArgsToAction_WhenMultipleWordInput()
    {
      string secondWord = "second";
      string thirdWord = "third";
      string input = $"{_testCommandWords.First()} {secondWord} {thirdWord}";
      _service.RegisterCommand(_testCommandWords, TestAction);

      _service.Parse(input);
      Assert.IsTrue(_argsPassed.First() == secondWord && _argsPassed.Skip(1).First() == thirdWord);
    }

    [TestMethod]
    public void Parse_DoesNothing_WithNullInput()
    {
      _service.Parse(null);
    }

    [TestMethod]
    public void Parse_DoesNothing_WithEmptyInput()
    {
      _service.Parse(string.Empty);
    }
  }
}
