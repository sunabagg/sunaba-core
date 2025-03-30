using Godot;
using Sunaba.Core.Modules;
using WattleScript.Interpreter;

namespace Sunaba.Core;

public partial class LuaNode : Node
{
    public LuaEnviroment _luaEnviroment;
    
    private String _mainScriptPath = "app://main.lua";

    public Theme Theme;
    public string ThemeName = "System";

    /*
    [Signal] public delegate void MainEventHandler();
    [Signal] public delegate void UpdateEventHandler(float delta);
    [Signal] public delegate void PhysicsUpdateEventHandler(float delta);
    [Signal] public delegate void InputEventHandler(InputEvent @event);
    [Signal] public delegate void UnhandledInputEventHandler(InputEvent @event);
    [Signal] public delegate void ShortcutInputEventHandler(InputEvent @event);
    [Signal] public delegate void UnhandledKeyInputEventHandler(InputEvent @event);
    [Signal] public delegate void StopEventHandler();
     */
    
    public LuaNode()
    {
        _luaEnviroment = new LuaEnviroment();
        _luaEnviroment.AddModule(typeof(GodotModule));
        _luaEnviroment.AddModule(typeof(GodotExtensions));
        _luaEnviroment.AddModule(typeof(UiModule));
        UserData.RegisterType<LuaNode>();
        _luaEnviroment.Script.Globals["rootNode"] = this;
        LoadTheme();
    }
    
    public void StartFromPath(string path)
    {
        FileSystemIo fileSys = new FileSystemIo(path, "app://");
        _luaEnviroment.IoInterface.Register(fileSys);
        _luaEnviroment.Start(_mainScriptPath);
    }
    
    public void StartFromZipFile(byte[] data)
    {
        IoInterfaceZip zip = new IoInterfaceZip(data, "app://");
        _luaEnviroment.IoInterface.Register(zip);
        _luaEnviroment.Start(_mainScriptPath);
    }
    
    public void StartFromZipFile(string path)
    {
        IoInterfaceZip zip = new IoInterfaceZip(path, "app://");
        _luaEnviroment.IoInterface.Register(zip);
        _luaEnviroment.Start(_mainScriptPath);
    }
    
    private float DoubleToFloat(double d)
    {
        return (float)d;
    }
    
    public override void _Process(double delta)
    {
        if (_luaEnviroment.Script.Globals["update"] != null)
        {
            var update = _luaEnviroment.Script.Globals["update"];
            _luaEnviroment.Script.Call(update, DoubleToFloat(delta));
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_luaEnviroment.Script.Globals["physicsUpdate"] != null)
        {
            var physicsUpdate = _luaEnviroment.Script.Globals["physicsUpdate"];
            _luaEnviroment.Script.Call(physicsUpdate, DoubleToFloat(delta));
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (_luaEnviroment.Script.Globals["input"] != null)
        {
            var input = _luaEnviroment.Script.Globals["input"];
            _luaEnviroment.Script.Call(input, @event);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (_luaEnviroment.Script.Globals["unhandledInput"] != null)
        {
            var unhandledInput = _luaEnviroment.Script.Globals["unhandledInput"];
            _luaEnviroment.Script.Call(unhandledInput, @event);
        }
    }

    public override void _ShortcutInput(InputEvent @event)
    {
        if (_luaEnviroment.Script.Globals["shortcutInput"] != null)
        {
            var shortcutInput = _luaEnviroment.Script.Globals["shortcutInput"];
            _luaEnviroment.Script.Call(shortcutInput, @event);
        }
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (_luaEnviroment.Script.Globals["unhandledKeyInput"] != null)
        {
            var unhandledKeyInput = _luaEnviroment.Script.Globals["unhandledKeyInput"];
            _luaEnviroment.Script.Call(unhandledKeyInput, @event);
        }
    }

    public override void _ExitTree()
    {
        if (_luaEnviroment.Script.Globals["stop"] != null)
        {
            var stop = _luaEnviroment.Script.Globals["stop"];
            _luaEnviroment.Script.Call(stop);
        }
    }
    
    public void ConnectSignal(Node node, Table table, string signalName, string functionName)
    {
        node.Connect(signalName, new Callable());
    }

    public Theme GetTheme()
	{
		if (ThemeName == "System")
		{
			if (DisplayServer.IsDarkMode())
			{
				return ResourceLoader.Load<Theme>("res://addons/lite/dark.tres");
			}
			else
			{
				return ResourceLoader.Load<Theme>("res://addons/lite/light.tres");
			}
		}
		else if (ThemeName == "Dark")
		{
			return ResourceLoader.Load<Theme>("res://addons/lite/dark.tres");
		}
		else if (ThemeName == "Light")
		{
			return ResourceLoader.Load<Theme>("res://addons/lite/light.tres");
		}
		else
		{
			return ResourceLoader.Load<Theme>("res://addons/lite/light.tres");
		}
	}

    public void LoadTheme()
	{
		Theme theme = GetTheme();
        if (theme != null)
        {
            var window = GetWindow();
            if (window != null)
            {
                window.Theme = theme;
            }
        }
	}
}