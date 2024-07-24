using Godot;
using System;

public partial class test_dummy : Node2D
{
	public int Health = 10;
	
	public void _on_area_2d_body_entered(Node2D body)
	{
		GD.Print("Something Collided with Dummy");
		
	}
}



