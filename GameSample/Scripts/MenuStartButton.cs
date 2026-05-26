using GameMake.UI;

public class MenuStartButton
{
    public static object OnCreate(UIEntity self) => null;

    public static void OnClick(UIEntity self, object ctx)
    {
        UISystem.Current.SwitchScene("GameHUD");
    }
}
