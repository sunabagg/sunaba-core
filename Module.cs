namespace Sunaba.Core;

public abstract class Module
{
    public LuaEnviroment LuaEnviroment;
    public Module(LuaEnviroment luaEnviroment)
    {
        LuaEnviroment = luaEnviroment;
    }

    public virtual void Init()
    {
        
    }
}