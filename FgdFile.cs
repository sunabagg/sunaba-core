using System.Text.Json;
using Godot;

namespace Sunaba.Core;

public class FgdFile
{
    public enum TargetMapEditors
    {
        Hammer,
        Sledge,
        TrenchBroom,
        Jack
    }
    
    public string FgdName = "Sunaba";

    public TargetMapEditors TargetMapEditor = TargetMapEditors.TrenchBroom;
    
    public List<FgdFile> BaseFgdFiles = new();
    public List<EntityClass> EntityDefinitions = new();
    
    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }
    
    public static FgdFile? FromJson(string json)
    {
        return JsonSerializer.Deserialize<FgdFile>(json);
    }
    
    public static FgdFile CreateDefault()
    {
        FgdFile fgdFile = new FgdFile();
        
        BaseClass phongBase = new BaseClass();
        Dictionary<string, int> phongsetttings = new() { { "disabled", 0 }, { "Smooth shading", 1} };
        phongBase.ClassProperties["_phong"] = phongsetttings;
        phongBase.ClassProperties["_phong_angle"] = 89.0f;
        Dictionary<string, string> pongSettingsDescription = new() { { "_phong", "Phong settings" }, { "_phong_angle", "Phong angle" } };
        phongBase.ClassPropertiesDescription = pongSettingsDescription;
        phongBase.ClassPropertiesDescription["_phong_angle"] = "Phong smoothing angle";
        phongBase.ClassName = "Phong";
        fgdFile.EntityDefinitions.Add(phongBase);
        
        SolidClass worldspawn = new SolidClass();
        worldspawn.ClassName = "worldspawn";
        worldspawn.BuildOcclusion = true;
        worldspawn.MetaProperties["color"] = new Color("#cccccc");
        ((EntityClass)worldspawn).NodeClass = "StaticBody3D";
        fgdFile.EntityDefinitions.Add(worldspawn);

        var funcGeo = new SolidClass();
        funcGeo.ClassName = "func_geo";
        funcGeo.BuildOcclusion = true;
        funcGeo.CollisionShapeType = SolidClass.CollisionShapeTypeEnum.Concave;
        funcGeo.BaseClasses.Add(phongBase);
        funcGeo.MetaProperties["color"] = new Color("#cccccc");
        ((EntityClass)funcGeo).NodeClass = "StaticBody3D";
        fgdFile.EntityDefinitions.Add(funcGeo);
        
        var funcDetail = new SolidClass();
        funcDetail.ClassName = "func_detail";
        funcDetail.CollisionShapeType = SolidClass.CollisionShapeTypeEnum.Concave;
        funcDetail.BaseClasses.Add(phongBase);
        funcDetail.MetaProperties["color"] = new Color("#cccccc");
        ((EntityClass)funcDetail).NodeClass = "StaticBody3D";
        fgdFile.EntityDefinitions.Add(funcDetail);
        
        var funcDetailIllusionary = new SolidClass();
        funcDetailIllusionary.ClassName = "func_detail_illusionary";
        funcDetailIllusionary.CollisionShapeType = SolidClass.CollisionShapeTypeEnum.None;
        funcDetailIllusionary.BaseClasses.Add(phongBase);
        funcDetailIllusionary.MetaProperties["color"] = new Color("#cccccc");
        ((EntityClass)funcDetailIllusionary).NodeClass = "Node3D";
        fgdFile.EntityDefinitions.Add(funcDetailIllusionary);
        
        var funcIllusionary = new SolidClass();
        funcIllusionary.ClassName = "func_illusionary";
        funcIllusionary.BuildOcclusion = true;
        funcIllusionary.CollisionShapeType = SolidClass.CollisionShapeTypeEnum.None;
        funcIllusionary.BaseClasses.Add(phongBase);
        funcIllusionary.MetaProperties["color"] = new Color("#cccccc");
        ((EntityClass)funcIllusionary).NodeClass = "Node3D";
        fgdFile.EntityDefinitions.Add(funcIllusionary);

        return fgdFile;
    }
}