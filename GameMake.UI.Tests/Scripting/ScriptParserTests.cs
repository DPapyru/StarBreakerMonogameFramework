using GameMake.UI.Scripting;

namespace GameMake.UI.Tests.Scripting;

public class ScriptParserTests
{
    [Fact]
    public void Parse_SayInstruction()
    {
        var yaml = @"
- Type: Say
  Speaker: Narrator
  Text: Hello, world!
";
        var instructions = ScriptParser.Parse(yaml);
        Assert.Single(instructions);
        var say = instructions[0];
        Assert.Equal(ScriptInstructionType.Say, say.Type);
        Assert.Equal("Narrator", say.Speaker);
        Assert.Equal("Hello, world!", say.Text);
    }

    [Fact]
    public void Parse_BranchWithOptions()
    {
        var yaml = @"
- Type: Branch
  Text: Choose an option
  Options:
    - Text: Option A
      JumpTarget: label_a
    - Text: Option B
      JumpTarget: label_b
";
        var instructions = ScriptParser.Parse(yaml);
        Assert.Single(instructions);
        var branch = instructions[0];
        Assert.Equal(ScriptInstructionType.Branch, branch.Type);
        Assert.Equal("Choose an option", branch.Text);
        Assert.NotNull(branch.Options);
        Assert.Equal(2, branch.Options.Count);
        Assert.Equal("Option A", branch.Options[0].Text);
        Assert.Equal("label_a", branch.Options[0].JumpTarget);
        Assert.Equal("Option B", branch.Options[1].Text);
        Assert.Equal("label_b", branch.Options[1].JumpTarget);
    }

    [Fact]
    public void Parse_Label()
    {
        var yaml = @"
- Type: Label
  Name: start
";
        var instructions = ScriptParser.Parse(yaml);
        Assert.Single(instructions);
        var label = instructions[0];
        Assert.Equal(ScriptInstructionType.Label, label.Type);
        Assert.Equal("start", label.Name);
    }

    [Fact]
    public void Parse_Jump()
    {
        var yaml = @"
- Type: Jump
  Target: end
";
        var instructions = ScriptParser.Parse(yaml);
        Assert.Single(instructions);
        var jump = instructions[0];
        Assert.Equal(ScriptInstructionType.Jump, jump.Type);
        Assert.Equal("end", jump.Target);
    }

    [Fact]
    public void Parse_Callback()
    {
        var yaml = @"
- Type: Callback
  Name: onChoiceMade
";
        var instructions = ScriptParser.Parse(yaml);
        Assert.Single(instructions);
        var cb = instructions[0];
        Assert.Equal(ScriptInstructionType.Callback, cb.Type);
        Assert.Equal("onChoiceMade", cb.CallbackName);
    }

    [Fact]
    public void Parse_SayOptionalFields()
    {
        var yaml = @"
- Type: Say
  Speaker: Narrator
  Text: Hello!
  Expression: happy
  Speed: 1.5
  AutoNext: true
";
        var instructions = ScriptParser.Parse(yaml);
        var say = instructions[0];
        Assert.Equal("happy", say.Expression);
        Assert.Equal(1.5f, say.Speed);
        Assert.True(say.AutoNext);
    }

    [Fact]
    public void Parse_SayOptionalFieldsDefaultNull()
    {
        var yaml = @"
- Type: Say
  Speaker: Narrator
  Text: Hello!
";
        var instructions = ScriptParser.Parse(yaml);
        var say = instructions[0];
        Assert.Null(say.Expression);
        Assert.Null(say.Speed);
        Assert.Null(say.AutoNext);
    }

    [Fact]
    public void Parse_InvalidYamlThrowsScriptParseException()
    {
        var yaml = @"
- Type: UnknownType
  Name: test
";
        var ex = Assert.Throws<ScriptParseException>(() => ScriptParser.Parse(yaml));
        Assert.Contains("Unknown", ex.Message);
    }

    [Fact]
    public void Parse_MissingTypeThrowsScriptParseException()
    {
        var yaml = @"
- Name: test
";
        var ex = Assert.Throws<ScriptParseException>(() => ScriptParser.Parse(yaml));
        Assert.Contains("Type", ex.Message);
    }

    [Fact]
    public void Parse_MultipleInstructions()
    {
        var yaml = @"
- Type: Label
  Name: start
- Type: Say
  Speaker: Narrator
  Text: Beginning!
- Type: Jump
  Target: end
- Type: Label
  Name: end
";
        var instructions = ScriptParser.Parse(yaml);
        Assert.Equal(4, instructions.Count);
        Assert.Equal(ScriptInstructionType.Label, instructions[0].Type);
        Assert.Equal("start", instructions[0].Name);
        Assert.Equal(ScriptInstructionType.Say, instructions[1].Type);
        Assert.Equal("Narrator", instructions[1].Speaker);
        Assert.Equal("Beginning!", instructions[1].Text);
        Assert.Equal(ScriptInstructionType.Jump, instructions[2].Type);
        Assert.Equal("end", instructions[2].Target);
        Assert.Equal(ScriptInstructionType.Label, instructions[3].Type);
        Assert.Equal("end", instructions[3].Name);
    }
}
