namespace GameMake.UI.Yaml;
public static class YamlMerger
{
    public static List<YamlEntity> Merge(List<YamlEntity> base_, List<YamlEntity> overlay)
    {
        var result = new List<YamlEntity>(base_);
        foreach (var o in overlay)
        {
            var existing = result.FirstOrDefault(e => e.Id == o.Id);
            if (existing != null)
                MergeOne(existing, o);
            else
                result.Add(o);
        }
        return result;
    }

    static void MergeOne(YamlEntity target, YamlEntity source)
    {
        foreach (var sc in source.Components)
        {
            var existing = target.Components.FirstOrDefault(c => c.Type == sc.Type);
            if (existing != null)
                foreach (var kv in sc.Properties)
                    existing.Properties[kv.Key] = kv.Value;
            else
                target.Components.Add(sc);
        }

        foreach (var sc in source.Children)
        {
            var existing = target.Children.FirstOrDefault(c => c.Id == sc.Id);
            if (existing != null)
                MergeOne(existing, sc);
            else
                target.Children.Add(sc);
        }
    }
}
