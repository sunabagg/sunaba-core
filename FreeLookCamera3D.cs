using Godot;
using System;

namespace Sunaba.Core;

[GlobalClass]
public partial class FreeLookCamera3D : Camera3D
{
    public double Sensitivity = 3;
    public double ControllerSensitivity = 20;
    public double DefaultVelocity = 5;
    public double SpeedScale = 1.17;
    public double BoostSpeedMultiplier = 3.0;
    public double MaxSpeed = 1000;
    public double MinSpeed = 0.2;

    public double Velocity;

    public Vector3 InitialPosition;
    public Vector3 InitialRotation;

    public bool Active = true;

    bool IsJoystickActive = false;

    public Node3D TransformNode;

    bool speedToggle = false;

    public FreeLookCamera3D()
    {
        TransformNode = this;
    }

    public override void _Ready()
    {
        Velocity = DefaultVelocity;
        InitialPosition = TransformNode.GlobalPosition;
        InitialRotation = TransformNode.GlobalRotation;
    }

    public override void _Input(InputEvent @event)
    {
        if ((!Current) || (!Active) || ((!Current) && (!Active)))
            return;
        if (!Current)
            return;
        if (!Active)
            return;
        if (GetWindow().HasFocus() == false)
            return;
        if (TransformNode == null)
            return;

        if (Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            if (@event is InputEventMouseMotion eventMouseMotion)
            {
                Vector3 rotation = TransformNode.Rotation;
                rotation.Y -= eventMouseMotion.Relative.X / 1000 * (float)Sensitivity;
                rotation.X -= eventMouseMotion.Relative.Y / 1000 * (float)Sensitivity;
                rotation.X = (float)Mathf.Clamp(rotation.X, Math.PI / -2, Math.PI / 2);
                TransformNode.Rotation = rotation;
            }
        }
        if (@event is InputEventJoypadMotion && Input.GetJoyAxis(0, 0) != 0)
        {
            Vector3 rotation = TransformNode.Rotation;
            rotation.Y -= Input.GetJoyAxis(0, JoyAxis.RightX) / 1000 * (float)ControllerSensitivity;
            rotation.X -= Input.GetJoyAxis(0, JoyAxis.RightY) / 1000 * (float)ControllerSensitivity;
            rotation.X = (float)Mathf.Clamp(rotation.X, Math.PI / -2, Math.PI / 2);
            TransformNode.Rotation = rotation;
            IsJoystickActive = true;
        }
        else
            IsJoystickActive = false;
        if (@event is InputEventJoypadButton)
        {
            if (Input.IsJoyButtonPressed(0, JoyButton.DpadUp))
                Velocity = Mathf.Clamp(Velocity * SpeedScale, MinSpeed, MaxSpeed);
            else if (Input.IsJoyButtonPressed(0, JoyButton.DpadDown))
                Velocity = Mathf.Clamp(Velocity / SpeedScale, MinSpeed, MaxSpeed);
        }

        if (@event is InputEventMouseButton eventMouseButton)
        {
            switch (eventMouseButton.ButtonIndex)
            {
                case MouseButton.Right:
                    if (eventMouseButton.Pressed)
                    {
                        Input.MouseMode = Input.MouseModeEnum.Captured;
                    }
                    else
                    {
                        Input.MouseMode = Input.MouseModeEnum.Visible;
                    }
                    break;
                case MouseButton.WheelUp:
                    Velocity = Mathf.Clamp(Velocity * SpeedScale, MinSpeed, MaxSpeed);
                    break;
                case MouseButton.WheelDown:
                    Velocity = Mathf.Clamp(Velocity / SpeedScale, MinSpeed, MaxSpeed);
                    break;
            }
        }
    }

    public override void _Process(double delta)
    {
        if ((!Current) || (!Active) || ((!Current) && (!Active)))
            return;
        if (!Current)
            return;
        if (!Active)
            return;
        if (GetWindow().HasFocus() == false)
            return;
        if (TransformNode == null)
            return;

        Vector3 direction = new Vector3(
            GetAxis(Input.IsPhysicalKeyPressed(Key.D), Input.IsPhysicalKeyPressed(Key.A)),
            GetAxis(Input.IsPhysicalKeyPressed(Key.E), Input.IsPhysicalKeyPressed(Key.Q)),
            GetAxis(Input.IsPhysicalKeyPressed(Key.S), Input.IsPhysicalKeyPressed(Key.W))
        ).Normalized();

        Vector3 joypadDirection = new Vector3(
            RoundAxis(Input.GetJoyAxis(0, JoyAxis.LeftX)),
            GetCombinedAxis(Input.GetJoyAxis(0, JoyAxis.TriggerRight), Input.GetJoyAxis(0, JoyAxis.TriggerLeft)),
            RoundAxis(Input.GetJoyAxis(0, JoyAxis.LeftY))
        ).Normalized();

        if (Input.IsJoyButtonPressed(0, JoyButton.LeftStick))
            speedToggle = !speedToggle;

        if (speedToggle && IsJoystickActive)
            TransformNode.Translate(joypadDirection * (float)Velocity * (float)delta * (float)BoostSpeedMultiplier);
        else if (IsJoystickActive)
            TransformNode.Translate(joypadDirection * (float)Velocity * (float)delta);
        else if (Input.IsPhysicalKeyPressed(Key.Shift))
            TransformNode.Translate(direction * (float)Velocity * (float)delta * (float)BoostSpeedMultiplier);
        else
            TransformNode.Translate(direction * (float)Velocity * (float)delta);

        if (Input.IsPhysicalKeyPressed(Key.Shift) && Input.IsKeyPressed(Key.R))
            Reset();
    }

    public void Reset()
    {
        TransformNode.GlobalPosition = InitialPosition;
        TransformNode.GlobalRotation = InitialRotation;
    }

    public float GetAxis(bool bool1, bool bool2)
    {
        float float1 = System.Convert.ToSingle(bool1);
        float float2 = System.Convert.ToSingle(bool2);
        return float1 - float2;
    }

    public float GetCombinedAxis(float axis1, float axis2)
    {
        return axis1 - axis2;
    }

    public float RoundAxis(float axis)
    {
        return Mathf.Round(axis);
    }
}