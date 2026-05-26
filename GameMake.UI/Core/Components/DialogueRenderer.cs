using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace GameMake.UI.Core.Components;

public enum DialogueState { Idle, Typing, WaitingForClick, Branching }

public class BranchOption
{
    public string Text { get; set; }
    public string JumpTarget { get; set; }
}

public class DialogueRenderer : UIComponent
{
    public string SpeakerName { get; set; }
    public string DialogueText { get; set; }
    public Texture2D AvatarTexture { get; set; }
    public float TextSpeed { get; set; } = 30;
    public float DisplayProgress { get; set; }
    public List<BranchOption> Options { get; set; } = new();
    public DialogueState State { get; set; } = DialogueState.Idle;

    // Styling properties
    public string Font { get; set; }
    public Color TextColor { get; set; } = Color.White;
    public Color BackgroundColor { get; set; } = Color.Transparent;
    public Vector2 Padding { get; set; } = Vector2.Zero;
    public Vector2 AvatarSize { get; set; } = new(64, 64);

    public void FastForward() { DisplayProgress = 1; }

    public void Reset()
    {
        DisplayProgress = 0;
        State = DialogueState.Idle;
    }

    public void Update(float dt)
    {
        if (State == DialogueState.Typing && DisplayProgress < 1)
        {
            var len = DialogueText?.Length ?? 0;
            if (len > 0)
                DisplayProgress = Math.Min(1, DisplayProgress + dt * TextSpeed / len);
            else
                DisplayProgress = 1;

            if (DisplayProgress >= 1)
                State = DialogueState.WaitingForClick;
        }
    }
}
