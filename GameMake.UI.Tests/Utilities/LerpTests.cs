using GameMake.UI.Utilities;
using Microsoft.Xna.Framework;

namespace GameMake.UI.Tests.Utilities;

public class LerpTests
{
    [Theory]
    [InlineData(0f, 10f, 0f, 0f)]
    [InlineData(0f, 10f, 0.5f, 5f)]
    [InlineData(0f, 10f, 1f, 10f)]
    public void Float_线性插值(float f, float t, float p, float e)
    {
        Assert.Equal(e, Lerp.Float(f, t, p), 4f);
    }

    [Fact]
    public void Vec2_插值()
    {
        var r = Lerp.Vec2(new Vector2(0, 10), new Vector2(10, 20), 0.5f);
        Assert.Equal(new Vector2(5, 15), r);
    }

    [Fact]
    public void Color_插值()
    {
        var r = Lerp.Color(new Color(0, 0, 0, 0), new Color(255, 255, 255, 255), 0.5f);
        Assert.Equal(new Color(127, 127, 127, 127), r);
    }
}
