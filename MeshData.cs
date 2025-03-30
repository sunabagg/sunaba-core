using System;
using Godot;
using System.Collections.Generic;
using System.Linq;
using GdArray = Godot.Collections.Array;
using GdDictionary = Godot.Collections.Dictionary;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Sunaba.Core;

public class MeshData
{
    public System.Collections.Generic.Dictionary<string, object> Data;

    public static MeshData FromImporterMesh(ImporterMesh importerMesh)
    {
        var meshData = new System.Collections.Generic.Dictionary<string, object>();
        
        for (int i = 0; i < importerMesh.GetSurfaceCount(); i++)
        {
            var surfaceData = new System.Collections.Generic.Dictionary<string, object>();

            // Retrieve the arrays from the surface (vertices, normals, indices, etc.)
            var arrays = importerMesh.GetSurfaceArrays(i);

            // Extract vertices and convert them to a list
            if (arrays[(int)ArrayMesh.ArrayType.Vertex] is Variant verticesVariant)
            {
                var vertices = (Vector3[])verticesVariant;
                var vertexList = new List<System.Collections.Generic.Dictionary<string, float>>();
                foreach (var vertex in vertices)
                {
                    vertexList.Add(new System.Collections.Generic.Dictionary<string, float>
                    {
                        { "x", vertex.X },
                        { "y", vertex.Y },
                        { "z", vertex.Z }
                    });
                }
                surfaceData["vertices"] = vertexList;
            }

            // Extract indices and convert them to a list
            if (arrays[(int)ArrayMesh.ArrayType.Index] is Variant indicesVariant)
            {
                var indices = (int[])indicesVariant;
                surfaceData["indices"] = indices;
            }
            
            if (arrays[(int)ArrayMesh.ArrayType.Normal] is Variant normalsVariant)
            {
                var normals = (Vector3[])normalsVariant;
                var normalList = new List<System.Collections.Generic.Dictionary<string, float>>();
                foreach (var normal in normals)
                {
                    normalList.Add(new System.Collections.Generic.Dictionary<string, float>
                    {
                        { "x", normal.X },
                        { "y", normal.Y },
                        { "z", normal.Z }
                    });
                }
                surfaceData["normals"] = normalList;
            }
            
            if (arrays[(int)ArrayMesh.ArrayType.TexUV] is Variant texUvVariant)
            {
                var texUv = (Vector2[])texUvVariant;
                var texUvList = new List<System.Collections.Generic.Dictionary<string, float>>();
                foreach (var uv in texUv)
                {
                    texUvList.Add(new System.Collections.Generic.Dictionary<string, float>
                    {
                        { "x", uv.X },
                        { "y", uv.Y }
                    });
                }
                surfaceData["texUv"] = texUvList;
            }

            if (arrays[(int)ArrayMesh.ArrayType.TexUV2] is Variant texUv2Variant)
            {
                var texUv2 = (Vector2[])texUv2Variant;
                var texUv2List = new List<System.Collections.Generic.Dictionary<string, float>>();
                foreach (var uv in texUv2)
                {
                    texUv2List.Add(new System.Collections.Generic.Dictionary<string, float>
                    {
                        { "x", uv.X },
                        { "y", uv.Y }
                    });
                }
                surfaceData["texUv2"] = texUv2List;
            }

            if (arrays[(int)ArrayMesh.ArrayType.Tangent] is Variant tangentVariant)
            {
                var tangents = (float[])tangentVariant;
                surfaceData["tangents"] = tangents;
            }
            
            if (arrays[(int)ArrayMesh.ArrayType.Color] is Variant colorVariant)
            {
                var colors = (Color[])colorVariant;
                var colorList = new List<System.Collections.Generic.Dictionary<string, float>>();
                foreach (var color in colors)
                {
                    colorList.Add(new System.Collections.Generic.Dictionary<string, float>
                    {
                        { "r", color.R },
                        { "g", color.G },
                        { "b", color.B },
                        { "a", color.A }
                    });
                }
                surfaceData["colors"] = colorList;
            }

            if (arrays[(int)ArrayMesh.ArrayType.Custom0] is Variant custom0Variant)
            {
                var custom0bytes = (byte[])custom0Variant;
                var custom0Base64 = Convert.ToBase64String(custom0bytes);
                surfaceData["custom0"] = custom0Base64;
            }
            
            if (arrays[(int)ArrayMesh.ArrayType.Custom1] is Variant custom1Variant)
            {
                var custom1bytes = (byte[])custom1Variant;
                var custom1Base64 = Convert.ToBase64String(custom1bytes);
                surfaceData["custom1"] = custom1Base64;
            }
            
            if (arrays[(int)ArrayMesh.ArrayType.Custom2] is Variant custom2Variant)
            {
                var custom2bytes = (byte[])custom2Variant;
                var custom2Base64 = Convert.ToBase64String(custom2bytes);
                surfaceData["custom2"] = custom2Base64;
            }
            
            if (arrays[(int)ArrayMesh.ArrayType.Custom3] is Variant custom3Variant)
            {
                var custom3bytes = (byte[])custom3Variant;
                var custom3Base64 = Convert.ToBase64String(custom3bytes);
                surfaceData["custom3"] = custom3Base64;
            }

            if (arrays[(int)ArrayMesh.ArrayType.Bones] is Variant bonesVariant)
            {
                var bones = (float[])bonesVariant;
                surfaceData["bones"] = bones;
            }

            // Add other array types similarly as needed
            // surfaceData["normals"] = ...

            // Add the surface data to the mesh data dictionary
            meshData[$"surface_{i}"] = surfaceData;
        }
        
        var md = new MeshData();
        md.Data = meshData;
        return md;
    }
    
    public string ToJson()
    {
        var JsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        return JsonSerializer.Serialize(Data, JsonSerializerOptions);
    }
    
    public static MeshData FromJson(string json)
    {
        var md = new MeshData();
        var data = Json.ParseString(json);
        var gddict = data.AsGodotDictionary();
        md.Data = toDictionary(gddict);
        return md;
    }
    
    private static Dictionary<string, object> toDictionary(GdDictionary dict)
    {
        var newDict = new Dictionary<string, object>();
        foreach (var kvp in dict)
        {
            if (kvp.Value.VariantType == Variant.Type.Dictionary)
            {
                newDict[kvp.Key.ToString()] = toDictionary(kvp.Value.AsGodotDictionary());
            }
            else if (kvp.Value.VariantType == Variant.Type.Array)
            {
                newDict[kvp.Key.ToString()] = toArray(kvp.Value.AsGodotArray());
            }
            else if (kvp.Value.VariantType == Variant.Type.Nil)
            {
                newDict[kvp.Key.ToString()] = null;
            }
            else if (kvp.Value.VariantType == Variant.Type.Bool)
            {
                newDict[kvp.Key.ToString()] = kvp.Value.AsBool();
            }
            else if (kvp.Value.VariantType == Variant.Type.Int)
            {
                newDict[kvp.Key.ToString()] = kvp.Value.AsInt32();
            }
            else if (kvp.Value.VariantType == Variant.Type.Float)
            {
                newDict[kvp.Key.ToString()] = kvp.Value.As<float>();
            }
            else if (kvp.Value.VariantType == Variant.Type.String)
            {
                newDict[kvp.Key.ToString()] = kvp.Value.ToString();
            }
            else
            {
                newDict[kvp.Key.ToString()] = kvp.Value;
            }
        }
        return newDict;
    }
    
    private static List<object> toArray(GdArray array)
    {
        var newArray = new List<object>();
        foreach (var item in array)
        {
            if (item.VariantType == Variant.Type.Dictionary)
            {
                newArray.Add(toDictionary(item.AsGodotDictionary()));
            }
            else if (item.VariantType == Variant.Type.Array)
            {
                newArray.Add(toArray(item.AsGodotArray()));
            }
            else if (item.VariantType == Variant.Type.Nil)
            {
                newArray.Add(null);
            }
            else if (item.VariantType == Variant.Type.Bool)
            {
                newArray.Add(item.AsBool());
            }
            else if (item.VariantType == Variant.Type.Int)
            {
                newArray.Add(item.AsInt32());
            }
            else if (item.VariantType == Variant.Type.Float)
            {
                newArray.Add(item.As<float>());
            }
            else if (item.VariantType == Variant.Type.String)
            {
                newArray.Add(item.ToString());
            }
            else
            {
                newArray.Add(item);
            }
        }
        return newArray;
    }
    
    private static System.Collections.Generic.Dictionary<string, object> JsonElementToDictionary(JsonElement element)
    {
        var dict = new System.Collections.Generic.Dictionary<string, object>();
        foreach (var property in element.EnumerateObject())
        {
            var key = property.Name;
            var value = property.Value;
            if (value.ValueKind == JsonValueKind.Object)
            {
                dict[key] = JsonElementToDictionary(value);
            }
            else if (value.ValueKind == JsonValueKind.Array)
            {
                dict[key] = JsonElementToList(value);
            }
            else
            {
                dict[key] = value;
            }
        }
        return dict;
    }
    
    private static List<object> JsonElementToList(JsonElement element)
    {
        var list = new List<object>();
        foreach (var value in element.EnumerateArray())
        {
            if (value.ValueKind == JsonValueKind.Object)
            {
                list.Add(JsonElementToDictionary(value));
            }
            else if (value.ValueKind == JsonValueKind.Array)
            {
                list.Add(JsonElementToList(value));
            }
            else
            {
                list.Add(value);
            }
        }
        return list;
    }

    public ImporterMesh ToImporterMesh()
    {
        var importerMesh = new ImporterMesh();

        foreach (var kvp in Data)
        {
            if (kvp.Key.StartsWith("surface_") && kvp.Value is System.Collections.Generic.Dictionary<string, object> surfaceData)
            {
                // Create arrays for vertices, indices, and any other attributes
                var arrays = new GdArray();

                // Add vertices
                if (surfaceData.TryGetValue("vertices", out var verticesObj) && verticesObj is List<object> vertexList && vertexList.Count > 0)
                {
                    var vertices = new Vector3[vertexList.Count];
                    for (int i = 0; i < vertexList.Count; i++)
                    {
                        var vertexDict = (System.Collections.Generic.Dictionary<string, object>)vertexList[i];
                        vertices[i] = new Vector3(
                            (float)vertexDict["x"],
                            (float)vertexDict["y"],
                            (float)vertexDict["z"]
                        );
                    }
                    arrays.Resize((int)ArrayMesh.ArrayType.Max);
                    arrays[(int)ArrayMesh.ArrayType.Vertex] = vertices;
                }

                // Add indices
                if (surfaceData.TryGetValue("indices", out var indicesObj) && indicesObj is List<object> indicesList && indicesList.Count > 0)
                {
                    var indices = new int[indicesList.Count];
                    for (int i = 0; i < indicesList.Count; i++)
                    {
                        indices[i] = Convert.ToInt32(indicesList[i]);
                    }
                    arrays[(int)ArrayMesh.ArrayType.Index] = indices;
                }

                // Add other surface data as needed (e.g., normals, UVs)
                
                if (surfaceData.TryGetValue("normals", out var normalsObj) && normalsObj is List<object> normalList && normalList.Count > 0)
                {
                    var normals = new Vector3[normalList.Count];
                    for (int i = 0; i < normalList.Count; i++)
                    {
                        var normalDict = (System.Collections.Generic.Dictionary<string, object>)normalList[i];
                        normals[i] = new Vector3(
                            (float)normalDict["x"],
                            (float)normalDict["y"],
                            (float)normalDict["z"]
                        );
                    }
                    arrays[(int)ArrayMesh.ArrayType.Normal] = normals;
                }
                
                if (surfaceData.TryGetValue("texUv", out var texUvObj) && texUvObj is List<object> texUvList && texUvList.Count > 0)
                {
                    var texUv = new Vector2[texUvList.Count];
                    for (int i = 0; i < texUvList.Count; i++)
                    {
                        var uvDict = (System.Collections.Generic.Dictionary<string, object>)texUvList[i];
                        texUv[i] = new Vector2(
                            (float)uvDict["x"],
                            (float)uvDict["y"]
                        );
                    }
                    arrays[(int)ArrayMesh.ArrayType.TexUV] = texUv;
                }
                
                if (surfaceData.TryGetValue("texUv2", out var texUv2Obj) && texUv2Obj is List<object> texUv2List && texUv2List.Count > 0)
                {
                    var texUv2 = new Vector2[texUv2List.Count];
                    for (int i = 0; i < texUv2List.Count; i++)
                    {
                        var uvDict = (System.Collections.Generic.Dictionary<string, object>)texUv2List[i];
                        texUv2[i] = new Vector2(
                            (float)uvDict["x"],
                            (float)uvDict["y"]
                        );
                    }
                    arrays[(int)ArrayMesh.ArrayType.TexUV2] = texUv2;
                }
                
                if (surfaceData.TryGetValue("tangents", out var tangentsObj) && tangentsObj is List<object> tangentsList && tangentsList.Count > 0)
                {
                    var tangents = new float[tangentsList.Count];
                    for (int i = 0; i < tangentsList.Count; i++)
                    {
                        tangents[i] = Convert.ToSingle(tangentsList[i]);
                    }
                    arrays[(int)ArrayMesh.ArrayType.Tangent] = tangents;
                }
                
                if (surfaceData.TryGetValue("colors", out var colorsObj) && colorsObj is List<object> colorsList && colorsList.Count > 0)
                {
                    var colors = new Color[colorsList.Count];
                    for (int i = 0; i < colorsList.Count; i++)
                    {
                        var colorDict = (System.Collections.Generic.Dictionary<string, object>)colorsList[i];
                        colors[i] = new Color(
                            (float)colorDict["r"],
                            (float)colorDict["g"],
                            (float)colorDict["b"],
                            (float)colorDict["a"]
                        );
                    }
                    arrays[(int)ArrayMesh.ArrayType.Color] = colors;
                }
                
                if (surfaceData.TryGetValue("custom0", out var custom0Obj) && custom0Obj is string custom0Base64 && custom0Base64.Length > 0)
                {
                    var custom0bytes = Convert.FromBase64String(custom0Base64);
                    arrays[(int)ArrayMesh.ArrayType.Custom0] = custom0bytes;
                }
                
                if (surfaceData.TryGetValue("custom1", out var custom1Obj) && custom1Obj is string custom1Base64 && custom1Base64.Length > 0)
                {
                    var custom1bytes = Convert.FromBase64String(custom1Base64);
                    arrays[(int)ArrayMesh.ArrayType.Custom1] = custom1bytes;
                }
                
                if (surfaceData.TryGetValue("custom2", out var custom2Obj) && custom2Obj is string custom2Base64 && custom2Base64.Length > 0)
                {
                    var custom2bytes = Convert.FromBase64String(custom2Base64);
                    arrays[(int)ArrayMesh.ArrayType.Custom2] = custom2bytes;
                }
                
                if (surfaceData.TryGetValue("custom3", out var custom3Obj) && custom3Obj is string custom3Base64 && custom3Base64.Length > 0)
                {
                    var custom3bytes = Convert.FromBase64String(custom3Base64);
                    arrays[(int)ArrayMesh.ArrayType.Custom3] = custom3bytes;
                }
                
                if (surfaceData.TryGetValue("bones", out var bonesObj) && bonesObj is List<object> bonesList && bonesList.Count > 0)
                {
                    var bones = new float[bonesList.Count];
                    for (int i = 0; i < bonesList.Count; i++)
                    {
                        bones[i] = Convert.ToSingle(bonesList[i]);
                    }
                    arrays[(int)ArrayMesh.ArrayType.Bones] = bones;
                }

                // Create a surface from the arrays
                importerMesh.AddSurface(Mesh.PrimitiveType.Triangles, arrays);
            }
        }

        return importerMesh;
    }
    
    public ArrayMesh ToArrayMesh()
    {
        var arrayMesh = new ArrayMesh();
    
        foreach (var kvp in Data)
        {
            if (kvp.Key.StartsWith("surface_") && kvp.Value is System.Collections.Generic.Dictionary<string, object> surfaceData && surfaceData.Count > 0)
            {
                var arrays = new GdArray();
                arrays.Resize((int)ArrayMesh.ArrayType.Max);

                // Add vertices
                if (surfaceData.TryGetValue("vertices", out var verticesObj) && verticesObj is List<object> vertexList && vertexList.Count > 0)
                {
                    var vertices = new Vector3[vertexList.Count];
                    for (int i = 0; i < vertexList.Count; i++)
                    {
                        var vertexDict = (System.Collections.Generic.Dictionary<string, object>)vertexList[i];
                        vertices[i] = new Vector3(
                            (float)vertexDict["x"],
                            (float)vertexDict["y"],
                            (float)vertexDict["z"]
                        );
                    }
                    arrays[(int)ArrayMesh.ArrayType.Vertex] = vertices;
                }

                // Add indices
                if (surfaceData.TryGetValue("indices", out var indicesObj) && indicesObj is List<object> indicesList && indicesList.Count > 0)
                {
                    var indices = new int[indicesList.Count];
                    for (int i = 0; i < indicesList.Count; i++)
                    {
                        indices[i] = Convert.ToInt32(indicesList[i]);
                    }
                    arrays[(int)ArrayMesh.ArrayType.Index] = indices;
                }
                
                if (surfaceData.TryGetValue("normals", out var normalsObj) && normalsObj is List<object> normalList && normalList.Count > 0)
                {
                    var normals = new Vector3[normalList.Count];
                    for (int i = 0; i < normalList.Count; i++)
                    {
                        var normalDict = (System.Collections.Generic.Dictionary<string, object>)normalList[i];
                        normals[i] = new Vector3(
                            (float)normalDict["x"],
                            (float)normalDict["y"],
                            (float)normalDict["z"]
                        );
                    }
                    arrays[(int)ArrayMesh.ArrayType.Normal] = normals;
                }
                
                if (surfaceData.TryGetValue("texUv", out var texUvObj) && texUvObj is List<object> texUvList && texUvList.Count > 0)
                {
                    var texUv = new Vector2[texUvList.Count];
                    for (int i = 0; i < texUvList.Count; i++)
                    {
                        var uvDict = (System.Collections.Generic.Dictionary<string, object>)texUvList[i];
                        texUv[i] = new Vector2(
                            (float)uvDict["x"],
                            (float)uvDict["y"]
                        );
                    }
                    arrays[(int)ArrayMesh.ArrayType.TexUV] = texUv;
                }
                
                if (surfaceData.TryGetValue("texUv2", out var texUv2Obj) && texUv2Obj is List<object> texUv2List && texUv2List.Count > 0)
                {
                    var texUv2 = new Vector2[texUv2List.Count];
                    for (int i = 0; i < texUv2List.Count; i++)
                    {
                        var uvDict = (System.Collections.Generic.Dictionary<string, object>)texUv2List[i];
                        texUv2[i] = new Vector2(
                            (float)uvDict["x"],
                            (float)uvDict["y"]
                        );
                    }
                    arrays[(int)ArrayMesh.ArrayType.TexUV2] = texUv2;
                }
                
                if (surfaceData.TryGetValue("tangents", out var tangentsObj) && tangentsObj is List<object> tangentsList && tangentsList.Count > 0)
                {
                    var tangents = new float[tangentsList.Count];
                    for (int i = 0; i < tangentsList.Count; i++)
                    {
                        tangents[i] = Convert.ToSingle(tangentsList[i]);
                    }
                    arrays[(int)ArrayMesh.ArrayType.Tangent] = tangents;
                }
                
                if (surfaceData.TryGetValue("colors", out var colorsObj) && colorsObj is List<object> colorsList && colorsList.Count > 0)
                {
                    var colors = new Color[colorsList.Count];
                    for (int i = 0; i < colorsList.Count; i++)
                    {
                        var colorDict = (System.Collections.Generic.Dictionary<string, object>)colorsList[i];
                        colors[i] = new Color(
                            (float)colorDict["r"],
                            (float)colorDict["g"],
                            (float)colorDict["b"],
                            (float)colorDict["a"]
                        );
                    }
                    arrays[(int)ArrayMesh.ArrayType.Color] = colors;
                }
                
                if (surfaceData.TryGetValue("custom0", out var custom0Obj) && custom0Obj is string custom0Base64 && custom0Base64.Length > 0)
                {
                    var custom0bytes = Convert.FromBase64String(custom0Base64);
                    arrays[(int)ArrayMesh.ArrayType.Custom0] = custom0bytes;
                }
                
                if (surfaceData.TryGetValue("custom1", out var custom1Obj) && custom1Obj is string custom1Base64 && custom1Base64.Length > 0)
                {
                    var custom1bytes = Convert.FromBase64String(custom1Base64);
                    arrays[(int)ArrayMesh.ArrayType.Custom1] = custom1bytes;
                }
                
                if (surfaceData.TryGetValue("custom2", out var custom2Obj) && custom2Obj is string custom2Base64 && custom2Base64.Length > 0)
                {
                    var custom2bytes = Convert.FromBase64String(custom2Base64);
                    arrays[(int)ArrayMesh.ArrayType.Custom2] = custom2bytes;
                }
                
                if (surfaceData.TryGetValue("custom3", out var custom3Obj) && custom3Obj is string custom3Base64 && custom3Base64.Length > 0)
                {
                    var custom3bytes = Convert.FromBase64String(custom3Base64);
                    arrays[(int)ArrayMesh.ArrayType.Custom3] = custom3bytes;
                }
                
                if (surfaceData.TryGetValue("bones", out var bonesObj) && bonesObj is List<object> bonesList && bonesList.Count > 0)
                {
                    var bones = new float[bonesList.Count];
                    for (int i = 0; i < bonesList.Count; i++)
                    {
                        bones[i] = Convert.ToSingle(bonesList[i]);
                    }
                    arrays[(int)ArrayMesh.ArrayType.Bones] = bones;
                }

                // Add surface to ArrayMesh
                arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
            }
        }

        return arrayMesh;
    }
}