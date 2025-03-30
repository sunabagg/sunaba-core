using System.Reflection;
using Godot;
using System.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using ToSnakeCaseGd = Godot.StringExtensions;
using MoonSharp.Interpreter;

namespace Sunaba.Core;

public partial class UiDocument : Control
{
    private IoInterface _ioInterface;


    public List<UiStyleSheet> StyleSheets = new List<UiStyleSheet>();

    public Dictionary<string, object> Globals;

    private List<string> Scripts;

    [Signal]
    public delegate void OnDisposedEventHandler();

    public UiDocument(IoInterface ioInterface)
    {
        _ioInterface = ioInterface;
        Globals = new Dictionary<string, object>();
        Scripts = new List<string>();
    }

    public static UiDocument LoadFromPath(IoInterface ioInterface, string path)
    {
        var doc = new UiDocument(ioInterface);
        doc.Load(path);
        return doc;
    }

    public void Load(string path)
    {
        string xml = _ioInterface.LoadText(path);
        try
        {
            LoadFromString(xml);
        }
        catch (Exception e)
        {
            throw new Exception("BXML Parse Error in " + path + " : " + e.Message + " : " + e.StackTrace);
        }
    }

    public void LoadFromString(string xml)
    {
        XmlDocument document = new XmlDocument();
        document.LoadXml(xml);
        Instantiate(document);
    }

    public bool IsValidXml(string xml)
    {
        XmlDocument document = new XmlDocument();
        try
        {
            document.LoadXml(xml);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void Instantiate(XmlDocument document)
    {
        foreach (var child in GetChildren())
        {
            child.QueueFree();
        }

        XmlNode firstChild = document.GetElementsByTagName("BXML")[0];
        if (firstChild.Name == "BXML")
        {
            XmlAttribute Version = firstChild.Attributes["Version"];
            string version;
            if (Version != null)
            {
                version = Version.Value;
            }
            else
            {
                version = "1.0";
            }
            XmlAttribute FullScreen = firstChild.Attributes["FullScreen"];
            if (FullScreen != null)
            {
                bool fullScreen = bool.Parse(FullScreen.Value);
                if (fullScreen)
                {
                    SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect, LayoutPresetMode.KeepSize);
                    SizeFlagsHorizontal = SizeFlags.ExpandFill;
                    SizeFlagsVertical = SizeFlags.ExpandFill;
                }
            }
            else
            {
                XmlAttribute anchorTop = firstChild.Attributes["AnchorTop"];
                if (anchorTop != null)
                {
                    AnchorTop = anchorTop.Value.ParseMacrosToFloat(_ioInterface);
                }
                XmlAttribute anchorLeft = firstChild.Attributes["AnchorLeft"];
                if (anchorLeft != null)
                {
                    AnchorLeft = anchorLeft.Value.ParseMacrosToFloat(_ioInterface);
                }
                XmlAttribute anchorRight = firstChild.Attributes["AnchorRight"];
                if (anchorRight != null)
                {
                    AnchorRight = anchorRight.Value.ParseMacrosToFloat(_ioInterface);
                }
                XmlAttribute anchorBottom = firstChild.Attributes["AnchorBottom"];
                if (anchorBottom != null)
                {
                    AnchorBottom = anchorBottom.Value.ParseMacrosToFloat(_ioInterface);
                }

                XmlAttribute offsetTop = firstChild.Attributes["OffsetTop"];
                if (offsetTop != null)
                {
                    OffsetTop = offsetTop.Value.ParseMacrosToFloat(_ioInterface);
                }
                XmlAttribute offsetLeft = firstChild.Attributes["OffsetLeft"];
                if (offsetLeft != null)
                {
                    OffsetLeft = offsetLeft.Value.ParseMacrosToFloat(_ioInterface);
                }
                XmlAttribute offsetRight = firstChild.Attributes["OffsetRight"];
                if (offsetRight != null)
                {
                    OffsetRight = offsetRight.Value.ParseMacrosToFloat(_ioInterface);
                }
                XmlAttribute offsetBottom = firstChild.Attributes["OffsetBottom"];
                if (offsetBottom != null)
                {
                    OffsetBottom = offsetBottom.Value.ParseMacrosToFloat(_ioInterface);
                }
            }
            XmlNodeList childNodes = firstChild.ChildNodes;
            List<string> scriptPaths = new List<string>();
            foreach (XmlNode node in childNodes)
            {
                if (node.Name == "Head")
                {
                    foreach (XmlNode headChildNode in node.ChildNodes)
                    {
                        if (headChildNode.Name == "Document")
                        {
                            if (headChildNode.Attributes["Path"] != null && headChildNode.Attributes["Type"] != null)
                            {
                                var documentPath = headChildNode.Attributes["Path"].Value;
                                var type = headChildNode.Attributes["Type"].Value;
                                if (type == "StyleSheet")
                                {
                                    var styleSheet = UiStyleSheet.LoadFromPath(documentPath, _ioInterface);
                                    StyleSheets.Add(styleSheet);
                                }
                                else if (type == "Script")
                                {
                                    scriptPaths.Add(documentPath);
                                }
                            }


                        }
                        else if (headChildNode.Name == "Style")
                        {
                            var style = headChildNode.InnerText;
                            var styleSheet = UiStyleSheet.Load(style, _ioInterface);
                            StyleSheets.Add(styleSheet);
                        }
                        else if (headChildNode.Name == "Script")
                        {
                            var script = headChildNode.InnerText;
                            Scripts.Add(script);

                        }
                        else if (headChildNode.Name == "Name")
                        {
                            var title = headChildNode.InnerText;
                            Name = title;
                        }
                    }
                }
                else if (node.Name == "Body")
                {
                    ConstructNodes(version, node.ChildNodes);
                }
            }
        }
    }

    private void ConstructNodes(string version, XmlNodeList nodes)
    {
        if (version == "1.0")
        {
            foreach (XmlNode node in nodes)
            {
                Node gdNode = Construct(node, _ioInterface);
                if (gdNode != null)
                {
                    AddChild(gdNode);
                }
            }
        }
    }

    public object GetObject(string name)
    {
        var node = GetNode(name);
        var type = node.GetType();
        if (!UserData.IsTypeRegistered(type))
            UserData.RegisterType(type);
        return node;
    }

    public Node Construct(XmlNode node, IoInterface assetIo)
    {
        if (node.Name == "Script")
        {
            var script = node.InnerText;
            Scripts.Add(script);
            return null;
        }
        else if (node.Name == "Template")
        {
            var template = new UiDocument(_ioInterface);
            var templatePath = node.Attributes["Path"].Value;
            template.Load(templatePath);
            return template;
        }
        else
        {
            Node gdNode;
            if (node.Name == "Control")
            {
                gdNode = new Control();
                if (gdNode is Control control)
                {
                    control.SetAnchorsPreset(Control.LayoutPreset.FullRect);
                }

                SetObjectValues(node, gdNode);
                SetStyles(node, (Control)gdNode);
                ConstructChildren(node, gdNode);
                return gdNode;
            }
            if (node.Name == "TabManager")
            {
                gdNode = new TabManager();
                if (gdNode is Control control)
                {
                    control.SetAnchorsPreset(Control.LayoutPreset.FullRect);
                }

                SetObjectValues(node, (TabManager)gdNode);
                SetStyles(node, (TabManager)gdNode);
                ConstructChildren(node, gdNode);
                return gdNode;
            }
            else
            {
                foreach (Type type in GetTypesInheritedFrom(typeof(Control)))
                {
                    if (node.Name == type.Name)
                    {
                        gdNode = (Node)Activator.CreateInstance(type);
                        if (gdNode is Control control)
                        {
                            control.SetAnchorsPreset(Control.LayoutPreset.FullRect);
                        }

                        SetObjectValues(node, gdNode);
                        SetStyles(node, (Control)gdNode);
                        ConstructChildren(node, gdNode);
                        return gdNode;
                    }
                }

                foreach (Type type in GetTypesInheritedFrom(typeof(Viewport)))
                {
                    if (node.Name == type.Name)
                    {
                        gdNode = (Node)Activator.CreateInstance(type);
                        SetObjectValues(node, gdNode);
                        if (gdNode is PopupMenu popupMenu)
                        {
                            ConstructMenu(node, popupMenu);
                        }
                        else
                        {
                            ConstructChildren(node, gdNode);
                        }
                        return gdNode;
                    }
                }
            }
            return null;
        }
    }

    public static Type[] GetTypesInheritedFrom(Type baseType)
    {
        Assembly assembly = baseType.Assembly;
        Type[] types = assembly.GetTypes();
        Type[] inheritedTypes = types.Where(t => t.IsSubclassOf(baseType)).ToArray();
        return inheritedTypes;
    }

    public void SetStyles(XmlNode node, Control control)
    {
        foreach (var attribute in node.Attributes)
        {
            var attr = (XmlAttribute)attribute;
            var styleName = attr.Name.ToSnakeCase();
            if (control.HasThemeStyleboxOverride(styleName))
            {
                foreach (UiStyleSheet styleSheet in StyleSheets)
                {
                    if (styleSheet.Styles.ContainsKey(attr.Value))
                    {
                        StyleBox styleBox = styleSheet.Styles[attr.Value];
                        control.AddThemeStyleboxOverride(styleName, styleBox);
                    }
                }
            }
            else if (control.HasThemeConstantOverride(styleName))
            {
                control.AddThemeConstantOverride(styleName, attr.Value.ParseMacrosToInt(_ioInterface));
            }
            else if (control.HasThemeFontSizeOverride(styleName))
            {
                control.AddThemeFontSizeOverride(styleName, attr.Value.ParseMacrosToInt(_ioInterface));
            }
            else if (control.HasThemeColorOverride(styleName))
            {
                control.AddThemeColorOverride(styleName, ToColor(attr.Value));
            }
            else if (control.HasThemeFontOverride(styleName))
            {
                //control.AddThemeFontOverride(styleName, attr.Value);
            }
            else if (control.HasThemeIconOverride(styleName))
            {
                control.AddThemeIconOverride(styleName, LoadImageTexture(_ioInterface, attr.Value.ParseMacros()));
            }
        }
    }

    public void SetObjectValues(XmlNode node, object obj)
    {
        var properties = obj.GetType().GetProperties();
        foreach (var attribute in node.Attributes)
        {
            // hack to set the ShowCloseButton property of the TabManager
            if (attribute is XmlAttribute attr)
                if (attr.Name == "ShowCloseButton")
                {
                    var showCloseButton = bool.Parse(attr.Value);
                    if (obj is TabManager tabManager)
                    {
                        tabManager.ShowCloseButton = showCloseButton;
                    }
                }
            SetObjectValue(attribute, obj, properties);
        }
    }



    public void SetObjectValue(object attribute, object obj, PropertyInfo[] properties)
    {
        var attr = (XmlAttribute)attribute;
        foreach (var property in properties)
        {
            if (property.Name == attr.Name)
            {
                if (property.PropertyType == typeof(string))
                {
                    property.SetValue(obj, attr.Value.ParseMacros(_ioInterface));
                }

                if (property.PropertyType == typeof(StringName))
                {
                    property.SetValue(obj, new StringName(attr.Value.ParseMacros(_ioInterface)));
                }
                else if (property.PropertyType == typeof(int))
                {
                    property.SetValue(obj, attr.Value.ParseMacrosToInt(_ioInterface));
                }
                else if (property.PropertyType == typeof(float))
                {
                    property.SetValue(obj, attr.Value.ParseMacrosToFloat(_ioInterface));
                }
                else if (property.PropertyType == typeof(Vector2))
                {
                    property.SetValue(obj, ToVector2(attr.Value, _ioInterface));
                }
                else if (property.PropertyType == typeof(Vector3))
                {
                    property.SetValue(obj, ToVector3(attr.Value, _ioInterface));
                }
                else if (property.PropertyType == typeof(Vector4))
                {
                    property.SetValue(obj, ToVector4(attr.Value, _ioInterface));
                }
                else if (property.PropertyType == typeof(Vector2I))
                {
                    property.SetValue(obj, ToVector2I(attr.Value, _ioInterface));
                }
                else if (property.PropertyType == typeof(Vector3I))
                {
                    property.SetValue(obj, ToVector3I(attr.Value, _ioInterface));
                }
                else if (property.PropertyType == typeof(Color))
                {
                    property.SetValue(obj, ToColor(attr.Value));
                }
                else if (property.PropertyType == typeof(long))
                {
                    property.SetValue(obj, attr.Value.ParseMacrosToLong(_ioInterface));
                }
                else if (property.PropertyType == typeof(double))
                {
                    property.SetValue(obj, attr.Value.ParseMacrosToDouble(_ioInterface));
                }
                else if (property.PropertyType == typeof(NodePath))
                {
                    property.SetValue(obj, ToNodePath(attr.Value));
                }
                else if (property.PropertyType == typeof(Texture2D))
                {
                    property.SetValue(obj, LoadImageTexture(_ioInterface, attr.Value.ParseMacros()));
                }
                else if (property.PropertyType == typeof(Texture))
                {
                    property.SetValue(obj, LoadImageTexture(_ioInterface, attr.Value.ParseMacros()));
                }
                else if (property.PropertyType == typeof(ImageTexture))
                {
                    property.SetValue(obj, LoadImageTexture(_ioInterface, attr.Value.ParseMacros()));
                }
                else if (property.PropertyType == typeof(bool))
                {
                    property.SetValue(obj, bool.Parse(attr.Value));
                }
                else if (property.PropertyType == typeof(StyleBox))
                {


                }
                else if (property.PropertyType.IsEnum)
                {
                    var a = property.PropertyType.GetCustomAttribute<System.FlagsAttribute>();
                    if (a != null)
                    {
                        // The enum has the FlagsAttribute, so it can have multiple flags
                        var enumValue = (Enum)Enum.Parse(property.PropertyType, attr.Value);
                        // Set the property value
                        property.SetValue(obj, enumValue);
                    }
                    else
                    {
                        property.SetValue(obj, ToEnum(attr.Value, property.PropertyType));
                    }
                }
            }
        }
    }

    public void ConstructMenu(XmlNode node, PopupMenu popupMenu)
    {
        foreach (var c in node.ChildNodes)
        {
            var childNode = (XmlNode)c;
            if (childNode.Name == "MenuItem")
            {
                if (childNode.Attributes != null && childNode.Attributes["Label"] != null)
                {
                    var label = childNode.Attributes["Label"].Value;
                    var id = popupMenu.ItemCount;
                    popupMenu.AddItem(label, id);
                    if (childNode.Attributes["Icon"] != null)
                    {
                        var icon = LoadImageTexture(_ioInterface, childNode.Attributes["Icon"].Value);
                        popupMenu.SetItemIcon(id, icon);
                    }
                    if (childNode.Attributes["Checkable"] != null)
                    {
                        var checkable = bool.Parse(childNode.Attributes["Checkable"].Value);
                        popupMenu.SetItemAsCheckable(id, checkable);
                    }
                    else if (childNode.Attributes["RadioCheckable"] != null)
                    {
                        var checkable = bool.Parse(childNode.Attributes["RadioCheckable"].Value);
                        popupMenu.SetItemAsRadioCheckable(id, checkable);
                    }
                    else if (childNode.Attributes["Separator"] != null)
                    {
                        var separator = bool.Parse(childNode.Attributes["Separator"].Value);
                        popupMenu.SetItemAsSeparator(id, separator);
                    }
                    if (childNode.Attributes["Checked"] != null)
                    {
                        var disabled = bool.Parse(childNode.Attributes["Checked"].Value);
                        popupMenu.SetItemChecked(id, disabled);
                    }
                    if (childNode.Attributes["Disabled"] != null)
                    {
                        var disabled = bool.Parse(childNode.Attributes["Disabled"].Value);
                        popupMenu.SetItemDisabled(id, disabled);
                    }

                }

            }
        }
    }

    public void ConstructChildren(XmlNode node, Node n)
    {
        foreach (var c in node.ChildNodes)
        {
            var childNode = (XmlNode)c;
            var child = Construct(childNode, _ioInterface);
            if (child != null)
            {
                n.AddChild(child);
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



    private int ToEnum(string str, Type t)
    {
        var en = Enum.Parse(t, str);
        return Convert.ToInt32(en);
    }


    private NodePath ToNodePath(string str)
    {
        return new NodePath(str);
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

    public override void _Process(double delta)
    {
        Theme t = GetWindow().Theme;
        if (Theme != t)
        {
            Theme = t;
        }
    }

    public override void _ExitTree()
    {
        DisposeDocument();
    }

    public void DisposeDocument()
    {
        EmitSignal(SignalName.OnDisposed);
    }
}