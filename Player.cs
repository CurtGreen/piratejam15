using Godot;
using System;

public partial class Player : CharacterBody2D
{
	    public enum CharacterState { IDLE, WALK, JUMP, FALL, WALL_SLIDE, ATTACK, DASH }
    public enum JumpDirections { UP = -1, DOWN = 1 }

    [Export] public NodePath PlayerSpritePath;
    [Export] public NodePath AnimationPlayerPath;
    private Sprite2D PlayerSprite;
    private AnimationPlayer AnimationPlayer;

    [Export] public bool JoystickMovement = false;
    [Export] public bool EnableSprint = false;
    [Export] public bool EnableWallJumping = false;
    [Export] public string ActionUp = "up";
    [Export] public string ActionDown = "down";
    [Export] public string ActionLeft = "left";
    [Export] public string ActionRight = "right";
    [Export] public string ActionJump = "jump";
    [Export] public string ActionSprint = "sprint";
    [Export] public string ActionAttack = "attack";
    [Export] public string ActionDash = "dash";

    [Export] public float Acceleration = 700.0f;
    [Export] public float MaxSpeed = 300.0f;
    [Export] public float SprintMultiplier = 1.5f;
    [Export] public float Friction = 800.0f;
    [Export] public float AirResistance = 200.0f;
    [Export] public float Gravity = 1200.0f;
    [Export] public float JumpForce = 700.0f;
    [Export] public float JumpCancelForce = 800.0f;
    [Export] public float WallSlideSpeed = 50.0f;
    [Export] public float CoyoteTimer = 0.08f;
    [Export] public float JumpBufferTimer = 0.1f;
    [Export] public float AttackCooldown = 0.5f;
    [Export] public float DashCooldown = 1.5f;
    [Export] public float DashForce = 1200.0f;
    [Export] public float DashDuration = 0.2f;

    private CharacterState state = CharacterState.IDLE;
    private bool sprinting = false;
    private bool canJump = false;
    private bool shouldJump = false;
    private bool wallJump = false;
    private bool jumping = false;
    private bool canSprint;
    private bool canWallJump;
    private bool canAttack = true;
    private bool canDash = true;
    private bool dashing = false;

    public override void _Ready()
    {
        PlayerSprite = GetNode<Sprite2D>(PlayerSpritePath) ?? GetNode<Sprite2D>("Sprite2D");
        AnimationPlayer = GetNode<AnimationPlayer>(AnimationPlayerPath) ?? GetNode<AnimationPlayer>("AnimationPlayer");
        canSprint = EnableSprint;
        canWallJump = EnableWallJumping;
    }

    public override void _PhysicsProcess(double delta)
    {
        PhysicsTick(delta);
    }

    private void PhysicsTick(double delta)
    {
        var inputs = GetInputs();
        HandleJump(delta, inputs.inputDirection, inputs.jumpStrength, inputs.jumpPressed, inputs.jumpReleased);
        HandleSprint(inputs.sprintStrength);
        HandleDash(delta, inputs.inputDirection, inputs.dashPressed);
        if (!dashing)
        {
            HandleVelocity(delta, inputs.inputDirection);
        }

        ManageAnimations();
        ManageState();

        HandleGravity(delta);

        MoveAndSlide();
    }

    private void ManageState()
    {
        if (dashing)
        {
            state = CharacterState.DASH;
        }
        else if (Velocity.Y == 0.0)
        {
            state = Velocity.X == 0 ? CharacterState.IDLE : CharacterState.WALK;
        }
        else if (Velocity.Y < 0)
        {
            state = CharacterState.JUMP;
        }
        else
        {
            if (canWallJump && IsOnWallOnly() && GetInputDirection().X != 0)
            {
                state = CharacterState.WALL_SLIDE;
            }
            else
            {
                state = CharacterState.FALL;
            }
        }
    }

    private void ManageAnimations()
    {
        PlayerSprite.FlipH = Velocity.X < 0;
        /*
        switch (state)
        {
            case CharacterState.IDLE:
                AnimationPlayer.Play("Idle");
                break;
            case CharacterState.WALK:
                AnimationPlayer.Play("Walk");
                break;
            case CharacterState.JUMP:
                AnimationPlayer.Play("Jump");
                break;
            case CharacterState.FALL:
                AnimationPlayer.Play("Fall");
                break;
            case CharacterState.WALL_SLIDE:
                AnimationPlayer.Play("Fall");
                break;
            case CharacterState.ATTACK:
                AnimationPlayer.Play("Attack");
                break;
            case CharacterState.DASH:
                AnimationPlayer.Play("Dash");
                break;
        }
        */
    }

    private (Vector2 inputDirection, float jumpStrength, bool jumpPressed, bool jumpReleased, float sprintStrength, bool dashPressed) GetInputs()
    {
        return (
            GetInputDirection(),
            Input.GetActionStrength(ActionJump),
            Input.IsActionJustPressed(ActionJump),
            Input.IsActionJustReleased(ActionJump),
            EnableSprint ? Input.GetActionStrength(ActionSprint) : 0.0f,
            Input.IsActionJustPressed(ActionDash)
        );
    }

    private Vector2 GetInputDirection()
    {
        float xDir = Input.GetActionStrength(ActionRight) - Input.GetActionStrength(ActionLeft);
        float yDir = Input.GetActionStrength(ActionDown) - Input.GetActionStrength(ActionUp);

        return new Vector2(JoystickMovement ? xDir : Mathf.Sign(xDir), JoystickMovement ? yDir : Mathf.Sign(yDir));
    }

    private void HandleGravity(double delta)
    {
		
        Velocity = new Vector2(Velocity.X, Velocity.Y + (float)(Gravity * delta));
		Console.WriteLine("Velocity: " + Velocity);

        if (canWallJump && state == CharacterState.WALL_SLIDE && !jumping)
        {
            Velocity = new Vector2(Velocity.X, Mathf.Clamp(Velocity.Y, 0.0f, WallSlideSpeed));
        }

        if (!IsOnFloor() && canJump)
        {
            CoyoteTime();
        }
    }

    private void HandleVelocity(double delta, Vector2 inputDirection)
    {
        if (inputDirection.X != 0)
        {
            ApplyVelocity(delta, inputDirection);
        }
        else
        {
            ApplyFriction(delta);
        }
    }

    private void ApplyVelocity(double delta, Vector2 moveDirection)
    {
        float sprintStrength = sprinting ? SprintMultiplier : 1.0f;
        Velocity = new Vector2(
            (float)(Velocity.X + moveDirection.X * Acceleration * delta * (IsOnFloor() ? sprintStrength : 1.0f)),
            Velocity.Y
        );
        if (!dashing) // As long as we aren't dashing, clamp the speed to stay below the speed limit
        {
            Velocity = new Vector2(
                Mathf.Clamp(Velocity.X, -MaxSpeed * Mathf.Abs(moveDirection.X) * sprintStrength, MaxSpeed * Mathf.Abs(moveDirection.X) * sprintStrength),
                Velocity.Y
            );
        }
    }

    private void ApplyFriction(double delta)
    {
        float fric = IsOnFloor() ? Friction * (float)delta * -Mathf.Sign(Velocity.X) : AirResistance * (float)delta * -Mathf.Sign(Velocity.X);
        if (Mathf.Abs(Velocity.X) <= Mathf.Abs(fric))
        {
            Velocity = new Vector2(0, Velocity.Y);
        }
        else
        {
            Velocity = new Vector2(Velocity.X + fric, Velocity.Y);
        }
    }

    private void HandleSprint(float sprintStrength)
    {
        sprinting = sprintStrength != 0 && canSprint;
    }

    private void HandleJump(double delta, Vector2 moveDirection, float jumpStrength, bool jumpPressed, bool jumpReleased)
    {
        if ((jumpPressed || shouldJump) && canJump)
        {
            ApplyJump(moveDirection);
        }
        else if (jumpPressed)
        {
            BufferJump();
        }
        else if (jumpStrength == 0 && Velocity.Y < 0)
        {
            CancelJump(delta);
        }
        else if (canWallJump && !IsOnFloor() && IsOnWallOnly())
        {
            canJump = true;
            wallJump = true;
            jumping = false;
        }

        if (IsOnFloor() && Velocity.Y >= 0)
        {
            canJump = true;
            wallJump = false;
            jumping = false;
        }
    }

    private void ApplyJump(Vector2 moveDirection, JumpDirections jumpDirection = JumpDirections.UP)
    {
        canJump = false;
        shouldJump = false;
        jumping = true;

        if (wallJump)
        {
            Velocity = new Vector2(Velocity.X + JumpForce * -moveDirection.X, 0);
            wallJump = false;
        }

        Velocity = new Vector2(Velocity.X, Velocity.Y + JumpForce * (int)jumpDirection);
    }

    private async void HandleDash(double delta, Vector2 moveDirection, bool dashPressed)
    {
        if (dashPressed && canDash && !dashing)
        {
            state = CharacterState.DASH;
            dashing = true;
            canDash = false;
            Velocity = new Vector2(DashForce * moveDirection.X, Velocity.Y);
            // Optionally: Play dash animation here
            await ToSignal(GetTree().CreateTimer(DashDuration), "timeout");
            dashing = false;
            state = CharacterState.FALL; // Adjust this based on your desired behavior
            await ToSignal(GetTree().CreateTimer(DashCooldown), "timeout");
            canDash = true;
        }
    }

    private void CancelJump(double delta)
    {
        jumping = false;
        Velocity = new Vector2(Velocity.X, Velocity.Y - JumpCancelForce * Mathf.Sign(Velocity.Y) * (float)delta);
    }

    private async void BufferJump()
    {
        shouldJump = true;
        await ToSignal(GetTree().CreateTimer(JumpBufferTimer), "timeout");
        shouldJump = false;
    }

    private async void CoyoteTime()
    {
        await ToSignal(GetTree().CreateTimer(CoyoteTimer), "timeout");
        canJump = false;
    }
}
