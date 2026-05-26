using GameMake.UI.Core.Components;
using GameMake.UI.Yaml;
namespace GameMake.UI.Tests.Yaml;
public class EntityBuilderTests
{
    [Fact] public void 构建单Entity() { Assert.NotNull(EntityBuilder.Build(new List<YamlEntity> { new() { Id = "P1" } }).Find("P1")); }
    [Fact] public void 构建带RectTransform() { Assert.NotNull(EntityBuilder.Build(new List<YamlEntity> { new() { Id = "P1", Components = { new() { Type = "RectTransform", Properties = { ["ZOrder"] = "5" } } } } }).Find("P1").Get<RectTransform>()); }
    [Fact] public void 未知组件类型被忽略() { Assert.NotNull(EntityBuilder.Build(new List<YamlEntity> { new() { Id = "P1", Components = { new() { Type = "NonExistent" } } } }).Find("P1")); }
    [Fact] public void Scene属性从YamlEntity传播() { Assert.Equal("MainMenu", EntityBuilder.Build(new List<YamlEntity> { new() { Id = "P1", Scene = "MainMenu" } }).Find("P1").Scene); }

    [Fact]
    public void Color属性从浮点列表正确转换为0到255范围()
    {
        var yaml = "- Entity: Panel\n  Id: P\n  Components:\n    - Type: PanelRenderer\n      Color:\n        - 0.2\n        - 0.5\n        - 0.2\n        - 1.0\n";
        var entities = YamlDeserializer.Deserialize(yaml);
        var root = EntityBuilder.Build(entities);
        var c = root.Find("P").Get<PanelRenderer>().Color;
        Assert.Equal(51, c.R);
        Assert.Equal(127, c.G);
        Assert.Equal(51, c.B);
        Assert.Equal(255, c.A);
    }
}
