using Godot;

namespace Sunaba.Core;

public partial class SimpleRuntimeNode : Node
{
    public LuaNode LuaNode;

    public SimpleRuntimeNode() 
    {
    }
    
    public override void _Ready()
    {
        try 
        {
            var args = OS.GetCmdlineArgs();
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("--dirpath="))
                {
                    var p = args[i].Replace("--dirpath=", "");
                    StartFromPath(p);
                }
                else if (args[i].StartsWith("--spkgpath="))
                {
                    var p = args[i].Replace("--spkgpath=", "");
                    StartFromZipFile(p);
                }
            }
            var window = GetWindow();
            window.Size = new Vector2I(1152, 648);
            window.Borderless = false;
            window.Unresizable = false;
            window.MoveToCenter();
            var splashscreenTexture = GetNode("Splashscreen");
            if (splashscreenTexture != null)
            {
                splashscreenTexture.QueueFree();
            }
        }
        catch (Exception e)
        {
            var dialog = new AcceptDialogPlus(AcceptDialogPlus.TypeEnum.Error);
            dialog.Text = e.ToString();
            dialog.Title = "Error";
            AddChild(dialog);
            dialog.PopupCentered();
        }
    }
    
    public void StartFromPath(string path)
    {
        LuaNode = new LuaNode();
        AddChild(LuaNode);
        LuaNode.StartFromPath(path);
    }

    public void StartFromZipFile(String path)
    {
        LuaNode = new LuaNode();
        AddChild(LuaNode);
        LuaNode.StartFromZipFile(path);
    }
}