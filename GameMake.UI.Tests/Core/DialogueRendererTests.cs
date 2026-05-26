using GameMake.UI.Core.Components;
namespace GameMake.UI.Tests.Core;

public class DialogueRendererTests
{
    [Fact]
    public void 默认状态为Idle()
    {
        var dr = new DialogueRenderer();
        Assert.Equal(DialogueState.Idle, dr.State);
    }

    [Fact]
    public void FastForward设置DisplayProgress为1()
    {
        var dr = new DialogueRenderer();
        dr.DisplayProgress = 0.5f;
        dr.FastForward();
        Assert.Equal(1, dr.DisplayProgress);
    }

    [Fact]
    public void Reset回到Idle状态()
    {
        var dr = new DialogueRenderer();
        dr.DisplayProgress = 0.5f;
        dr.State = DialogueState.Typing;
        dr.Reset();
        Assert.Equal(0, dr.DisplayProgress);
        Assert.Equal(DialogueState.Idle, dr.State);
    }

    [Fact]
    public void Update在Typing状态推进DisplayProgress()
    {
        var dr = new DialogueRenderer();
        dr.State = DialogueState.Typing;
        dr.DialogueText = "Hello World";
        dr.TextSpeed = 50;
        dr.DisplayProgress = 0;
        dr.Update(0.5f);
        Assert.True(dr.DisplayProgress > 0);
    }

    [Fact]
    public void Update在进度达到1后转换到WaitingForClick()
    {
        var dr = new DialogueRenderer();
        dr.State = DialogueState.Typing;
        dr.DialogueText = "Hello";
        dr.TextSpeed = 100;
        dr.DisplayProgress = 0.999f;
        dr.Update(1f);
        Assert.Equal(1, dr.DisplayProgress);
        Assert.Equal(DialogueState.WaitingForClick, dr.State);
    }
}
