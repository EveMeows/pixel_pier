using Godot;
using System;

public partial class Player : CharacterBody2D {
    #region Fields
    [Export] private RayCast2D buffer;
	private bool jumpBuffered = false;
	private readonly float JumpForce = -500;

	private bool jumping = false;

	[Export] private Timer coyoteTimer;
	private readonly float CoyoteTime = 0.25f;
	private bool coyote = false;
	private bool lastFrameFloored = false;

	private readonly float Gravity = 980;

	private readonly float MaxSpeed = 200;
	private readonly float Acceleration = 15.75f;
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
}
