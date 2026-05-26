using GameMake.UI.Rendering;
namespace GameMake.UI.Tests.Rendering;

public class RenderSystemTests
{
    [Fact]
    public void 构造不抛出异常()
    {
        var exception = Record.Exception(() => new RenderSystem());
        Assert.Null(exception);
    }
}
