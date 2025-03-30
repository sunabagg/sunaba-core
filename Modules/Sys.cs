using WattleScript.Interpreter;

namespace Sunaba.Core.Modules;

public class Sys : Module
{
    public Sys(LuaEnviroment luaEnviroment) : base(luaEnviroment)
    {
    }

    public override void Init()
    {
        Table SysNamespace = new Table(LuaEnviroment.Script);
        LuaEnviroment.Script.Globals["sys"] = SysNamespace;
        
        UserData.RegisterType<IoManager>();
        UserData.RegisterType<FileSystemIo>();
        UserData.RegisterType<IoInterfaceZip>();

        SysNamespace["IoManager"] = IoManager;
        SysNamespace["FileSystemIo"] = FileSystemIo;
        SysNamespace["IoInterfaceZip"] = IoInterfaceZip;
        
    }

    public IoManager IoManager()
    {
        return new IoManager();
    }
    
    public FileSystemIo FileSystemIo(String path, string pathUrl)
    {
        return new FileSystemIo(path, pathUrl);
    }
    
    public IoInterfaceZip IoInterfaceZip(byte[] data, String pathUrl)
    {
        return new IoInterfaceZip(data, pathUrl);
    }
}