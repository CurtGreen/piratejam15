using Godot;
using System;

public partial class move : Node
{
public void HandleVelocity(double delta, Vector2 inputDirection, CharacterBody2D body, bool dashing, float Acceleration, float MaxSpeed, float Friction, float AirResistance)
    {
        if (inputDirection.X != 0)
        {
            ApplyVelocity(delta, inputDirection, body, dashing, Acceleration, MaxSpeed);
        }
        else
        {
            ApplyFriction(delta, body, Friction, AirResistance);
        }
    }

    private void ApplyVelocity(double delta, Vector2 moveDirection, CharacterBody2D body, bool dashing, float Acceleration, float MaxSpeed)
    {
        body.Velocity = new Vector2(
            (float)(body.Velocity.X + moveDirection.X * Acceleration * delta),
            body.Velocity.Y
        );
        if (!dashing) // As long as we aren't dashing, clamp the speed to stay below the speed limit
        {
            body.Velocity = new Vector2(
                Mathf.Clamp(body.Velocity.X, -MaxSpeed * Mathf.Abs(moveDirection.X), MaxSpeed * Mathf.Abs(moveDirection.X)),
                body.Velocity.Y
            );
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