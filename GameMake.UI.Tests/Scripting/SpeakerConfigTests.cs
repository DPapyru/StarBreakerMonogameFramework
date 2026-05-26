using GameMake.UI.Scripting;
namespace GameMake.UI.Tests.Scripting;

public class SpeakerConfigTests
{
    [Fact]
    public void 解析YAML字符串返回正确的SpeakerDef()
    {
        var yaml = @"
speakers:
  hero:
    display_name: ""Hero""
    color: [1.0, 0.5, 0.0, 1.0]
    expressions:
      normal: ""textures/hero_normal.png""
      happy: ""textures/hero_happy.png""
  npc:
    display_name: ""NPC""
    expressions:
      default: ""textures/npc_default.png""
";
        var result = SpeakerConfig.Parse(yaml);
        Assert.Equal(2, result.Count);
        Assert.True(result.ContainsKey("hero"));
        Assert.Equal("Hero", result["hero"].DisplayName);
        Assert.NotNull(result["hero"].Color);
        Assert.Equal(2, result["hero"].Expressions.Count);
        Assert.Equal("textures/hero_happy.png", result["hero"].Expressions["happy"]);
        Assert.True(result.ContainsKey("npc"));
        Assert.Equal("NPC", result["npc"].DisplayName);
        Assert.Null(result["npc"].Color);
        Assert.Single(result["npc"].Expressions);
    }

    [Fact]
    public void 缺失文件返回空字典()
    {
        var result = SpeakerConfig.Load("/nonexistent/path.yaml");
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
