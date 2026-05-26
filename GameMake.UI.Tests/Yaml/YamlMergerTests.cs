using GameMake.UI.Yaml;
namespace GameMake.UI.Tests.Yaml;
public class YamlMergerTests
{
    [Fact] public void 新ID追加() { Assert.Equal(2, YamlMerger.Merge(new() { new() { Id = "A" } }, new() { new() { Id = "B" } }).Count); }
    [Fact] public void 同ID覆盖组件属性() { var r = YamlMerger.Merge(new() { new() { Id = "A", Components = { new() { Type = "P", Properties = { ["C"] = "old" } } } } }, new() { new() { Id = "A", Components = { new() { Type = "P", Properties = { ["C"] = "new" } } } } }); Assert.Equal("new", r[0].Components[0].Properties["C"]); }
    [Fact] public void 同ID新增组件() { var r = YamlMerger.Merge(new() { new() { Id = "A", Components = { new() { Type = "P" } } } }, new() { new() { Id = "A", Components = { new() { Type = "S", Properties = { ["P"] = "x.cs" } } } } }); Assert.Equal(2, r[0].Components.Count); }
    [Fact] public void 子节点递归合并() { var r = YamlMerger.Merge(new() { new() { Id = "A", Children = { new() { Id = "B", Components = { new() { Type = "T" } } } } } }, new() { new() { Id = "A", Children = { new() { Id = "B", Components = { new() { Type = "S" } } } } } }); Assert.Equal(2, r[0].Children[0].Components.Count); }
}
