using Godot;
using System;

public partial class move : Node
{
public void HandleVelocity(double delta, Vector2 inputDirection, CharacterBody2D body, bool dashing, float Acceleration, float MaxSpeed, float Friction, float AirResistance, Player.Element element)
	{
		if (inputDirection.X != 0)
		{
			ApplyVelocity(delta, inputDirection, body, dashing, Acceleration, MaxSpeed, element);
		}
		else
		{
			ApplyFriction(delta, body, Friction, AirResistance);
		}
		if (element == Player.Element.FIRE && body.IsOnFloor())
			{
				Boolean inFire = false;
				Area2D space = body.GetNode<Area2D>("PlayerSpace");
				foreach (Node2D O in space.GetOverlappingAreas())
				{
					//inFire = O.GetGroups().Contains("Fire") ? true : false;
					if(O.IsInGroup("Fire"))
					{
						inFire = true;
						break;
					}
				}
				if (!inFire)
				{
					var scene = GD.Load<PackedScene>("res://Fire.tscn");
					Node2D fire = (Node2D)scene.Instantiate();
					body.GetParent().AddChild(fire);
					fire.Position = new Vector2 (body.Position.X, body.Position.Y + 30); // There is probably a better way to find the bottom of the sprite, but fuck it
				}
			}
	}

	private void ApplyVelocity(double delta, Vector2 moveDirection, CharacterBody2D body, bool dashing, float Acceleration, float MaxSpeed, Player.Element element)
	{
		body.Velocity = new Vector2(
			(float)(body.Velocity.X + moveDirection.X * Acceleration * delta),
			body.Velocity.Y
		);
		if (!dashing) // As long as we aren't dashing, clamp the speed to stay below the speed limit
		{
			if (element == Player.Element.AIR)
			{
				body.Velocity = new Vector2(
				Mathf.Clamp(body.Velocity.X, -MaxSpeed *1.5f * Mathf.Abs(moveDirection.X), MaxSpeed * 1.5f * Mathf.Abs(moveDirection.X)),
				body.Velocity.Y
			);
			}
			else
			{
				body.Velocity = new Vector2(
					Mathf.Clamp(body.Velocity.X, -MaxSpeed * Mathf.Abs(moveDirection.X), MaxSpeed * Mathf.Abs(moveDirection.X)),
					body.Velocity.Y
				);
			}
		}
	}

	private void ApplyFriction(double delta, CharacterBody2D body, float Friction, float AirResistance)
	{
		float fric = body.IsOnFloor() ? Friction * (float)delta * -Mathf.Sign(body.Velocity.X) : AirResistance * (float)delta * -Mathf.Sign(body.Velocity.X);
		if (Mathf.Abs(body.Velocity.X) <= Mathf.Abs(fric))
		{
			body.Velocity = new Vector2(0, body.Velocity.Y);
		}
		else
		{
			body.Velocity = new Vector2(body.Velocity.X + fric, body.Velocity.Y);
		}
	}
}
