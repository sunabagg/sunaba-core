namespace Sunaba.Core;

public class ModelPointClass : PointClass
{
    public enum TargetMapEditorEnum
    {
        Generic,
        TrechBroom,
        Hammer,
    }
    
    public TargetMapEditorEnum TargetMapEditor = TargetMapEditorEnum.Generic;
    public string ModelsSubFolder = "";
    public string ScaleExpression = "";
    public bool GenerateSizeProperty = false;
}