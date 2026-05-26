namespace GameMake.UI.Scripting;

public enum ScriptInstructionType
{
    Say,
    Branch,
    Label,
    Jump,
    Callback
}

public class BranchOption
{
    public string Text { get; set; }
    public string JumpTarget { get; set; }
}

public class ScriptInstruction
{
    public ScriptInstructionType Type { get; set; }

    // Say
    public string Speaker { get; set; }
    public string Text { get; set; }
    public string Expression { get; set; }
    public float? Speed { get; set; }
    public bool? AutoNext { get; set; }

    // Branch
    public List<BranchOption> Options { get; set; }

    // Label / Jump
    public string Name { get; set; }
    public string Target { get; set; }

    // Callback
    public string CallbackName { get; set; }
}
