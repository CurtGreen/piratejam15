using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class dash : Node
{
    public bool dashing = false;
    public bool canDash = true;
    public async void HandleDash(double delta, Vector2 moveDirection, bool dashPressed, CharacterBody2D body, float DashForce, float DashDuration, float DashCooldown)
    {
        if (dashPressed && canDash && !dashing)
        {
            dashing = true;
            canDash = false;
            body.Velocity = new Vector2(DashForce * moveDirection.X, body.Velocity.Y);
            // Optionally: Play dash animation here
            await ToSignal(body.GetTree().CreateTimer(DashDuration), "timeout");
            dashing = false;
            await ToSignal(body.GetTree().CreateTimer(DashCooldown), "timeout");
            canDash = true;
        }
    }
}