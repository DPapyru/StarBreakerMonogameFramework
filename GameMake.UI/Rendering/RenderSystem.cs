using GameMake.UI.Core.Components;
using GameMake.UI.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameMake.UI.Rendering;

public class RenderSystem
{
    readonly Dictionary<Type, Action<UIComponent, Rectangle, SpriteBatch, ResourceManager>> _renderers = new();

    public RenderSystem()
    {
        _renderers[typeof(PanelRenderer)] = (c, r, sb, _) =>
        {
            var p = (PanelRenderer)c;
            sb.Draw(GetPixel(sb), r, p.Color);
        };
        _renderers[typeof(ButtonRenderer)] = (c, r, sb, _) =>
        {
            var b = (ButtonRenderer)c;
            var color = b.State switch
            {
                ButtonState.Hovered => b.HoverColor,
                ButtonState.Pressed => b.PressColor,
                _ => b.NormalColor
            };
            sb.Draw(GetPixel(sb), r, color);
        };
        _renderers[typeof(ProgressRenderer)] = (c, r, sb, _) =>
        {
            var p = (ProgressRenderer)c;
            sb.Draw(GetPixel(sb), r, p.BgColor);
            var fill = new Rectangle(r.X, r.Y, (int)(r.Width * p.Progress), r.Height);
            sb.Draw(GetPixel(sb), fill, p.FillColor);
        };
        _renderers[typeof(ImageRenderer)] = (c, r, sb, res) =>
        {
            var img = (ImageRenderer)c;
            if (!string.IsNullOrEmpty(img.TexturePath))
            {
                var tex = res.GetTexture(img.TexturePath);
                if (tex != null) sb.Draw(tex, r, img.Tint);
            }
        };
        _renderers[typeof(SpriteRenderer)] = (c, r, sb, res) =>
        {
            var sprite = (SpriteRenderer)c;
            if (!string.IsNullOrEmpty(sprite.TexturePath))
            {
                var tex = res.GetTexture(sprite.TexturePath);
                if (tex != null) sb.Draw(tex, r, sprite.Tint);
            }
        };
        _renderers[typeof(TextRenderer)] = (c, r, sb, res) =>
        {
            var t = (TextRenderer)c;
            if (string.IsNullOrEmpty(t.Text) || string.IsNullOrEmpty(t.FontPath)) return;
            var font = res.GetFont(t.FontPath, t.FontSize);
            if (font == null) return;
            var size = font.MeasureString(t.Text);
            var pos = TextLayout.ComputeTextPosition(r, size);
            sb.DrawString(font, t.Text, pos, t.Color);
        };
        _renderers[typeof(DialogueRenderer)] = (c, r, sb, res) =>
        {
            var d = (DialogueRenderer)c;
            var padX = (int)d.Padding.X;
            var padY = (int)d.Padding.Y;
            var inner = new Rectangle(r.X + padX, r.Y + padY, r.Width - padX * 2, r.Height - padY * 2);

            // Background
            if (d.BackgroundColor.A > 0)
                sb.Draw(GetPixel(sb), r, d.BackgroundColor);

            // Avatar (left side)
            var avatarRect = Rectangle.Empty;
            if (d.AvatarTexture != null)
            {
                var avX = inner.X;
                var avY = inner.Y + 40;
                var avW = (int)d.AvatarSize.X;
                var avH = (int)d.AvatarSize.Y;
                avatarRect = new Rectangle(avX, avY, avW, avH);
                sb.Draw(d.AvatarTexture, avatarRect, Color.White);
            }

            // Speaker name
            var font = !string.IsNullOrEmpty(d.Font) ? res.GetFont(d.Font) : null;
            if (font != null && !string.IsNullOrEmpty(d.SpeakerName))
                sb.DrawString(font, d.SpeakerName, new Vector2(inner.X, inner.Y), d.TextColor);

            // Dialogue text (typewriter)
            if (font != null && !string.IsNullOrEmpty(d.DialogueText))
            {
                var textLen = d.DialogueText.Length;
                var displayLen = (int)(textLen * Math.Min(1, Math.Max(0, d.DisplayProgress)));
                if (displayLen > 0)
                {
                    var displayText = d.DialogueText[..displayLen];
                    var textX = avatarRect != Rectangle.Empty ? avatarRect.Right + 8 : inner.X;
                    var textY = inner.Y + 40;
                    sb.DrawString(font, displayText, new Vector2(textX, textY), d.TextColor);
                }
            }

            // Branching options
            if (d.State == DialogueState.Branching && d.Options?.Count > 0)
            {
                var optY = inner.Y + 100;
                for (int i = 0; i < d.Options.Count; i++)
                {
                    var opt = d.Options[i];
                    var optRect = new Rectangle(inner.X + 20, optY + i * 40, inner.Width - 40, 30);
                    sb.Draw(GetPixel(sb), optRect, new Color(0.2f, 0.2f, 0.3f, 1f));
                    if (font != null && !string.IsNullOrEmpty(opt.Text))
                        sb.DrawString(font, opt.Text, new Vector2(optRect.X + 4, optRect.Y + 4), d.TextColor);
                }
            }
        };
        _renderers[typeof(AvatarSprite)] = (c, r, sb, res) =>
        {
            var a = (AvatarSprite)c;
            if (a.CurrentTexture == null) return;

            if (a.TransitionProgress >= 1)
                sb.Draw(a.CurrentTexture, r, Color.White);
            else
            {
                if (a._previousTexture != null)
                {
                    var oldAlpha = 1f - a.TransitionProgress;
                    sb.Draw(a._previousTexture, r, Color.White * oldAlpha);
                }
                sb.Draw(a.CurrentTexture, r, Color.White * a.TransitionProgress);
            }
        };
    }

    static Texture2D _pixel;
    static Texture2D GetPixel(SpriteBatch sb)
    {
        if (_pixel == null)
        {
            _pixel = new Texture2D(sb.GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }
        return _pixel;
    }

    public void Render(UIEntity root, SpriteBatch sb, ResourceManager res)
    {
        var all = new List<(UIEntity, int)>();
        void Walk(UIEntity e, int pz)
        {
            var z = pz + (e.Get<RectTransform>()?.ZOrder ?? 0);
            all.Add((e, z));
            foreach (var c in e.Children) Walk(c, z);
        }
        Walk(root, 0);

        sb.Begin();
        foreach (var (e, _) in all.OrderBy(x => x.Item2))
        {
            var rt = e.Get<RectTransform>();
            if (rt == null || !rt.Visible) continue;
            foreach (var comp in e.Components)
                if (_renderers.TryGetValue(comp.GetType(), out var r))
                    r(comp, rt.Bounds, sb, res);
        }
        sb.End();
    }
}
