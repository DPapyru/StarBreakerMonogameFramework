using GameMake.UI;

public class ReturnButton
{
    public static object OnCreate(UIEntity self) => null;

    public static void OnClick(UIEntity self, object ctx)
    {
        UISystem.Current.SwitchScene("MainMenu");
    }
}
