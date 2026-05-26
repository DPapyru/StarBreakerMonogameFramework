using GameMake.UI.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameMake.UI.Debug;

public class DebugOverlay
{
    public bool Visible { get; set; }
    Texture2D _pixel;

    public void LoadContent(GraphicsDevice gd)
    {
        _pixel = new Texture2D(gd, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public void Draw(SpriteBatch sb, UIEntity root)
    {
        if (!Visible) return;
        var all = new List<UIEntity>();
        void Walk(UIEntity e) { all.Add(e); foreach (var c in e.Children) Walk(c); }
        Walk(root);

        sb.Begin();
        foreach (var e in all)
        {
            var rt = e.Get<RectTransform>();
            if (rt == null) continue;
            var r = rt.Bounds;
            sb.Draw(_pixel, new Rectangle(r.X, r.Y, r.Width, 1), Color.Green);
            sb.Draw(_pixel, new Rectangle(r.X, r.Bottom, r.Width, 1), Color.Green);
            sb.Draw(_pixel, new Rectangle(r.X, r.Y, 1, r.Height), Color.Green);
            sb.Draw(_pixel, new Rectangle(r.Right, r.Y, 1, r.Height), Color.Green);
        }
        sb.End();
    }
}
