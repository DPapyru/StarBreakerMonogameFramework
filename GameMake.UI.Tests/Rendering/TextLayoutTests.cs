using GameMake.UI.Rendering;
using Microsoft.Xna.Framework;

namespace GameMake.UI.Tests.Rendering;

public class TextLayoutTests
{
    [Fact]
    public void ComputeTextPosition_居中于父容器()
    {
        var bounds = new Rectangle(100, 200, 400, 300);
        var textSize = new Vector2(100, 30);
        var result = TextLayout.ComputeTextPosition(bounds, textSize);
        Assert.Equal(250, result.X);  // 100 + (400-100)/2
        Assert.Equal(335, result.Y);  // 200 + (300-30)/2
    }

    [Fact]
    public void ComputeTextPosition_文字大于父容器_不限制负偏移()
    {
        var bounds = new Rectangle(10, 20, 50, 30);
        var textSize = new Vector2(100, 50);
        var result = TextLayout.ComputeTextPosition(bounds, textSize);
        Assert.Equal(-15, result.X);  // 10 + (50-100)/2 = -15
        Assert.Equal(10, result.Y);   // 20 + (30-50)/2 = 10
    }

    [Fact]
    public void ComputeTextPosition_零尺寸边界()
    {
        var bounds = new Rectangle(0, 0, 0, 0);
        var textSize = new Vector2(10, 5);
        var result = TextLayout.ComputeTextPosition(bounds, textSize);
        Assert.Equal(-5, result.X);  // 0 + (0-10)/2
        Assert.Equal(-2, result.Y);  // 0 + (0-5)/2
    }

    [Fact]
    public void ComputeTextPosition_零尺寸文字()
    {
        var bounds = new Rectangle(0, 0, 800, 600);
        var textSize = Vector2.Zero;
        var result = TextLayout.ComputeTextPosition(bounds, textSize);
        Assert.Equal(400, result.X);  // 0 + (800-0)/2
        Assert.Equal(300, result.Y);  // 0 + (600-0)/2
    }
}
