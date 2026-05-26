using GameMake.UI.Scripting;
namespace GameMake.UI.Core.Components;
public class ScriptComponent : UIComponent
{
    public string Path { get; set; }
    public string ClassName { get; set; }
    public int Order { get; set; }
    public object Context { get; set; }
    internal CompiledScript Script { get; set; }
    internal UIEntity Owner { get; set; }
    public UIEntity GetEntity() => Owner;
}
