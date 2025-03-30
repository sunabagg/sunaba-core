using Godot;
using FileAccess = Godot.FileAccess;

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

    public static void LoadTextures(string path, IoInterface ioInterface)
    {
        var tempTexturePath = "user://temp_texture_cache"; // Example path for caching textures
        
        var pngFiles = ioInterface.GetFileList(path, ".png");
        var jpgFiles = ioInterface.GetFileList(path, ".jpg");
        var jpegFiles = ioInterface.GetFileList(path, ".jpeg");
        var bmpFiles = ioInterface.GetFileList(path, ".bmp");
        var webpFiles = ioInterface.GetFileList(path, ".webp");
        var tgaFiles = ioInterface.GetFileList(path, ".tga");
        var ktxFiles = ioInterface.GetFileList(path, ".ktx");
        var svgFiles = ioInterface.GetFileList(path, ".svg");

        foreach (var pngFilePath in pngFiles)
        {
            var pngBytes = ioInterface.LoadBytes(pngFilePath);
            if (pngBytes != null)
            {
                string[] pathTree = pngFilePath.Replace(ioInterface.PathUrl, "").Split("/");

                string directory = tempTexturePath;
                foreach (var pathName in pathTree)
                {
                    if (!pathName.EndsWith(".png"))
                    {
                        directory += "/" + pathName; // Build the directory path
                        if (!DirAccess.DirExistsAbsolute(directory))
                            DirAccess.MakeDirAbsolute(directory);
                    }
                }
                
                FileAccess fileAccess = FileAccess.Open(directory + "/" + pngFilePath.Replace(ioInterface.PathUrl, ""), FileAccess.ModeFlags.Write);
                if (fileAccess != null)
                {
                    // Write the bytes to the file
                    fileAccess.StoreBuffer(pngBytes);
                    fileAccess.Close(); // Close the file after writing
                }
                else
                {
                    GD.PrintErr($"Failed to open file for writing: {directory + "/" + pngFilePath.Replace(ioInterface.PathUrl, "")}");
                }
            }
        }
        
        foreach (var jpgFilePath in jpgFiles)
        {
            var jpgBytes = ioInterface.LoadBytes(jpgFilePath);
            if (jpgBytes != null)
            {
                string[] pathTree = jpgFilePath.Replace(ioInterface.PathUrl, "").Split("/");
                
                string directory = tempTexturePath;
                foreach (var pathName in pathTree)
                {
                    if (!pathName.EndsWith(".jpg"))
                    {
                        directory += "/" + pathName; // Build the directory path
                        if (!DirAccess.DirExistsAbsolute(directory))
                            DirAccess.MakeDirAbsolute(directory);
                    }
                }
                
                FileAccess fileAccess = FileAccess.Open(directory + "/" + jpgFilePath.Replace(ioInterface.PathUrl, ""), FileAccess.ModeFlags.Write);
                if (fileAccess != null)
                {
                    // Write the bytes to the file
                    fileAccess.StoreBuffer(jpgBytes);
                    fileAccess.Close(); // Close the file after writing
                }
                else
                {
                    GD.PrintErr($"Failed to open file for writing: {directory + "/" + jpgFilePath.Replace(ioInterface.PathUrl, "")}");
                }
            }
        }
        
        foreach (var jpegFilePath in jpegFiles)
        {
            var jpegBytes = ioInterface.LoadBytes(jpegFilePath);
            if (jpegBytes != null)
            {
                string[] pathTree = jpegFilePath.Replace(ioInterface.PathUrl, "").Split("/");
                
                string directory = tempTexturePath;
                foreach (var pathName in pathTree)
                {
                    if (!pathName.EndsWith(".jpeg"))
                    {
                        directory += "/" + pathName; // Build the directory path
                        if (!DirAccess.DirExistsAbsolute(directory))
                            DirAccess.MakeDirAbsolute(directory);
                    }
                }
                
                FileAccess fileAccess = FileAccess.Open(directory + "/" + jpegFilePath.Replace(ioInterface.PathUrl, ""), FileAccess.ModeFlags.Write);
                if (fileAccess != null)
                {
                    // Write the bytes to the file
                    fileAccess.StoreBuffer(jpegBytes);
                    fileAccess.Close(); // Close the file after writing
                }
                else
                {
                    GD.PrintErr($"Failed to open file for writing: {directory + "/" + jpegFilePath.Replace(ioInterface.PathUrl, "")}");
                }
            }
        }
        
        foreach (var bmpFilePath in bmpFiles)
        {
            var bmpBytes = ioInterface.LoadBytes(bmpFilePath);
            if (bmpBytes != null)
            {
                string[] pathTree = bmpFilePath.Replace(ioInterface.PathUrl, "").Split("/");
                
                string directory = tempTexturePath;
                foreach (var pathName in pathTree)
                {
                    if (!pathName.EndsWith(".bmp"))
                    {
                        directory += "/" + pathName; // Build the directory path
                        if (!DirAccess.DirExistsAbsolute(directory))
                            DirAccess.MakeDirAbsolute(directory);
                    }
                }
                
                FileAccess fileAccess = FileAccess.Open(directory + "/" + bmpFilePath.Replace(ioInterface.PathUrl, ""), FileAccess.ModeFlags.Write);
                if (fileAccess != null)
                {
                    // Write the bytes to the file
                    fileAccess.StoreBuffer(bmpBytes);
                    fileAccess.Close(); // Close the file after writing
                }
                else
                {
                    GD.PrintErr($"Failed to open file for writing: {directory + "/" + bmpFilePath.Replace(ioInterface.PathUrl, "")}");
                }
            }
        }
        
        foreach (var webpFilePath in webpFiles)
        {
            var webpBytes = ioInterface.LoadBytes(webpFilePath);
            if (webpBytes != null)
            {
                string[] pathTree = webpFilePath.Replace(ioInterface.PathUrl, "").Split("/");
                
                string directory = tempTexturePath;
                foreach (var pathName in pathTree)
                {
                    if (!pathName.EndsWith(".webp"))
                    {
                        directory += "/" + pathName; // Build the directory path
                        if (!DirAccess.DirExistsAbsolute(directory))
                            DirAccess.MakeDirAbsolute(directory);
                    }
                }
                
                FileAccess fileAccess = FileAccess.Open(directory + "/" + webpFilePath.Replace(ioInterface.PathUrl, ""), FileAccess.ModeFlags.Write);
                if (fileAccess != null)
                {
                    // Write the bytes to the file
                    fileAccess.StoreBuffer(webpBytes);
                    fileAccess.Close(); // Close the file after writing
                }
                else
                {
                    GD.PrintErr($"Failed to open file for writing: {directory + "/" + webpFilePath.Replace(ioInterface.PathUrl, "")}");
                }
            }
        }
        
        foreach (var tgaFilePath in tgaFiles)
        {
            var tgaBytes = ioInterface.LoadBytes(tgaFilePath);
            if (tgaBytes != null)
            {
                string[] pathTree = tgaFilePath.Replace(ioInterface.PathUrl, "").Split("/");
                
                string directory = tempTexturePath;
                foreach (var pathName in pathTree)
                {
                    if (!pathName.EndsWith(".tga"))
                    {
                        directory += "/" + pathName; // Build the directory path
                        if (!DirAccess.DirExistsAbsolute(directory))
                            DirAccess.MakeDirAbsolute(directory);
                    }
                }
                
                FileAccess fileAccess = FileAccess.Open(directory + "/" + tgaFilePath.Replace(ioInterface.PathUrl, ""), FileAccess.ModeFlags.Write);
                if (fileAccess != null)
                {
                    // Write the bytes to the file
                    fileAccess.StoreBuffer(tgaBytes);
                    fileAccess.Close(); // Close the file after writing
                }
                else
                {
                    GD.PrintErr($"Failed to open file for writing: {directory + "/" + tgaFilePath.Replace(ioInterface.PathUrl, "")}");
                }
            }
        }
        
        foreach (var ktxFilePath in ktxFiles)
        {
            var ktxBytes = ioInterface.LoadBytes(ktxFilePath);
            if (ktxBytes != null)
            {
                string[] pathTree = ktxFilePath.Replace(ioInterface.PathUrl, "").Split("/");
                
                string directory = tempTexturePath;
                foreach (var pathName in pathTree)
                {
                    if (!pathName.EndsWith(".ktx"))
                    {
                        directory += "/" + pathName; // Build the directory path
                        if (!DirAccess.DirExistsAbsolute(directory))
                            DirAccess.MakeDirAbsolute(directory);
                    }
                }
                
                FileAccess fileAccess = FileAccess.Open(directory + "/" + ktxFilePath.Replace(ioInterface.PathUrl, ""), FileAccess.ModeFlags.Write);
                if (fileAccess != null)
                {
                    // Write the bytes to the file
                    fileAccess.StoreBuffer(ktxBytes);
                    fileAccess.Close(); // Close the file after writing
                }
                else
                {
                    GD.PrintErr($"Failed to open file for writing: {directory + "/" + ktxFilePath.Replace(ioInterface.PathUrl, "")}");
                }
            }
        }
        
        foreach (var svgFilePath in svgFiles)
        {
            var svgBytes = ioInterface.LoadBytes(svgFilePath);
            if (svgBytes != null)
            {
                string[] pathTree = svgFilePath.Replace(ioInterface.PathUrl, "").Split("/");
                
                string directory = tempTexturePath;
                foreach (var pathName in pathTree)
                {
                    if (!pathName.EndsWith(".svg"))
                    {
                        directory += "/" + pathName; // Build the directory path
                        if (!DirAccess.DirExistsAbsolute(directory))
                            DirAccess.MakeDirAbsolute(directory);
                    }
                }
                
                FileAccess fileAccess = FileAccess.Open(directory + "/" + svgFilePath.Replace(ioInterface.PathUrl, ""), FileAccess.ModeFlags.Write);
                if (fileAccess != null)
                {
                    // Write the bytes to the file
                    fileAccess.StoreBuffer(svgBytes);
                    fileAccess.Close(); // Close the file after writing
                }
                else
                {
                    GD.PrintErr($"Failed to open file for writing: {directory + "/" + svgFilePath.Replace(ioInterface.PathUrl, "")}");
                }
            }
        }
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