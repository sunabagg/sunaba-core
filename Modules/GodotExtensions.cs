using WattleScript.Interpreter;
using Sunaba.Core;

namespace Sunaba.Core.Modules;

public class GodotExtensions : Module
{
    public GodotExtensions(LuaEnviroment luaEnviroment) : base(luaEnviroment)
    {
    }

    public override void Init()
    {
        Table GdExtNamespace = new Table(LuaEnviroment.Script);
        LuaEnviroment.Script.Globals["gdx"] = GdExtNamespace;

        UserData.RegisterType<FreeLookCamera2D>();
        UserData.RegisterType<FreeLookCamera3D>();
        UserData.RegisterType<MouseRayCast3D>();
        UserData.RegisterType<UiDocument>();
        UserData.RegisterType<UiStyleSheet>();
        UserData.RegisterType<TabManager>();
        UserData.RegisterType<AnimationData>();
        UserData.RegisterType<MeshData>();
        UserData.RegisterType<CharacterController>();
        UserData.RegisterType<EntityClass>();
        UserData.RegisterType<BaseClass>();
        UserData.RegisterType<SolidClass>();
        UserData.RegisterType<PointClass>();
        UserData.RegisterType<ModelPointClass>();
        UserData.RegisterType<FgdFile>();

        GdExtNamespace["FreeLookCamera2D"] = typeof(FreeLookCamera2D);
        GdExtNamespace["FreeLookCamera3D"] = typeof(FreeLookCamera3D);
        GdExtNamespace["MouseRayCast3D"] = typeof(MouseRayCast3D);
        GdExtNamespace["UiDocument"] = typeof(UiDocument);
        GdExtNamespace["UiStyleSheet"] = typeof(UiStyleSheet);
        GdExtNamespace["TabManager"] = typeof(TabManager);
        GdExtNamespace["AnimationData"] = typeof(AnimationData);
        GdExtNamespace["MeshData"] = typeof(MeshData);
        GdExtNamespace["CharacterController"] = typeof(CharacterController);
        GdExtNamespace["EntityClass"] = typeof(EntityClass);
        GdExtNamespace["BaseClass"] = typeof(BaseClass);
        GdExtNamespace["SolidClass"] = typeof(SolidClass);
        GdExtNamespace["PointClass"] = typeof(PointClass);
        GdExtNamespace["ModelPointClass"] = typeof(ModelPointClass);
        GdExtNamespace["FgdFile"] = typeof(FgdFile);
    }
}