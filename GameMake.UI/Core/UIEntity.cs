namespace GameMake.UI;

public class UIEntity
{
    public string Id { get; set; }
    public UIEntity Parent { get; set; }
    public List<UIEntity> Children { get; set; } = new();
    public List<UIComponent> Components { get; set; } = new();
    public bool IsModal { get; set; }
    public string Scene { get; set; }

    public T Get<T>() where T : UIComponent
        => Components.OfType<T>().FirstOrDefault();

    public void Add<T>(T component) where T : UIComponent
        => Components.Add(component);

    public void Remove<T>() where T : UIComponent
        => Components.RemoveAll(c => c is T);

    public UIEntity Find(string id)
    {
        if (Id == id) return this;
        foreach (var child in Children)
            if (child.Find(id) is { } found) return found;
        return null;
    }

    public void AddChild(UIEntity child)
    {
        child.Parent = this;
        Children.Add(child);
    }

    public void Destroy()
    {
        foreach (var sc in Components.OfType<Core.Components.ScriptComponent>())
            sc.Script?.OnDestroy?.Invoke(sc.Owner ?? this, sc.Context);
        Parent?.Children.Remove(this);
    }
}
