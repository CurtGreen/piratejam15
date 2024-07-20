using Godot;
using System;

public partial class jump : Node
{
    private bool canJump = false;
    private bool shouldJump = false;
    private bool wallJump = false;
    private bool jumping = false;
    
    public enum JumpDirections { UP = -1, DOWN = 1 }

	public void HandleJump(double delta, Vector2 moveDirection, float jumpStrength, bool jumpPressed, bool jumpReleased, CharacterBody2D body, bool canWallJump, float JumpForce, float JumpCancelForce, double JumpBufferTimer)
    {
        if ((jumpPressed || shouldJump) && canJump)
        {
            ApplyJump(moveDirection, body, JumpForce, JumpDirections.UP);
        }
        else if (jumpPressed)
        {
            BufferJump(JumpBufferTimer, body);
        }
        else if (jumpStrength == 0 && body.Velocity.Y < 0)
        {
            CancelJump(delta, body, JumpCancelForce);
        }
        else if (canWallJump && !body.IsOnFloor() &&body.IsOnWallOnly())
        {
            canJump = true;
            wallJump = true;
            jumping = false;
        }

        if (body.IsOnFloor() && body.Velocity.Y >= 0)
        {
            canJump = true;
            wallJump = false;
            jumping = false;
        }
    }

    private void ApplyJump(Vector2 moveDirection, CharacterBody2D body, float JumpForce, JumpDirections jumpDirection = JumpDirections.UP)
    {
        canJump = false;
        shouldJump = false;
        jumping = true;

        if (wallJump)
        {
            body.Velocity = new Vector2(body.Velocity.X + JumpForce * -moveDirection.X, 0);
            wallJump = false;
        }

        body.Velocity = new Vector2(body.Velocity.X, body.Velocity.Y + JumpForce * (int)jumpDirection);
    }
	
    private void CancelJump(double delta, CharacterBody2D body, float JumpCancelForce)
    {
        jumping = false;
        body.Velocity = new Vector2(body.Velocity.X, body.Velocity.Y - JumpCancelForce * Mathf.Sign(body.Velocity.Y) * (float)delta);
    }

    private async void BufferJump(double JumpBufferTimer, CharacterBody2D body)
    {
        shouldJump = true;
        await ToSignal(body.GetTree().CreateTimer(JumpBufferTimer), "timeout");
        shouldJump = false;
    }

    public void HandleGravity(double delta, CharacterBody2D body, bool canWallJump, Player.CharacterState state, float Gravity, float WallSlideSpeed, double CoyoteTimer)
    {
		
        body.Velocity = new Vector2(body.Velocity.X, body.Velocity.Y + (float)(Gravity * delta));
        if (canWallJump && state == Player.CharacterState.WALL_SLIDE && !jumping)
        {
            body.Velocity = new Vector2(body.Velocity.X, Mathf.Clamp(body.Velocity.Y, 0.0f, WallSlideSpeed));
        }

        if (!body.IsOnFloor() && canJump)
        {
            CoyoteTime(CoyoteTimer, body);
        }
    }
    private async void CoyoteTime(double CoyoteTimer, CharacterBody2D body)
    {
        await ToSignal(body.GetTree().CreateTimer(CoyoteTimer), "timeout");
        canJump = false;
    }
}