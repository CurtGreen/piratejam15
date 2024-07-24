using Godot;
using System;

public partial class Fireball : Node2D
{
	private Vector2 velocity = new Vector2(0, 0);
	private float speed = 400;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	public void SetDirection(Vector2 direction)
	{
		velocity = direction.Normalized() * speed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
