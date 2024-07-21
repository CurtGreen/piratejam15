using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class dash : Node
{
    public bool dashing = false;
    public bool canDash = true;
    private int dashCount = 0;
    private int maxDashes = 2;

    public async void HandleDash(double delta, Vector2 moveDirection, bool dashPressed, CharacterBody2D body, float DashForce, float DashDuration, float DashCooldown, Player.Element element)
    {
        if (element == Player.Element.AIR)
        { maxDashes = 2; }
        else
        { maxDashes = 1; }

        if (dashPressed && canDash && !dashing && dashCount < maxDashes)
        {
            dashing = true;
            dashCount++;
            if(element == Player.Element.EARTH)
            { body.Velocity = new Vector2(DashForce * 1.0f * moveDirection.X, DashForce * 0.8f * -1 ); }
            else
            { body.Velocity = new Vector2(DashForce * moveDirection.X, body.Velocity.Y); } 
            
            // Optionally: Play dash animation here
            await ToSignal(body.GetTree().CreateTimer(DashDuration), "timeout");
            dashing = false;
            
            if(dashCount >= maxDashes)
            {
                await ToSignal(body.GetTree().CreateTimer(DashCooldown), "timeout");
                dashCount = 0;
                canDash = true;
                
            }
        }
    }
}