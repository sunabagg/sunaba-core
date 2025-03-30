using Godot;

namespace Sunaba.Core;

public partial class MouseRayCast3D : RayCast3D
{
	public Camera3D Camera;

	const float RAY_LENGTH = 1000.0f;

	public bool active = false;

	public override void _Ready()
	{
		CollideWithAreas = true;
		CollideWithBodies = false;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (active)
		{
			var space_state = GetWorld3D().DirectSpaceState;
			var mousepos = GetViewport().GetMousePosition();

			var origin = Camera.ProjectRayOrigin(mousepos);
			var end = origin + Camera.ProjectRayNormal(mousepos) * RAY_LENGTH;

			GlobalTransform = Camera.GlobalTransform;
			GlobalRotation = new Vector3(0, 0, 0);
			TargetPosition = end;
		}
	}

	/*
    func _physics_process(delta):
	    var space_state = scene_root.get_world_3d().direct_space_state
	    var mousepos = scene_viewport.get_mouse_position()
    
	    var origin = free_look_camera.project_ray_origin(mousepos)
	    var end = origin + free_look_camera.project_ray_normal(mousepos) * RAY_LENGTH
	
	    #if not Input.is_mouse_button_pressed(MOUSE_BUTTON_RIGHT):
	    gizmo_raycast.global_position = free_look_camera.global_position
	    gizmo_raycast.global_rotation = Vector3.ZERO
	    gizmo_raycast.target_position = end
    */
}