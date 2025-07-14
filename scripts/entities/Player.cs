using Godot;
using System;

public partial class Player : CharacterBody2D {
	#region Fields
	[Export] private Node2D rod;
	[Export] private Marker2D rodRight;
	[Export] private Marker2D rodLeft;

	[Export] private RayCast2D buffer;
	private bool jumpBuffered = false;
	[Export] private float JumpForce = -500;

	private bool jumping = false;

	[Export] private Timer coyoteTimer;
	private readonly float CoyoteTime = 0.25f;
	private bool coyote = false;
	private bool lastFrameFloored = false;

	private readonly float Gravity = 980;

	[Export] private float MaxSpeed = 200;
	[Export] private float Acceleration = 15.75f;

	[Export] private RayCast2D safePositionLeft;
	[Export] private RayCast2D safePositionRight;
	public Vector2? LastKnownSafePosition { get; private set; } = null;
	#endregion

	public override void _Ready() {
		coyoteTimer.WaitTime = CoyoteTime;
	}

	public override void _PhysicsProcess(double delta) {
		Vector2 velocity = Velocity;
		if (IsOnFloor() && jumping) jumping = false;

		if (!IsOnFloor()) {
			velocity.Y += Gravity * (float)delta;

			if (lastFrameFloored && !jumping) {
				coyote = true;
				coyoteTimer.Start();
			}
		}

        #region Jumping and Movement Logic
        // Left-Right movement.
        float direction = Input.GetAxis("left", "right");
		if (direction != 0) {
			velocity.X = Mathf.MoveToward(velocity.X, direction * MaxSpeed, Acceleration);

			// Node2D does not have a FlipH field :(
			rod.Position = direction < 0 ? rodLeft.Position : rodRight.Position;
			rod.Scale = new Vector2(direction, 1);
		} else {
			velocity.X = Mathf.MoveToward(velocity.X, 0, Acceleration);
		}

		// Jump
		if (Input.IsActionJustPressed("jump") && (IsOnFloor() || coyote)) {
			velocity.Y = JumpForce;
			jumping = true;

			if (coyote) coyote = false;
		}

		// Jump buffering
		if (!IsOnFloor() && buffer.IsColliding()) {
			// Velocity has to be positive
			// It means we are FALLING.
			if (velocity.Y > 0 && Input.IsActionJustPressed("jump")) { 
				jumpBuffered = true;
			}
		}

		if (IsOnFloor()) {
			if (jumpBuffered) {
				velocity.Y = JumpForce;
				jumping = true;

				jumpBuffered = false;
			}
		}
        #endregion

        lastFrameFloored = IsOnFloor();
		Velocity = velocity;
		MoveAndSlide();
	}

	private void CoyoteTimeout() => coyote = false;

	private void CheckSafePosition() {
		if (safePositionLeft.IsColliding() && safePositionRight.IsColliding()) {
			LastKnownSafePosition = Position;
		}
	}
}
