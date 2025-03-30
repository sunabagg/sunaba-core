using System.Reflection;
using Godot;
using WattleScript.Interpreter;
using Timer = Godot.Timer;
using GodotScript = Godot.Script;

namespace Sunaba.Core.Modules;

public class GodotModule : Module
{
    public GodotModule(LuaEnviroment luaEnviroment) : base(luaEnviroment) { }
    
    public override void Init()
    {
        Table GdCoreNamespace = new Table(LuaEnviroment.Script);
        LuaEnviroment.Script.Globals["godot"] = GdCoreNamespace;
        
        UserData.RegisterType(typeof(AudioServer));
        UserData.RegisterType(typeof(CameraServer));
        UserData.RegisterType(typeof(DisplayServer));
        UserData.RegisterType(typeof(NavigationServer2D));
        UserData.RegisterType(typeof(NavigationServer3D));
        UserData.RegisterType(typeof(OpenXRExtensionWrapperExtension));
        UserData.RegisterType(typeof(OpenXRInteractionProfileMetadata));
        UserData.RegisterType(typeof(PhysicsDirectBodyState2D));
        UserData.RegisterType(typeof(PhysicsDirectSpaceState2D));
        UserData.RegisterType(typeof(PhysicsServer2D));
        UserData.RegisterType(typeof(PhysicsServer2DManager));
        UserData.RegisterType(typeof(PhysicsDirectBodyState3D));
        UserData.RegisterType(typeof(PhysicsDirectSpaceState3D));
        UserData.RegisterType(typeof(PhysicsServer3D));
        UserData.RegisterType(typeof(PhysicsServer3DManager));
        UserData.RegisterType(typeof(RefCounted));
        UserData.RegisterType(typeof(RenderingDevice));
        UserData.RegisterType(typeof(RenderingServer));
        UserData.RegisterType(typeof(ThemeDB));
        UserData.RegisterType(typeof(Time));
        UserData.RegisterType(typeof(TranslationServer));
        UserData.RegisterType(typeof(UndoRedo));
        UserData.RegisterType(typeof(XRServer));

        GdCoreNamespace[nameof(AudioServer)] = UserData.CreateStatic(typeof(AudioServer));
        GdCoreNamespace[nameof(CameraServer)] = UserData.CreateStatic(typeof(CameraServer));
        GdCoreNamespace[nameof(DisplayServer)] = UserData.CreateStatic(typeof(DisplayServer));
        GdCoreNamespace[nameof(NavigationServer2D)] = UserData.CreateStatic(typeof(NavigationServer2D));
        GdCoreNamespace[nameof(NavigationServer3D)] = UserData.CreateStatic(typeof(NavigationServer3D));
        GdCoreNamespace[nameof(OpenXRExtensionWrapperExtension)] = UserData.CreateStatic(typeof(OpenXRExtensionWrapperExtension));
        GdCoreNamespace[nameof(OpenXRInteractionProfileMetadata)] = UserData.CreateStatic(typeof(OpenXRInteractionProfileMetadata));
        GdCoreNamespace[nameof(PhysicsDirectBodyState2D)] = UserData.CreateStatic(typeof(PhysicsDirectBodyState2D));
        GdCoreNamespace[nameof(PhysicsDirectSpaceState2D)] = UserData.CreateStatic(typeof(PhysicsDirectSpaceState2D));
        GdCoreNamespace[nameof(PhysicsServer2D)] = UserData.CreateStatic(typeof(PhysicsServer2D));
        GdCoreNamespace[nameof(PhysicsServer2DManager)] = UserData.CreateStatic(typeof(PhysicsServer2DManager));
        GdCoreNamespace[nameof(PhysicsDirectBodyState3D)] = UserData.CreateStatic(typeof(PhysicsDirectBodyState3D));
        GdCoreNamespace[nameof(PhysicsDirectSpaceState3D)] = UserData.CreateStatic(typeof(PhysicsDirectSpaceState3D));
        GdCoreNamespace[nameof(PhysicsServer3D)] = UserData.CreateStatic(typeof(PhysicsServer3D));
        GdCoreNamespace[nameof(PhysicsServer3DManager)] = UserData.CreateStatic(typeof(PhysicsServer3DManager));
        if (LuaEnviroment.Sandboxed == false)
            GdCoreNamespace[nameof(RenderingDevice)] = UserData.CreateStatic(typeof(RenderingDevice));
        GdCoreNamespace[nameof(RenderingServer)] = UserData.CreateStatic(typeof(RenderingServer));
        GdCoreNamespace[nameof(ThemeDB)] = UserData.CreateStatic(typeof(ThemeDB));
        GdCoreNamespace[nameof(Time)] = UserData.CreateStatic(typeof(Time));
        GdCoreNamespace[nameof(TranslationServer)] = UserData.CreateStatic(typeof(TranslationServer));
        GdCoreNamespace[nameof(UndoRedo)] = UserData.CreateStatic(typeof(UndoRedo));
        GdCoreNamespace[nameof(XRServer)] = UserData.CreateStatic(typeof(XRServer));

        if (!LuaEnviroment.Sandboxed)
		{
			UserData.RegisterType(typeof(OS));
			GdCoreNamespace["OS"] = UserData.CreateStatic(typeof(OS));
			UserData.RegisterType(typeof(SceneTree));
			GdCoreNamespace["SceneTree"] = UserData.CreateStatic(typeof(SceneTree));
			UserData.RegisterType(typeof(Engine));
            GdCoreNamespace["Engine"] = UserData.CreateStatic(typeof(Engine));
		}

        UserData.RegisterType<Variant>();
        UserData.RegisterType<Color>();
        UserData.RegisterType<Vector3>();
        UserData.RegisterType<Vector2>();
        UserData.RegisterType<Vector3I>();
        UserData.RegisterType<Vector2I>();
        UserData.RegisterType<Vector4>();
        UserData.RegisterType<Rect2>();
        UserData.RegisterType<Basis>();
        UserData.RegisterType<Transform2D>();
        UserData.RegisterType<Transform3D>();
        UserData.RegisterType<Plane>();
        UserData.RegisterType<Quaternion>();
        UserData.RegisterType<Aabb>();
        UserData.RegisterType<Node>();
        UserData.RegisterType<GodotObject>();
        UserData.RegisterType<TreeItem>();
        UserData.RegisterType<NodePath>();

        GdCoreNamespace["Variant"] = typeof(Variant);
        GdCoreNamespace["Color"] = typeof(Color);
        GdCoreNamespace["Vector3"] = typeof(Vector3);
        GdCoreNamespace["Vector2"] = typeof(Vector2);
        GdCoreNamespace["Vector3i"] = typeof(Vector3I);
        GdCoreNamespace["Vector2i"] = typeof(Vector2I);
        GdCoreNamespace["Vector4"] = typeof(Vector4);
        GdCoreNamespace["Rect2"] = typeof(Rect2);
        GdCoreNamespace["Basis"] = typeof(Basis);
        GdCoreNamespace["Transform2D"] = typeof(Transform2D);
        GdCoreNamespace["Transform3D"] = typeof(Transform3D);
        GdCoreNamespace["Plane"] = typeof(Plane);
        GdCoreNamespace["Quaternion"] = typeof(Quaternion);
        GdCoreNamespace["Aabb"] = typeof(Aabb);
        GdCoreNamespace["Node"] = typeof(Node);
        GdCoreNamespace["Object"] = typeof(GodotObject);
        GdCoreNamespace["TreeItem"] = typeof(TreeItem);
        GdCoreNamespace["NodePath"] = typeof(NodePath);

        
        foreach (var type in GetTypesInheritedFrom(typeof(Node)))
        {
            if (!IsBlackListedType(type) || LuaEnviroment.Sandboxed == false)
            {
                UserData.RegisterType(type);
                GdCoreNamespace[type.Name] = type;
            }
        }

        UserData.RegisterType<RefCounted>();
        GdCoreNamespace["RefCounted"] = typeof(RefCounted);
        foreach (var type in GetTypesInheritedFrom(typeof(RefCounted)))
        {
            if (type == typeof(GltfAccessor))
            {
                continue;
            }
            if ((!IsBlackListedType(type) || LuaEnviroment.Sandboxed == false) && !UserData.IsTypeRegistered(type))
            {
                UserData.RegisterType(type);
                GdCoreNamespace[type.Name] = type;
            }
        }
        
        Type[] enumTypes = GetEnumTypesFromNamespace("Godot");
        foreach (Type type in enumTypes)
        {
            // hack fix to stop the .NET debugger from bitching.
            bool isNested = type.IsNested;
                        
            if (!isNested)
            {
                UserData.RegisterType(type);
                GdCoreNamespace[type.Name] = type;
            }
        }
        //UserData.RegisterType<ItemList>();
        
        
    }
    
    public Type[] GetTypesInheritedFrom(Type baseType)
    {
        Assembly assembly = baseType.Assembly;
        Type[] types = assembly.GetTypes();
        Type[] inheritedTypes = types.Where(t => t.IsSubclassOf(baseType)).ToArray();
        return inheritedTypes;
    }
    
    public Type[] GetEnumTypesFromAssembly(Assembly assembly)
    {
        List<Type> types = new List<Type>();
        foreach (var t in assembly.GetTypes())
        {
            if (t.IsEnum)
            {
                types.Add(t);
            }
        }
        return types.ToArray();
    }
    
    public static Type[] GetEnumTypesFromNamespace(string @namespace)
    {
        List<Type> types = new List<Type>();
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in asm.GetTypes())
            {
                if (type.Namespace == @namespace && type.IsEnum)
                {
                    types.Add(type);
                }
            }
        }

        return types.ToArray();
    }
    
    public Type[] GetBlackListedTypes()
    {
        return new[]
        {
            typeof(GDExtension),
            typeof(GDScript),
            typeof(CSharpScript),
            typeof(GodotScript),
            typeof(DirAccess),
            typeof(ENetConnection)
            ,
        };
    }

    public bool IsBlackListedType(Type type)
    {
        return GetBlackListedTypes().Contains(type);
    }
    
    public Vector2I GetWindowResolution()
    {
        return DisplayServer.WindowGetSize();
    }
}