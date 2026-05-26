using GameMake.UI.Yaml;
namespace GameMake.UI.Tests.Yaml;
public class YamlDeserializerTests
{
    [Fact] public void 单个Entity含组件() { var y = "- Entity: Panel\n  Id: M\n  Components:\n    - Type: PanelRenderer\n"; var r = YamlDeserializer.Deserialize(y); Assert.Single(r); Assert.Equal("PanelRenderer", r[0].Components[0].Type); }
    [Fact] public void Entity含子节点() { var y = "- Entity: Panel\n  Id: R\n  Children:\n    - Entity: Button\n      Id: B\n"; var r = YamlDeserializer.Deserialize(y); Assert.Equal("B", r[0].Children[0].Id); }
    [Fact] public void 空YAML返回空列表() { Assert.Empty(YamlDeserializer.Deserialize("")); }
}
