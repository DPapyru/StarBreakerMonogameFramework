namespace GameMake.UI.Scripting;

public class ScriptParseException : Exception
{
    public ScriptParseException(string message) : base(message) { }
    public ScriptParseException(string message, Exception inner) : base(message, inner) { }
}
