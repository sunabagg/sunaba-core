using Godot;

namespace Sunaba.Core;

public partial class ConfirmationDialogPlus : ConfirmationDialog
{
    public enum TypeEnum
    {
        Error,
        Warning,
        Info,
    }
    
    private TypeEnum _type = TypeEnum.Info;
    public TypeEnum Type
    {
        get => _type;
    }
    
    private HBoxContainer _hBoxContainer;
    
    public Label Label;

    public string Text
    {
        get => Label.Text;
        set => Label.Text = value;
    }
    
    public ConfirmationDialogPlus(TypeEnum type)
    {
        _type = type;
        PackedScene boxScene = GD.Load<PackedScene>("res://ErrorBox.tscn");
        if (_type == TypeEnum.Warning)
        {
            boxScene = GD.Load<PackedScene>("res://WarnBox.tscn");
        }
        else if (_type == TypeEnum.Info)
        {
            boxScene = GD.Load<PackedScene>("res://InfoBox.tscn");
        }

        _hBoxContainer = (HBoxContainer)boxScene.Instantiate<HBoxContainer>();
        AddChild(_hBoxContainer);
        Label = (Label)_hBoxContainer.GetNode("Label");
    }
}