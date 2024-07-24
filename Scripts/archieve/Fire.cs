using Godot;
using System;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

public partial class Fire : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		GpuParticles2D fire = this.GetNode<GpuParticles2D>("Flame");
		await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
		fire.AmountRatio = 0.5f;
		await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
		fire.AmountRatio = 0.25f;
		await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
		fire.AmountRatio = 0.05f;
		await ToSignal(GetTree().CreateTimer(1.5f), "timeout");
		this.QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
