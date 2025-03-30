using System;
using Godot;

namespace Sunaba.Core;

public enum CharacterCameraMode
{
    None,
    FirstPerson,
    ThirdPerson
}

public partial class CharacterController : CharacterBody3D
{

    public Node3D Head;
    public Camera3D FirstPersonCamera;
    public SpringArm3D LeftSpringArm;
    public SpringArm3D BackSpringArm;
    public Camera3D ThirdPersonCamera;

    public CapsuleShape3D CapsuleShape;
    public CollisionShape3D CollisionShape;

    public CharacterCameraMode CameraMode = CharacterCameraMode.FirstPerson;
    [Signal]
    public delegate void OnCameraModeChangedEventHandler(CharacterCameraMode mode);

    // Movement Related
    public int DefaultSpeed = 5;
    public int SprintSpeed = 7;
    public int CrouchSpeed = 1;
    public int TimeToCrouch = 20;
    public int Acceleration = 60;
    public int Friction = 50;
    public int AirFriction = 10;
    public int JumpImpulse = 20;
    public int Gravity = -40;
    // Camera Related
    public float MouseSensitivity = 1;
    public int ControllerSensitivity = 30;

    private Vector3 _velocity = new Vector3();

    public Vector3 SnapVector = Vector3.Zero;
    
    
    private Camera3D _camera;
    public Camera3D? Camera
    {
        get
        {
            if (CameraMode == CharacterCameraMode.FirstPerson)
            {
                return FirstPersonCamera;
            }
            else if (CameraMode == CharacterCameraMode.ThirdPerson)
            {
                return ThirdPersonCamera;
            }
            else
            {
                Camera3D camera = _camera;
                if (camera != null)
                {
                    return camera;
                }
                else
                {
                    return null;
                }
            }
        }
        set
        {
            if (CameraMode == CharacterCameraMode.None)
            {
                _camera = value;
            }
        }
    }

    public double DefaultHeight = 1.497;
    public double CrouchHeight = 0.8;
    public double HeadHeight = 1.111;
    public double HeadCrouchHeight = 0.866;
    public int Speed = 5;

    public bool IsOnWater = false;
    
    private bool _isActive = true;

    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            if (_isActive)
            {
                start();
            }
            else
            {
                Input.MouseMode = Input.MouseModeEnum.Visible;
            }
        }
    }

    public CharacterController()
    {
        Head = new Node3D();
        Head.Position = new Vector3(0, (float)HeadHeight, 0);
        AddChild(Head);
        FirstPersonCamera = new Camera3D();
        FirstPersonCamera.Far = 200;
        Head.AddChild(FirstPersonCamera);
        LeftSpringArm = new SpringArm3D();
        LeftSpringArm.SpringLength = 0.5f;
        Head.AddChild(LeftSpringArm);
        BackSpringArm = new SpringArm3D();
        BackSpringArm.SpringLength = 2f;
        LeftSpringArm.AddChild(BackSpringArm);
        ThirdPersonCamera = new Camera3D();
        BackSpringArm.AddChild(ThirdPersonCamera);

        CapsuleShape = new CapsuleShape3D();
        CapsuleShape.Radius = 0.28f;
        CapsuleShape.Height = (float)DefaultHeight;
        CollisionShape = new CollisionShape3D();
        CollisionShape.Shape = CapsuleShape;
        AddChild(CollisionShape);
    }

    public override void _Ready()
    {
        FirstPersonCamera.Current = false;
        ThirdPersonCamera.Current = false;
        Speed = DefaultSpeed;

        InputMap.AddAction("moveForward");
        InputMap.AddAction("moveBackward");
        InputMap.AddAction("moveLeft");
        InputMap.AddAction("moveRight");
        InputMap.AddAction("jump");
        InputMap.AddAction("sprint");
        InputMap.AddAction("crouch");
        InputMap.AddAction("pause");

        if (IsActive)
        {
            start();
        }
    }

    private bool isStarted = false;
    private void start()
    {
        if (isStarted)
            return;
        
        isStarted = true;
        
        
        Input.MouseMode = Input.MouseModeEnum.Captured;
        SetCameraMode(CameraMode);
    }

    public void SetCameraMode(CharacterCameraMode mode)
    {
        CameraMode = mode;
        switch (mode)
        {
            case CharacterCameraMode.FirstPerson:
                FirstPersonCamera.Current = true;
                ThirdPersonCamera.Current = false;
                _camera = null;
                break;
            case CharacterCameraMode.ThirdPerson:
                FirstPersonCamera.Visible = false;
                ThirdPersonCamera.Current = true;
                _camera = null;
                break;
            case CharacterCameraMode.None:
                FirstPersonCamera.Current = false;
                ThirdPersonCamera.Current = false;
                break;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey key)
        {
            if (key.Keycode == Key.W)
            {
                InputMap.ActionAddEvent("moveForward", @event);
            }
            if (key.Keycode == Key.S)
            {
                InputMap.ActionAddEvent("moveBackward", @event);
            }
            if (key.Keycode == Key.A)
            {
                InputMap.ActionAddEvent("moveLeft", @event);
            }
            if (key.Keycode == Key.D)
            {
                InputMap.ActionAddEvent("moveRight", @event);
            }
            if (key.Keycode == Key.Space)
            {
                InputMap.ActionAddEvent("jump", @event);
            }
            if (key.Keycode == Key.Shift)
            {
                InputMap.ActionAddEvent("sprint", @event);
            }
        }

        if (IsActive)
        {
            if (@event is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
            {
                Vector2 mouseAxis = mouseMotion.Relative;
                Vector3 newRotation = Rotation;
                newRotation.Y -= mouseAxis.X * MouseSensitivity * (float).001;
                Rotation = newRotation;
                Vector3 newHeadRotation = Head.Rotation;
                newHeadRotation.X = Mathf.Clamp(newHeadRotation.X - mouseAxis.Y * MouseSensitivity * (float).001, -1.5f, 1.5f);
                Head.Rotation = newHeadRotation;
            }
        }
    }

    public override void _Process(double delta)
    {
        if (IsActive)
        {
            if (Camera != null)
            {
                Camera.Current = true;
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsActive)
        {
            Speed = DefaultSpeed;
            Vector3 InputVector = GetInputVector();
            Vector3 direction = GetDirection(InputVector);
            Jump();

            if (Input.IsActionPressed("sprint"))
            {
                Speed = SprintSpeed;
            }
            
            if (Input.IsActionPressed("crouch") && !Input.IsKeyPressed(Key.Alt))
            {
                Shape3D shape3D = CollisionShape.Shape;
                if (shape3D is CapsuleShape3D capsule)
                {
                    capsule.Height = (float)CrouchHeight;
                }

                Speed = CrouchSpeed;
            }
            else
            {
                Shape3D shape3D = CollisionShape.Shape;
                if (shape3D is CapsuleShape3D capsule)
                {
                    capsule.Height = (float)DefaultHeight;
                }
            }
            
            ApplyMovement(direction, delta);
            ApplyGravity(delta);
            ApplyFriction(direction, delta);
            ApplyControllerRotation();

            UpDirection = Vector3.Up;
            FloorStopOnSlope = true;
            MaxSlides = 4;
            FloorMaxAngle = (float).7853;
            MoveAndSlide();
            ProcessCollisions();
        }
    }

    public void ProcessCollisions()
    {
        for (var i = 0; i < GetSlideCollisionCount(); i++)
        {
            var collision = GetSlideCollision(i);
            for (var k = 0; k < collision.GetCollisionCount(); k++)
            {
                var body = collision.GetCollider(k) as RigidBody3D;
                if (body == null)
                    continue;
                var point = collision.GetPosition(k) - body.GlobalPosition;

                var force = 5; // put whatever force amount you want here

                body.ApplyImpulse(-collision.GetNormal(k) * force, point);
            }
        }
    }
    
    private Vector3 GetInputVector()
    {
        Vector3 InputVector = Vector3.Zero;
        InputVector.X = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        InputVector.Z = Input.GetActionStrength("move_backward") - Input.GetActionStrength("move_forward");
        if (InputVector.Length() > 1)
        {
            return InputVector;
        }
        else
        {
            return InputVector;
        }
    }

    private Vector3 GetDirection(Vector3 InputVector)
    {
        Vector3 direction = Vector3.Zero;
        direction = (InputVector.X * Transform.Basis.X) + (InputVector.Z * Transform.Basis.Z);
        return direction;
    }
    
    private void ApplyMovement(Vector3 direction, double delta)
    {
        if (direction != Vector3.Zero)
        {
            Vector3 velocity = Velocity;
            velocity.X = velocity.MoveToward(direction * Speed, Acceleration * (float)delta).X;
            velocity.Z = velocity.MoveToward(direction * Speed, Acceleration * (float)delta).Z;
            Velocity = velocity;
        }
    }

    private void ApplyFriction(Vector3 direction, double delta)
    {
        if (direction == Vector3.Zero)
        {
            Vector3 velocity = Velocity;
            if (IsOnFloor()) velocity = velocity.MoveToward(Vector3.Zero, Friction * (float)delta);
            else
            {
                velocity.X = velocity.MoveToward(direction * Speed, AirFriction * (float)delta).X;
                velocity.Z = velocity.MoveToward(direction * Speed, AirFriction * (float)delta).Z;
            }
            Velocity = velocity;
        }
    }

    private void ApplyGravity(double delta)
    {
        Vector3 velocity = Velocity;
        velocity.Y += Gravity * (float)delta;
        velocity.Y = Mathf.Clamp(velocity.Y, Gravity, JumpImpulse);
        Velocity = velocity;
    }
    
    private void UpdateSnapVector()
    {
        if (!IsOnFloor())
        {
            SnapVector = GetFloorNormal();
        }
        else
        {
            SnapVector = Vector3.Down;
        }
    }

    public void Jump()
    {
        if ((Input.IsActionJustPressed("jump") && IsOnFloor()))// || (Input.IsActionJustPressed("jump") && timesJumped == 1))
        {
            SnapVector = Vector3.Zero;
            Vector3 velocity = Velocity;
            velocity.Y = JumpImpulse;
            Velocity = velocity;
            TimesJumped += 1;
        }
        else if (Input.IsActionPressed("jump") && IsOnWater)
        {
            //jumpSound.Play();
            SnapVector = Vector3.Zero;
            Vector3 velocity = Velocity;
            velocity.Y = JumpImpulse / 8;
            Velocity = velocity;
            //timesJumped += 1;
        }
        if (Input.IsActionJustPressed("jump") && Velocity.Y > JumpImpulse / 2.0)
        {
            Vector3 velocity = Velocity;
            velocity.Y = JumpImpulse / (float)2.0;
            Velocity = velocity;
        }
    }

    public int TimesJumped { get; set; }
    
    private void ApplyControllerRotation()
    {
        Vector2 axisVector = Vector2.Zero;
        axisVector.X = Input.GetActionStrength("look_right") - Input.GetActionStrength("look_left");
        axisVector.Y = Input.GetActionStrength("look_down") - Input.GetActionStrength("look_up");

        /*if (inputEvent is InputEventJoypadMotion) (InputEvent inputEvent)
        {*/
        Vector3 rotation = Rotation;
        rotation.Y -= axisVector.X * ControllerSensitivity * (float).001;
        Vector3 headRotation = Head.Rotation;
        headRotation.X = Mathf.Clamp(headRotation.X - axisVector.Y * ControllerSensitivity * (float).001, (float)-1.5, (float)1.5);
        Rotation = rotation;
        Head.Rotation = headRotation;
        //}
    }
    
    

    public override void _ExitTree()
    {
        InputMap.EraseAction("moveForward");
        InputMap.EraseAction("moveBackward");
        InputMap.EraseAction("moveLeft");
        InputMap.EraseAction("moveRight");
        InputMap.EraseAction("jump");
        InputMap.EraseAction("sprint");
    }
}