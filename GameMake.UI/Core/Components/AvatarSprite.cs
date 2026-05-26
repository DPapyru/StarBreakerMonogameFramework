using Microsoft.Xna.Framework.Graphics;
namespace GameMake.UI.Core.Components;

public class AvatarSprite : UIComponent
{
    public string TexturePath { get; set; }
    public float TransitionDuration { get; set; } = 0.3f;
    public Texture2D CurrentTexture { get; set; }
    public float TransitionProgress { get; set; } = 1f;

    internal Texture2D _previousTexture;
    string _lastTexturePath;

    public void Update(float dt)
    {
        if (TexturePath != _lastTexturePath)
        {
            _previousTexture = CurrentTexture;
            _lastTexturePath = TexturePath;
            TransitionProgress = 0;
        }
        else if (TransitionProgress < 1)
        {
            TransitionProgress = Math.Min(1, TransitionProgress + dt / TransitionDuration);
            if (TransitionProgress >= 1)
                _previousTexture = null;
        }
    }
}
