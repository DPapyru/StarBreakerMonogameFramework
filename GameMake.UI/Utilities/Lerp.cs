using Microsoft.Xna.Framework;
namespace GameMake.UI.Utilities;
public static class Lerp
{
    public static float Float(float f, float t, float p) => f + (t - f) * p;
    public static Vector2 Vec2(Vector2 f, Vector2 t, float p) => new(Float(f.X, t.X, p), Float(f.Y, t.Y, p));
    public static Color Color(Color f, Color t, float p) => new(
        (byte)Float(f.R, t.R, p), (byte)Float(f.G, t.G, p),
        (byte)Float(f.B, t.B, p), (byte)Float(f.A, t.A, p));
}
