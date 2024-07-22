using Godot;
using System;

public partial class tempEnemy : CharacterBody2D
{

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		// Add the gravity.
		if (!IsOnFloor())
			Velocity = new Vector2(Velocity.X, Velocity.Y + gravity * (float)delta);
		MoveAndSlide();
	}
}
