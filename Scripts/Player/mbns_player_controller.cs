using Godot;
using System;

public partial class mbns_player_controller : Node3D
{
	[Export] public const float Speed = 5.0f;
	[Export] public float JumpVelocity { get; set; }

	[Export] public string IdleAnimationName { get; set; } //idle animation

	[Export] public Node3D CameraNode { get; set; }
	//Only needed if the player has a model
	//[Export] public Node3D PlayerModel { get; set; }
	[Export] public Node3D PlayerRoot { get; set; }

	
	[Export] public float RotationSpeed { get; set; }
	[Export] public float CameraActualRotationSpeed { get; set; }
	[Export] public float BodyActualRotationSpeed { get; set; }
	[Export] public float RootActualRotationSpeed { get; set; }
	[Export] public float VerticalRotationLimit { get; set; } = 90; //we want the player to be able to spin when in midair, so this may prove unnecessary in the future

	[Export] public RayCast3D RideHeightRay { get; set;}
	[Export] public RayCast3D GroundHeightRay { get; set;}
	private Vector3 camTargetRotation;
	//private Vector3 bodyTargetRotation;
	private Vector3 rootTargetRotation;
	private Vector3 Velocity;

	public bool IsOnFloor = true;


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
		GD.Print(IsOnFloor);
		Vector3 velocity = Velocity;
		GD.Print(Velocity);
		
		
		if (GroundHeightRay.IsColliding()) {
			IsOnFloor = true;
		}
		else {
			IsOnFloor = false;
		}

		// Add the gravity. This still goes when the player is grounded for some reason.
		if (!IsOnFloor) {
			velocity.Y -= gravity * (float)delta;
		}
		else {
			velocity.Y = 0;
		}

		
		// Handle Jump.
		if (Input.IsActionJustPressed("jump") & IsOnFloor)
			velocity.Y = JumpVelocity;


		// Get the input direction and handle the movement/deceleration.
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		Vector3 direction = Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y);
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
		//rigidbody uses apply_impulse. If it moves infinitely use linear_damp to cause the body to lose velocity over time

		//handle camera up/down (x) rotation
		CameraNode.Rotation = new Vector3(
			Mathf.LerpAngle(CameraNode.Rotation.X, Mathf.DegToRad(camTargetRotation.X), CameraActualRotationSpeed * (float)delta),
			0,
			0
			);
		//handle player side to side (y) rotation
		PlayerRoot.Rotation = new Vector3(
				0,
				Mathf.LerpAngle(PlayerRoot.Rotation.Y, Mathf.DegToRad(rootTargetRotation.Y), RootActualRotationSpeed * (float)delta),
				0
			);
	}
		
	public override void _Input(InputEvent @event)
	{
		//detects and responds to user mouse movement
		if (@event is InputEventMouseMotion mouseMotion)
		{
			//rotate the characterbody node on the Y axis when mouse is moved side to side
			//rotate characterbody node on the X axis when mouse is moved up and down (later change this to the hip bone on the player skeleton)
			//Debug.WriteLine("mouse input detected");
			//calculate camera x rotation
			camTargetRotation = new Vector3(
				Mathf.Clamp((-1 * mouseMotion.Relative.Y * RotationSpeed) + camTargetRotation.X, -VerticalRotationLimit, VerticalRotationLimit),
				0, 
				0);
			//calculate y rotation of the entire player
			rootTargetRotation = new Vector3(
				0,
				Mathf.Wrap((-1 * mouseMotion.Relative.X * RotationSpeed) + rootTargetRotation.Y, 0, 360), 
				0
			);
		
		}

		if (@event.IsActionPressed("escape")){
			ToggleMouseMode();
		}

		if (@event.IsActionPressed("primary_fire")) {

		}

		if (@event.IsActionPressed("secondary_fire")) {
			
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
