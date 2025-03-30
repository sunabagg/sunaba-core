using System.Reflection;
using Godot;
using Sunaba.Core.Modules;
using WattleScript.Interpreter;
using Script = WattleScript.Interpreter.Script;

namespace Sunaba.Core;

public class LuaEnviroment
{
    public IoManager IoInterface;

    public Script Script;

    
    public Dictionary<string, string> EnviromentVariables = new();
    
    public List<Module> Modules = new();
    
    private bool _sandboxed = false;
    
    public bool Sandboxed
    {
        get => _sandboxed;
        set
        {
            if (_sandboxed == false)
            {
                _sandboxed = value;
            }
            else
            {
                throw new Exception("Sandbox cannot be turn off once set to true");
            }
        }
    }
    
    public class OnExitEventArgs : EventArgs
    {
        public int ExitCode;
    }
    
    public event EventHandler<OnExitEventArgs> OnExit;
    
    public LuaEnviroment()
    {
        Script = new Script();
        IoInterface = new IoManager();
        Script.GlobalOptions.Platform = new BoltPlatformAccessor(this);
        Script.Options.ScriptLoader = new IoInterfaceScriptLoader(IoInterface);
        Script.Globals["doubleToFloat"] = (Func<double, float>)DoubleToFloat;
        Script.Globals["convert"] = (Func<object, Variant>)convert;
        
        UserData.RegisterType<IoManager>(); // Register the IoManager type
        Script.Globals["ioInterface"] = IoInterface;
        
        EnviromentVariables["PLATFORM"] = "Bolt";
        EnviromentVariables["PLATFORM_VERSION"] = "1.0.0";

        EnviromentVariables["OS_NAME"] = OS.GetName();

        Script.Globals["recast"] = (Func<object, Type, object>)reCast;
        Script.Globals["fixme"] = (Func<object, object>)fixme;
        Script.Globals["typeofobj"] = (Func<object, string>)getTypeName;
        Script.Globals["addSignal"] = (Action<GodotObject, string, Closure>)addSignal;
        Script.Globals["setvar"] = (Action<object, string, object>)setValue;
        Script.Globals["SetOnItemActivated"] = (Action<ItemList, Closure>)SetOnItemActivated;
        Script.Globals["addLuaFuncToIntEvent"] = (Action<object, string, Closure>)addLuaFuncToIntEvent;
        
        AddModule(typeof(Sys));
    }
    
    public void AddModule(Module module)
    {
        Modules.Add(module);
        module.Init();
    }
    
    public void AddModule(Type moduleType)
    {
        var module = (Module)Activator.CreateInstance(moduleType, this);
        Modules.Add(module);
        module.Init();
    }
    
    private bool _isInitNoSandbox = false;

    public void InitNoSandbox()
    {
        _isInitNoSandbox = true;

        //AddModule(typeof(SocketPlugin));

        Script.Globals["loadDll"] = (Action<string>)LoadModuleDll;

        IoInterface ioInterface = new UnixSystemIo();
        if (OS.GetName() == "Windows")
        {
            ioInterface = new WindowsSystemIo();
            UserData.RegisterType<WindowsSystemIo>();
            Script.Globals["WindowsSystemIo"] = typeof(WindowsSystemIo);
        }
        else {
            UserData.RegisterType<UnixSystemIo>();
            Script.Globals["UnixSystemIo"] = typeof(UnixSystemIo);
        }
        UserData.RegisterType<SystemIoBase>();
        Script.Globals["BaseSystemIo"] = typeof(SystemIoBase);
        UserData.RegisterType<FileSystemIo>();
        Script.Globals["FileSystemIo"] = typeof(FileSystemIo);

        IoInterface.Register(ioInterface);
            
    }
    
    public void Start(string entryPoint)
    {
        if (!_isInitNoSandbox && !Sandboxed)
            InitNoSandbox();
        
        string fullPath = IoInterface.GetFullPath(entryPoint);
        //GD.Print($"Loading script from path: {fullPath}");

        try 
        {
            Script.DoFile(entryPoint);
        }
        catch (Exception e)
        {
            if (e is ScriptRuntimeException ex)
            {
                GD.PrintErr($"Error in script: {ex.DecoratedMessage}");
            }
            else
            {
                GD.PrintErr($"Error in script: {e.ToString()}");
            }
        }
    }

    public void LoadScript(string path)
    {
        
    }

    public void Exit(int exitCode)
    {
        OnExit?.Invoke(this, new OnExitEventArgs() { ExitCode = exitCode });
    }
    
    public float DoubleToFloat(double d)
    {
        return (float)d;
    }

    private void LoadModuleDll(string path)
    {
        Assembly assembly = Assembly.LoadFile(path);
        foreach (Type type in assembly.GetTypes())
        {
            if (type.BaseType == typeof(Module))
            {
                AddModule(type);
            }
        }
    }

    private Variant convert(object obj)
    {
        if (obj is string s)
            return Variant.CreateFrom(s);
        else if (obj is int i)
            return Variant.CreateFrom(i);
        else if (obj is long l)
            return Variant.CreateFrom(l);
        else if (obj is float f)
            return Variant.CreateFrom(f);
        else if (obj is double d)
            return Variant.CreateFrom(d);
        else if (obj is Vector2 v2)
            return Variant.CreateFrom(v2);
        else if (obj is Vector3 v3)
            return Variant.CreateFrom(v3);
        else if (obj is Color c)
            return Variant.CreateFrom(c);
        else if (obj is bool b)
            return Variant.CreateFrom(b);
        else if (obj is Node n)
            return Variant.CreateFrom(n);
        else if (obj is Control c2)
            return Variant.CreateFrom(c2);
        else if (obj is Node2D n2)
            return Variant.CreateFrom(n2);
        else if (obj is Node3D n3)
            return Variant.CreateFrom(n3);
        else if (obj is Vector4 v4)
            return Variant.CreateFrom(v4);
        else if (obj is Quaternion q)
            return Variant.CreateFrom(q);
        else if (obj is Vector2I v2i)
            return Variant.CreateFrom(v2i);
        else if (obj is Vector3I v3i)
            return Variant.CreateFrom(v3i);
        else if (obj is Rect2 r2)
            return Variant.CreateFrom(r2);
        else if (obj is Rect2I r2i)
            return Variant.CreateFrom(r2i);
        else if (obj is GodotObject gobj)
            return Variant.CreateFrom(gobj);
        else
            return Variant.CreateFrom("");
        
    }

    public string getTypeName(object obj) => obj.GetType().Name;

    public object fixme(object obj)
    {
        var type = obj.GetType();
        return Convert.ChangeType(obj, type);
    }

    public object reCast(object obj, Type type)
    {
        return Convert.ChangeType(obj, type);
    }

    public object? getProperty(object obj, string name)
    {
        return obj.GetType().GetProperty(name).GetValue(obj);
    }

    public void setValue(object obj, string name, object value) 
    {
        var property = obj.GetType().GetProperty(name);
        if (property != null)
        {
            if (property.PropertyType.IsEnum && value is double d)
            {
                property.SetValue(obj, Enum.Parse(property.PropertyType, d.ToString()));
                return;
            }
            else if (property.PropertyType.IsEnum && value is string s) 
            {
                property.SetValue(obj, Enum.Parse(property.PropertyType, s));
                return;
            }
            property.SetValue(obj, value);
        }
    }

    // stupid hack
    public void SetOnItemActivated(ItemList itemList, Closure closure)
    {
        itemList.ItemActivated += args => closure.Call(args);
    }

    public void addSignal(GodotObject godotObject, string name, Closure closure)
    {
        var eventName = name.ToPascalCase();
        var t = godotObject.GetType();
        var _event = t.GetEvent(eventName);
        if (_event == null) return;
        
        var action = new Action<Variant[]>(args => closure.Call(args));
        Callable callable = Callable.From(action);
        godotObject.Connect(name, callable);
    }

    // this hack exists because apparently it isn't posible to use lua function with evevtn that take an int parameter in MoonSharp
    public void addLuaFuncToIntEvent(object obj, string eventName, Closure closure)
    {
        if (obj == null) return;
        var t = obj.GetType();
        if (t == null) return;
        var _event = t.GetEvent(eventName);
        if (_event == null) return;

        var del = WrapDoubleFuncToIntDel(closure);
        var eventDel = Delegate.CreateDelegate(_event.EventHandlerType, del.Target, del.Method);
        _event.AddEventHandler(obj, eventDel);
    }

    public void addLuaFuncToEvent(object obj, string eventName, Closure closure)
    {
        if (obj == null) return;
        var t = obj.GetType();
        if (t == null) return;
        var _event = t.GetEvent(eventName);
        if (_event == null) return;

        var del = new Action<object[]>((args) => closure.Call(args));
        var eventDel = Delegate.CreateDelegate(_event.EventHandlerType, del.Target, del.Method);
        _event.AddEventHandler(obj, eventDel);
    }

    public Delegate WrapDoubleFuncToIntDel(Closure closure) 
    {
        return new Action<long>(l => closure.Call((double)l));
    }


}