using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GameMake.UI.Scripting;

public static class ScriptParser
{
    private static readonly IDeserializer _deserializer = new DeserializerBuilder()
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .Build();

    public static List<ScriptInstruction> Parse(string yamlContent)
    {
        try
        {
            var raw = _deserializer.Deserialize<List<Dictionary<string, object>>>(yamlContent);
            if (raw == null)
                return new List<ScriptInstruction>();

            var instructions = new List<ScriptInstruction>();
            foreach (var entry in raw)
            {
                if (!entry.TryGetValue("Type", out var typeObj) || typeObj?.ToString() == null)
                    throw new ScriptParseException("Each instruction must have a 'Type' field.");

                var typeStr = typeObj.ToString();
                if (!Enum.TryParse<ScriptInstructionType>(typeStr, true, out var type))
                    throw new ScriptParseException($"Unknown instruction type: '{typeStr}'.");

                var inst = new ScriptInstruction { Type = type };

                switch (type)
                {
                    case ScriptInstructionType.Say:
                        inst.Speaker = GetString(entry, "Speaker");
                        inst.Text = GetString(entry, "Text");
                        if (inst.Speaker == null) throw new ScriptParseException("Say instruction requires 'Speaker'.");
                        if (inst.Text == null) throw new ScriptParseException("Say instruction requires 'Text'.");
                        inst.Expression = GetString(entry, "Expression");
                        inst.Speed = GetFloat(entry, "Speed");
                        inst.AutoNext = GetBool(entry, "AutoNext");
                        break;

                    case ScriptInstructionType.Branch:
                        inst.Text = GetString(entry, "Text");
                        inst.Options = ParseOptions(entry);
                        break;

                    case ScriptInstructionType.Label:
                        inst.Name = GetString(entry, "Name");
                        if (inst.Name == null) throw new ScriptParseException("Label instruction requires 'Name'.");
                        break;

                    case ScriptInstructionType.Jump:
                        inst.Target = GetString(entry, "Target");
                        if (inst.Target == null) throw new ScriptParseException("Jump instruction requires 'Target'.");
                        break;

                    case ScriptInstructionType.Callback:
                        inst.CallbackName = GetString(entry, "Name");
                        if (inst.CallbackName == null) throw new ScriptParseException("Callback instruction requires 'Name'.");
                        break;
                }

                instructions.Add(inst);
            }

            return instructions;
        }
        catch (ScriptParseException) { throw; }
        catch (Exception ex)
        {
            throw new ScriptParseException("Failed to parse script: " + ex.Message, ex);
        }
    }

    private static List<BranchOption> ParseOptions(Dictionary<string, object> entry)
    {
        var result = new List<BranchOption>();
        if (!entry.TryGetValue("Options", out var optionsObj) || optionsObj is not List<object> optionsList)
            throw new ScriptParseException("Branch instruction requires 'Options' list.");

        foreach (var item in optionsList)
        {
            if (item is not Dictionary<object, object> optDict)
                throw new ScriptParseException("Each option must be a mapping.");

            var opt = new BranchOption();
            foreach (var kv in optDict)
            {
                var key = kv.Key?.ToString();
                if (string.Equals(key, "Text", StringComparison.OrdinalIgnoreCase))
                    opt.Text = kv.Value?.ToString();
                else if (string.Equals(key, "JumpTarget", StringComparison.OrdinalIgnoreCase))
                    opt.JumpTarget = kv.Value?.ToString();
            }
            result.Add(opt);
        }

        return result;
    }

    private static string GetString(Dictionary<string, object> dict, string key)
    {
        return dict.TryGetValue(key, out var v) ? v?.ToString() : null;
    }

    private static float? GetFloat(Dictionary<string, object> dict, string key)
    {
        if (dict.TryGetValue(key, out var v) && v != null)
        {
            if (v is float f) return f;
            if (v is double d) return (float)d;
            if (float.TryParse(v.ToString(), out var parsed)) return parsed;
        }
        return null;
    }

    private static bool? GetBool(Dictionary<string, object> dict, string key)
    {
        if (dict.TryGetValue(key, out var v) && v != null)
        {
            if (v is bool b) return b;
            if (bool.TryParse(v.ToString(), out var parsed)) return parsed;
        }
        return null;
    }
}
