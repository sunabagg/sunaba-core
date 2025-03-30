using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Godot;

namespace Sunaba.Core;

public class UiStyleSheet
{
    private IoInterface _ioInterface;
    
    public string Source;
    
    public Theme Theme;
    
    public Dictionary<string, StyleBox> Styles = new();
    
    public static UiStyleSheet LoadFromPath(string path, IoInterface assetIo)
    {
        var styleSheet = assetIo.LoadText(path);
        try
        {
            return UiStyleSheet.Load(styleSheet, assetIo);
        }
        catch (Exception e)
        {
            throw new Exception("SBSS Parse Error in " + path + ": " + e.Message);
        }
        return null;
    }
    
    public static UiStyleSheet Load(string styleSheet, IoInterface assetIo)
    {
        UiStyleSheet uiStyleSheet = new UiStyleSheet();
        uiStyleSheet.Source = styleSheet;
        uiStyleSheet._ioInterface = assetIo;
        uiStyleSheet.Parse();
        return uiStyleSheet;
    }
    
    public void Parse()
    {
        var doc = JsonNode.Parse(Source);
        var obj = doc.AsObject();
        foreach (var (key, value) in obj)
        {
            /*if (key == "theme" && value is JsonObject thmObj)
            {
                ParseTheme(thmObj);
            }
            else*/ if (value is JsonObject obj2)
            {
                ParseStyle(obj2, key);
            }
        }
    }
    
    private void ParseTheme(JsonObject obj)
    {
        
    }
    
    private void ParseStyle(JsonObject obj, string name)
    {
        var typeName = obj["Type"].ToString();
        if (typeName == "StyleBoxEmpty")
        {
            StyleBoxEmpty styleBox = new StyleBoxEmpty();
            JsonObjectToStyleBox(obj, styleBox);
            Styles.Add(name, styleBox);
        }
        else if (typeName == "StyleBoxFlat")
        {
            StyleBoxFlat styleBox = new StyleBoxFlat();
            JsonObjectToStyleBox(obj, styleBox);
            Styles.Add(name, styleBox);
        }
        else if (typeName == "StyleBoxLine")
        {
            StyleBoxLine styleBox = new StyleBoxLine();
            JsonObjectToStyleBox(obj, styleBox);
            Styles.Add(name, styleBox);
        }
        else if (typeName == "StyleBoxTexture")
        {
            StyleBoxTexture styleBox = new StyleBoxTexture();
            JsonObjectToStyleBox(obj, styleBox);
            Styles.Add(name, styleBox);
        }
    }

    private void JsonObjectToStyleBox(JsonObject obj, StyleBox styleBox)
    {
        foreach (var (key, value) in obj)
        {
            if (key == "type")
            {
                continue;
            }
            else
            {
                foreach (var property in styleBox.GetType().GetProperties())
                {
                    if (property.Name == key)
                    {
                        if (property.PropertyType == typeof(Color))
                        {
                            property.SetValue(styleBox, ToColor(value.ToString()));
                        }
                        else if (property.PropertyType == typeof(float))
                        {
                            property.SetValue(styleBox, value.ToString().ParseMacrosToFloat());
                        }
                        else if (property.PropertyType == typeof(int))
                        {
                            property.SetValue(styleBox, value.ToString().ParseMacrosToInt());
                        }
                        else if (property.PropertyType == typeof(long))
                        {
                            property.SetValue(styleBox, value.ToString().ParseMacrosToLong());
                        }
                        else if (property.PropertyType == typeof(double))
                        {
                            property.SetValue(styleBox, value.ToString().ParseMacrosToDouble());
                        }
                        else if (property.PropertyType == typeof(bool))
                        {
                            property.SetValue(styleBox, bool.Parse(value.ToString()));
                        }
                        else if (property.PropertyType == typeof(Texture))
                        {
                            property.SetValue(styleBox, LoadImageTexture(_ioInterface, value.ToString()));
                        }
                        else if (property.PropertyType == typeof(ImageTexture))
                        {
                            property.SetValue(styleBox, LoadImageTexture(_ioInterface, value.ToString()));
                        }
                        else if (property.PropertyType == typeof(Texture2D))
                        {
                            property.SetValue(styleBox, LoadImageTexture(_ioInterface, value.ToString()));
                        }
                        else if (property.PropertyType == typeof(Vector2))
                        {
                            property.SetValue(styleBox, ToVector2(value.ToString().ParseMacros()));
                        }
                        else if (property.PropertyType == typeof(Vector3))
                        {
                            property.SetValue(styleBox, ToVector3(value.ToString().ParseMacros()));
                        }
                        else if (property.PropertyType == typeof(Vector4))
                        {
                            property.SetValue(styleBox, ToVector4(value.ToString().ParseMacros()));
                        }
                        else if (property.PropertyType == typeof(Vector2I))
                        {
                            property.SetValue(styleBox, ToVector2I(value.ToString().ParseMacros()));
                        }
                        else if (property.PropertyType == typeof(Vector3I))
                        {
                            property.SetValue(styleBox, ToVector3I(value.ToString().ParseMacros()));
                        }
                        else if (property.PropertyType == typeof(Rect2))
                        {
                            property.SetValue(styleBox, ToRect2(value.ToString().ParseMacros()));
                        }

                        if (property.PropertyType.IsEnum)
                        {
                            var a = property.PropertyType.GetCustomAttribute<System.FlagsAttribute>();
                            if (a != null)
                            {
                                // The enum has the FlagsAttribute, so it can have multiple flags
                                var enumValue = (Enum)Enum.Parse(property.PropertyType, value.ToString());
                                // Set the property value
                                property.SetValue(obj, enumValue);
                            }
                            else
                            {
                                property.SetValue(obj, ToEnum(value.ToString(), property.PropertyType));
                            }
                        }
                    }
                }
            }
        }
    }
    
    private Vector2 ToVector2(string str, IoInterface assetIo = null)
    {
        var parts = str.Split(',');
        var vec2 = new Vector2();//(parts[0].ToFloat(), parts[1].ToFloat());
        float x = parts[0].ParseMacrosToFloat(assetIo);
        float y = parts[1].ParseMacrosToFloat(assetIo);
        vec2.X = x;
        vec2.Y = y;
        return vec2;
    }
    
    private Vector3 ToVector3(string str, IoInterface assetIo = null)
    {
        var parts = str.Split(',');
        var vec3 = new Vector3();//(parts[0].ToFloat(), parts[1].ToFloat(), parts[2].ToFloat());
        float x = parts[0].ParseMacrosToFloat(assetIo);
        float y = parts[1].ParseMacrosToFloat(assetIo);
        float z = parts[2].ParseMacrosToFloat(assetIo);
        vec3.X = x;
        vec3.Y = y;
        vec3.Z = z;
        return vec3;
    }
    
    private Vector4 ToVector4(string str, IoInterface assetIo = null)
    {
        var parts = str.Split(',');
        var vec4 = new Vector4();//(parts[0].ToFloat(), parts[1].ToFloat(), parts[2].ToFloat(), parts[3].ToFloat());
        float x = parts[0].ParseMacrosToFloat(assetIo);
        float y = parts[1].ParseMacrosToFloat(assetIo);
        float z = parts[2].ParseMacrosToFloat(assetIo);
        float w = parts[3].ParseMacrosToFloat(assetIo);
        vec4.X = x;
        vec4.Y = y;
        vec4.Z = z;
        vec4.W = w;
        return vec4;
    }
    
    private Vector2I ToVector2I(string str, IoInterface assetIo = null)
    {
        var parts = str.Split(',');
        var vec2i = new Vector2I();//(parts[0].ToInt(), parts[1].ToInt());
        var x = parts[0].ParseMacrosToInt(assetIo);
        var y = parts[1].ParseMacrosToInt(assetIo);
        vec2i.X = x;
        vec2i.Y = y;
        return vec2i;
    }
    
    private Vector3I ToVector3I(string str, IoInterface assetIo = null)
    {
        var parts = str.Split(',');
        var vec3i = new Vector3I();//(parts[0].ToInt(), parts[1].ToInt(), parts[2].ToInt());
        var x = parts[0].ParseMacrosToInt(assetIo);
        var y = parts[1].ParseMacrosToInt(assetIo);
        var z = parts[2].ParseMacrosToInt(assetIo);
        vec3i.X = x;
        vec3i.Y = y;
        vec3i.Z = z;
        return vec3i;
    }
    
    public Rect2 ToRect2(string str, IoInterface assetIo = null)
    {
        var parts = str.Split(',');
        var rect2 = new Rect2();//(parts[0].ToVector2(assetIo), parts[1].ToVector2(assetIo));
        var pos = ToVector2(parts[0], assetIo);
        var size = ToVector2(parts[1], assetIo);
        rect2.Position = pos;
        rect2.Size = size;
        return rect2;
    }
    
    private int ToEnum(string str, Type t)
    {
        var en = Enum.Parse(t, str);
        return Convert.ToInt32(en);
    }
    
    private Color ToColor(string str)
    {
        if (str.StartsWith("#"))
        {
            var color = new Color(str); //(str.ToColor());
            return color;
        }
        else
        {
            var parts = str.Split(',');
            var color = new Color(); //(parts[0].ToFloat(), parts[1].ToFloat(), parts[2].ToFloat(), parts[3].ToFloat());
            color.R = parts[0].ToFloat();
            color.G = parts[1].ToFloat();
            color.B = parts[2].ToFloat();
            color.A = parts[3].ToFloat();
            return color;
        }
    }
    
    private ImageTexture LoadImageTexture(IoInterface assetIo, string path)
    {
        var buffer = assetIo.LoadBytes(path);
        var image = new Image();
        if (path.EndsWith(".png"))
        {
            image.LoadPngFromBuffer(buffer);
        }
        else if (path.EndsWith(".jpg"))
        {
            image.LoadJpgFromBuffer(buffer);
        }
        else if (path.EndsWith(".bmp"))
        {
            image.LoadBmpFromBuffer(buffer);
        }
        else if (path.EndsWith(".tga"))
        {
            image.LoadTgaFromBuffer(buffer);
        }
        else if (path.EndsWith(".ktx"))
        {
            image.LoadKtxFromBuffer(buffer);
        }
        else if (path.EndsWith(".hdr"))
        {
            image.LoadWebpFromBuffer(buffer);
        }
        else if (path.EndsWith(".svg"))
        {
            image.LoadSvgFromBuffer(buffer);
        }
        var texture = ImageTexture.CreateFromImage(image);
        return texture;
    }
}