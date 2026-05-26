namespace GameMake.UI.Utilities;
public static class Ease
{
    public static float InQuad(float t) => t * t;
    public static float OutCubic(float t) => 1 - MathF.Pow(1 - t, 3);
    public static float OutBack(float t) { const float c1 = 1.70158f, c3 = c1 + 1; return 1 + c3 * MathF.Pow(t - 1, 3) + c1 * MathF.Pow(t - 1, 2); }
}
