using GameMake.UI;

namespace GameMake.UI.Scripting;

public class CompiledScript
{
    public Func<UIEntity, object> OnCreate { get; set; }
    public Action<UIEntity, object, float> OnUpdate { get; set; }
    public Action<UIEntity, object> OnClick { get; set; }
    public Action<UIEntity, object> OnHoverStart { get; set; }
    public Action<UIEntity, object> OnHoverEnd { get; set; }
    public Action<UIEntity, object> OnDestroy { get; set; }
}
