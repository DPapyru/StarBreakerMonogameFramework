using GameMake.UI.Mods;
namespace GameMake.UI.Tests.Mods;
public class ModManagerTests
{
    [Fact] public void 空目录返回空() { var d = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()); Directory.CreateDirectory(d); try { Assert.Empty(ModManager.DiscoverMods(d)); } finally { Directory.Delete(d); } }
    [Fact] public void 发现Mod() { var d = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()); Directory.CreateDirectory(Path.Combine(d, "MyMod")); try { Assert.Equal("MyMod", ModManager.DiscoverMods(d)[0].Name); } finally { Directory.Delete(d, true); } }
}
