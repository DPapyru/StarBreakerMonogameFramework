using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Framework.Content.Pipeline.Builder;

var builder = new Builder();
builder.Run(args);
return builder.FailedToBuild > 0 ? -1 : 0;

public class Builder : ContentBuilder
{
    public override IContentCollection GetContentCollection()
    {
        var cc = new ContentCollection();
        cc.Include<WildcardRule>("*.spritefont");
        return cc;
    }
}
