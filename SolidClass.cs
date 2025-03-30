using Godot;

namespace Sunaba.Core;

public class SolidClass : EntityClass
{
    public enum SpawnTypeEnum
    {
        Worldspawn,
        MergeWorldspawn,
        Entity
    }
    public enum OriginTypeEnum
    {
        Averaged,
        Absolute,
        Relative,
        Brush,
        BoundsCenter,
        BroundsMins,
        BoundsMaxs
    }
    public enum CollisionShapeTypeEnum 
    {
        None,
        Convex,
        Concave
    }

    public SpawnTypeEnum SpawnType = SpawnTypeEnum.Entity;
    public OriginTypeEnum OriginType = OriginTypeEnum.BoundsCenter;
    
    public bool BuildVisuals = true;
    public bool UseInBakedLighting = true;
    public GeometryInstance3D.ShadowCastingSetting ShadowCastingSetting = GeometryInstance3D.ShadowCastingSetting.On;
    public bool BuildOcclusion = false;
    
    public CollisionShapeTypeEnum CollisionShapeType = CollisionShapeTypeEnum.Convex;
    public int[] CollisionLayer = { 1 };
    public int[] CollisionMask = { 1 };
    public float CollisionPriority = 1.0f;
    public float CollisionShapeMargin = 0.04f;

    public bool AddTexturesMetadata = false;
    public bool AddVertexMetadata = false;
    public bool AddFacePositionMetadata = false;
    public bool AddFaceNormalMetadata = false;
    public bool AddCollisionShapwFace = false;
}