using System.Text;
using Godot;
using WattleScript.Interpreter;
using WattleScript.Interpreter.Platforms;
using Script = WattleScript.Interpreter.Script;

namespace Sunaba.Core;

public class BoltPlatformAccessor : PlatformAccessorBase
{
    public LuaEnviroment Enviroment;
    
    public BoltPlatformAccessor(LuaEnviroment enviroment)
    {
        Enviroment = enviroment;
    }
    
    public override string GetPlatformNamePrefix()
    {
        return "Bolt";
    }

    public override void DefaultPrint(string content)
    {
        GD.Print(content);
    }

    public override Stream IO_OpenFile(Script script, string filename, Encoding encoding, string mode)
    {
        StreamMode streamMode;
        switch (mode)
        {
            case "r":
                streamMode = StreamMode.Read;
                break;
            case "w":
                streamMode = StreamMode.Write;
                break;
            case "a":
                streamMode = StreamMode.Append;
                break;
            case "r+":
                streamMode = StreamMode.ReadPlus;
                break;
            case "w+":
                streamMode = StreamMode.WritePlus;
                break;
            case "a+":
                streamMode = StreamMode.AppendPlus;
                break;
            default:
                throw new Exception("Invalid mode");
        }
        
        return Enviroment.IoInterface.GetStream(filename, streamMode);
    }

    public override Stream IO_GetStandardStream(StandardFileType type)
    {
        if (type == StandardFileType.StdIn)
        {
            return Enviroment.IoInterface.GetStream("stdin", StreamMode.Read);
        }
        else if (type == StandardFileType.StdOut)
        {
            return Enviroment.IoInterface.GetStream("stdout", StreamMode.Write);
        }
        else if (type == StandardFileType.StdErr)
        {
            return Enviroment.IoInterface.GetStream("stderr", StreamMode.Write);
        }
        else
        {
            throw new Exception("Invalid Standard File Type");
        }
    }

    public override string IO_OS_GetTempFilename()
    {
        return Enviroment.IoInterface.GetTempFilename();
    }

    public override void OS_ExitFast(int exitCode)
    {
        Enviroment.Exit(exitCode);
    }

    public override bool OS_FileExists(string file)
    {
        return Enviroment.IoInterface.FileExists(file);
    }

    public override void OS_FileDelete(string file)
    {
        Enviroment.IoInterface.DeleteFile(file);
    }

    public override void OS_FileMove(string src, string dst)
    {
        Enviroment.IoInterface.MoveFile(src, dst);
    }

    public override int OS_Execute(string cmdline)
    {
        throw new NotImplementedException();
    }

    public override CoreModules FilterSupportedCoreModules(CoreModules module)
    {
        return CoreModules.Preset_Complete;
    }

    public override string GetEnvironmentVariable(string envvarname)
    {
        var envVariables = Enviroment.EnviromentVariables;
        foreach (var envKey in envVariables)
        {
            if (envKey.Key == envvarname)
            {
                return envKey.Value;
            }
        }
        
        return null;
    }
}