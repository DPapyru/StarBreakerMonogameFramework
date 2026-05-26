namespace GameMake.UI.Yaml;
public class YamlEntity { public string Id { get; set; } public string Scene { get; set; } public bool? Modal { get; set; } public List<YamlComponent> Components { get; set; } = new(); public List<YamlEntity> Children { get; set; } = new(); }
