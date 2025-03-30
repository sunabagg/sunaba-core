using Godot;

namespace Sunaba.Core;

public class MapSettings
{
    public float ScaleFactor = 0.03125f;
    private float _inverseScaleFactor = 32f;
    public float InverseScaleFactor
    {
        get
        {
            return _inverseScaleFactor;
        }
        set
        {
            _inverseScaleFactor = value;
            ScaleFactor = (float)(1.0 * value);
        }
    }
    
    public FgdFile FgdFile = FgdFile.CreateDefault();

    public String EntityNameProperty = "";
    public string BaseTextureDir = "";
    public List<string> TextureFileExtensions = new() { "png", "jpg", "jpeg", "bmp", "tga" , "webp" };
    
    public string ClipTexture = "special/clip";
    public string SkipTexture = "special/skip";
    
    private static Material _getDefaultMaterial()
    {
        var mat = new StandardMaterial3D();

        mat.AlbedoTexture = GD.Load<Texture2D>("res://default_texture.png");
        mat.MetallicSpecular = 0.0f;
        mat.TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest;
        
        return mat;
    }
    
    public Material DefaultMaterial = _getDefaultMaterial();
    
    public string AlbedoMapPattern = "%s_albedo.%s";
    public string NormalMapPattern = "%s_normal.%s";
    public string MetallicMapPattern = "%s_metallic.%s";
    public string RoughnessMapPattern = "%s_roughness.%s";
    public string EmissionMapPattern = "%s_emission.%s";
    public string AmbientOcclusionMapPattern = "%s_ao.%s";
    public string HeightMapPattern = "%s_height.%s";
    public string OrmMapPattern = "%s_orm.%s";

    public float UvUnwrapTexelSize = 2.0f;
    
    public bool UseTrenchbroomGroupsHierarchy = false;
}