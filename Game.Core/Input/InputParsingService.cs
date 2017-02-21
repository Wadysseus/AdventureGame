using Game.Core.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventureGame.Input
{
  public interface IInputParsingService
  {
    void Parse(string input);
    void RegisterCommand(string[] commandWords, Action<string[]> callback);
    IEnumerable<string> GetCommandWords();
  }

  public class InputParsingService : IInputParsingService
  {
    private Dictionary<string, Action<string[]>> Actions = new Dictionary<string, Action<string[]>>();
    private static char[] separatingCharacters = { ' ' };
    private IMessageHub _hub;

    public InputParsingService(IMessageHub hub)
    {
      _hub = hub;
    }

    public IEnumerable<string> GetCommandWords()
    {
      return Actions.Keys;
    }

    public void Parse(string input)
    {
      string[] inputTokens = input.Split(separatingCharacters);
      var currentAction = Actions[inputTokens.First()];
      currentAction(inputTokens.Skip(1).ToArray());
    }

    public void RegisterCommand(string[] commandWords, Action<string[]> callback)
    {
      if (IsAnyWordAlreadyRegistered(commandWords))
        throw new ArgumentException("Duplicate commandWord already registered in Actions.");

      foreach(var word in commandWords)
        Actions.Add(word, callback);
    }

    private bool IsAnyWordAlreadyRegistered(string[] commandWords)
    {
      return GetCommandWords().Any(word => commandWords.Any(x => x == word));
    }
  }
}
