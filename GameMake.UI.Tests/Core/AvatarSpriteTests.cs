using GameMake.UI.Core.Components;
namespace GameMake.UI.Tests.Core;

public class AvatarSpriteTests
{
    [Fact]
    public void 可创建并设置基本属性()
    {
        var avatar = new AvatarSprite();
        avatar.TexturePath = "test.png";
        Assert.Equal("test.png", avatar.TexturePath);
        Assert.Equal(0.3f, avatar.TransitionDuration);
    }

    [Fact]
    public void Crossfade在TexturePath变化时重置进度()
    {
        var avatar = new AvatarSprite();
        avatar.TexturePath = "old.png";
        avatar.Update(0.1f); // stabilize initial state
        avatar.TexturePath = "new.png";
        avatar.Update(0f); // detect change
        Assert.Equal(0f, avatar.TransitionProgress);
    }

    [Fact]
    public void Crossfade随Update推进()
    {
        var avatar = new AvatarSprite();
        avatar.TexturePath = "old.png";
        avatar.Update(0.1f); // stabilize
        avatar.TexturePath = "new.png";
        avatar.Update(0f); // detect change, progress = 0
        avatar.Update(0.15f); // half of 0.3s duration
        Assert.Equal(0.5f, avatar.TransitionProgress, 0.01f);
        avatar.Update(0.3f); // past full duration
        Assert.Equal(1f, avatar.TransitionProgress);
    }
}
