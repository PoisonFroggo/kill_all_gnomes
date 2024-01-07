using Godot;
using System;
using System.Diagnostics;

public partial class player : CharacterBody3D
{
	[Export] public const float Speed = 5.0f;
	[Export] public const float JumpVelocity = 4.5f;

	[Export] public string IdleAnimationName { get; set; } //idle animation

	[Export] public Node3D CameraNode { get; set; }
	[Export] public Node3D PlayerModel { get; set; }

	
	[Export] public float RotationSpeed { get; set; }
	[Export] public float CameraActualRotationSpeed { get; set; }
	[Export] public float VerticalRotationLimit { get; set; } = 80; //we want the player to be able to spin when in midair, so this may prove unnecessary in the future

	private Vector3 targetRotation;


	private float _rotationX = 0f;
	private float _rotationY = 0f;
	private float lookSensitivity = -.01f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	
	public override void _Ready()
	{
		//lock the mouse cursor
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
		
	public override void _Input(InputEvent @event)
	{
		//detects and responds to user mouse movement
		if (@event is InputEventMouseMotion mouseMotion)
		{
			//Debug.WriteLine("mouse input detected");
			/*targetRotation = new Vector3(
				Mathf.Clamp((-1 * mouseMotion.Relative.Y * RotationSpeed) + targetRotation.X, -VerticalRotationLimit, VerticalRotationLimit),
				Mathf.Wrap((-1 * mouseMotion.Relative.X * RotationSpeed) + targetRotation.Y, 0, 360), 
				0);
			CameraNode.Rotation = targetRotation;*/
			//rotate the characterbody node on the Y axis when mouse is moved side to side
			//rotate characterbody node on the X axis when mouse is moved up and down (later change this to the hip bone on the player skeleton)
		}

		if (@event.IsActionPressed("escape")){
			ToggleMouseMode();
		}
	}

	private void ToggleMouseMode()
	{
		if(Input.MouseMode == Input.MouseModeEnum.Visible)
		{
			Input.MouseMode =Input.MouseModeEnum.Captured;
		}
		else 
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}
}
