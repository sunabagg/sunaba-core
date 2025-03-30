using WattleScript.Interpreter;

namespace Sunaba.Core.Modules;

public class UiModule : Module
{
    public UiModule(LuaEnviroment luaEnviroment) : base(luaEnviroment)
    {
    }

    public override void Init()
    {
        Table UiNamespace = new Table(LuaEnviroment.Script);
        LuaEnviroment.Script.Globals["ui"] = UiNamespace;
        
        UserData.RegisterType<UiDocument>();
        UiNamespace["document"] = typeof(UiDocument);
        UserData.RegisterType<UiStyleSheet>();
        UiNamespace["styleSheet"] = typeof(UiStyleSheet);
    }
}