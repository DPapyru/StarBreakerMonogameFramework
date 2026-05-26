using GameMake.UI.Utilities;
namespace GameMake.UI.Tests.Utilities;
public class EaseTests
{
    [Fact] public void InQuad_起点0() => Assert.Equal(0.0, (double)Ease.InQuad(0f));
    [Fact] public void InQuad_终点1() => Assert.Equal(1.0, (double)Ease.InQuad(1f), 6);
    [Fact] public void OutCubic_起点0() => Assert.Equal(0.0, (double)Ease.OutCubic(0f));
    [Fact] public void OutCubic_终点1() => Assert.Equal(1.0, (double)Ease.OutCubic(1f), 6);
    [Fact] public void OutBack_起点0() => Assert.Equal(0.0, (double)Ease.OutBack(0f), 6);
    [Fact] public void OutBack_终点1() => Assert.Equal(1.0, (double)Ease.OutBack(1f), 6);
}
