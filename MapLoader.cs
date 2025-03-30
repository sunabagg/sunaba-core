using Godot;

namespace Sunaba.Core;

public class MapLoader
{
    public static Node CreateMapNode()
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
         // Pass the TextureLoader to the func_godot_map instance
         return funcGodotMap;
    }
    
    public static void SetTexturePath(string path, Node funcGodotMap, IoInterface ioInterface)
    {
        funcGodotMap.Call("set_base_texture_dir", path);
        var TextureLoader = new TextureLoader(ioInterface);
        funcGodotMap.Call("set_texture_loader", TextureLoader);
    }

    public static void LoadMap(string path, Node funcGodotMap, IoInterface ioInterface)
    {
        var mapContents = ioInterface.LoadText(path);
        if (string.IsNullOrEmpty(mapContents))
        {
            GD.PrintErr($"Failed to load map contents from path: {path}");
            return; // Handle the error as needed
        }
        funcGodotMap.Call("load_from_string", mapContents); // Call the method to load the map from the string
    }

    public static void AddPostLoadCallback(WattleScript.Interpreter.Closure func, Node node)
    {
        var action = new Action(() =>
        {
            // Call the Lua function after the map has been loaded
            try
            {
                if (func != null)
                {
                    func.Call(node); // Pass the node to the Lua function if needed
                }
            }
            catch (Exception e)
            {
                GD.PrintErr($"Error calling post-load callback: {e.Message}");
            }
        });
        var callable = Callable.From(action);
        node.Connect("build_complete", callable);
    }
}