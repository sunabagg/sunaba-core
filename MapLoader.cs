using Godot;

namespace Sunaba.Core;

public static class MapLoader
{
    public static Node LoadMap(string path, Node node, IoInterface ioInterface)
    {
        var funcGodotMapScript = GD.Load<GDScript>("res://addons/func_godot/src/map/func_godot_map.gd");
        if (funcGodotMapScript == null)
        {
            GD.PrintErr("Failed to load func_godot_map.gd script.");
            return null;
        }
        var funcGodotMap = (Node)funcGodotMapScript.New();
        if (funcGodotMap == null)
        {
            GD.PrintErr("Failed to create instance of func_godot_map.");
            return null;
        }
        node.AddChild(funcGodotMap); // Add the func_godot_map instance to the scene tree
        var TextureLoader = new TextureLoader(ioInterface);
        funcGodotMap.Call("set_texture_loader", TextureLoader); // Pass the TextureLoader to the func_godot_map instance
        var mapContents = ioInterface.LoadText(path);
        if (string.IsNullOrEmpty(mapContents))
        {
            GD.PrintErr($"Failed to load map contents from path: {path}");
            funcGodotMap.QueueFree();
            return null;
        }
        funcGodotMap.Call("load_from_string", mapContents); // Call the method to load the map from the string
        return funcGodotMap;
    }
}