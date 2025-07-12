using Godot;
using System;

public partial class Player : CharacterBody2D {
	private readonly float Gravity = 980;
	
	public override void _PhysicsProcess(double delta) {
		Vector2 velocity = Velocity;
		if (!IsOnFloor()) {
			velocity.Y += Gravity * (float)delta;
		}
		
		Velocity = velocity;
		MoveAndSlide();
	}
}
