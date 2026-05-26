using GameMake.UI;
using GameMake.UI.Core.Components;
using GameMake.UI.Debug;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameSample;

public class Game1 : Game
{
    GraphicsDeviceManager _g;
    SpriteBatch _sb;
    UISystem _ui;

    public Game1() { _g = new GraphicsDeviceManager(this); IsMouseVisible = true; }

    protected override void LoadContent()
    {
        _sb = new SpriteBatch(GraphicsDevice);
        _ui = new UISystem(GraphicsDevice);
        _ui.LoadContent(GraphicsDevice);
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;

        UIDebug.Enabled = true;

        Content.RootDirectory = Path.Combine(baseDir, "Content");
        _ui.Content = Content;
        _ui.LoadUI(baseDir, Path.Combine(baseDir, "UILayout"), Path.Combine(baseDir, "Mods"));

        var vp = GraphicsDevice.Viewport;
        Console.WriteLine($"[GameSample] Viewport: {vp.Width}x{vp.Height}, LoadUI done");
    }

    protected override void Update(GameTime gt)
    {
        _ui.Update((float)gt.ElapsedGameTime.TotalSeconds, Mouse.GetState(), Keyboard.GetState());
    }

    protected override void Draw(GameTime gt)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _ui.Draw(_sb);
    }
}
