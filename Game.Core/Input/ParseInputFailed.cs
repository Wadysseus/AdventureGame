using Game.Core.Messaging;

namespace AdventureGame.Input
{
  public class ParseInputFailed : Message
  {
    public string Message { get; private set; }

    public ParseInputFailed(string failedCommandWord)
    {
      Message = $"{failedCommandWord} is not a valid command.";
    }
  }
}
