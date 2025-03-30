using System.Text.Json;
using Godot;

namespace Sunaba.Core;

public class EntityClass
{
    public string ClassName = "";
    public string Description = "";
    
    public List<EntityClass> BaseClasses = new();
    public Dictionary<string, object> ClassProperties = new();
    public Dictionary<string, string> ClassPropertiesDescription = new();
    public Dictionary<string, object> MetaProperties = new()
    {
        { "size", new Aabb(new Vector3(-8, -8, -8), new Vector3(8, 8, 8)) },
        { "color", new Color(0.8f, 0.8f, 0.8f) }
    };
    
    public string NodeClass = "";
    public string NameProperty = "";
    
    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }
}