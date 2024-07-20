using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public enum CharacterState { IDLE, WALK, JUMP, FALL, WALL_SLIDE, ATTACK, DASH }

    [Export] public NodePath PlayerSpritePath;
    [Export] public NodePath AnimationPlayerPath;
    private Sprite2D PlayerSprite;
    private AnimationPlayer AnimationPlayer;

    [Export] public bool JoystickMovement = false;
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
    [Export] public float Friction = 800.0f;
    [Export] public float AirResistance = 200.0f;
    [Export] public float Gravity = 1200.0f;//1200.0f
    [Export] public float JumpForce = 700.0f;
    [Export] public float JumpCancelForce = 800.0f;
    [Export] public float WallSlideSpeed = 50.0f;
    [Export] public float CoyoteTimer = 0.08f;
    [Export] public float JumpBufferTimer = 0.1f;
    [Export] public float AttackCooldown = 0.5f;
    [Export] public float DashCooldown = 1.5f;
    [Export] public float DashForce = 1200.0f;
    [Export] public float DashDuration = 0.2f;

    public CharacterState state = CharacterState.IDLE;
    private bool canWallJump;
    private bool canAttack = true;

    private move move_script;
    private jump jump_script;
    private dash dash_script;
    private attack attack_script;

    public override void _Ready()
    {
        PlayerSprite = GetNode<Sprite2D>(PlayerSpritePath) ?? GetNode<Sprite2D>("Sprite2D");
        AnimationPlayer = GetNode<AnimationPlayer>(AnimationPlayerPath) ?? GetNode<AnimationPlayer>("AnimationPlayer");
        canWallJump = EnableWallJumping;

        move_script = (move)GD.Load<CSharpScript>("res://Scripts/Character/move.cs").New();
        jump_script = (jump)GD.Load<CSharpScript>("res://Scripts/Character/jump.cs").New();
        dash_script = (dash)GD.Load<CSharpScript>("res://Scripts/Character/dash.cs").New();
        attack_script = (attack)GD.Load<CSharpScript>("res://Scripts/Character/attack.cs").New();
    }

    public override void _PhysicsProcess(double delta)
    {
        PhysicsTick(delta);
    }

    private void PhysicsTick(double delta)
    {
        Console.WriteLine(dash_script.dashing);
        var inputs = GetInputs();
        jump_script.HandleJump(delta, inputs.inputDirection, inputs.jumpStrength, inputs.jumpPressed, inputs.jumpReleased, this, canWallJump, JumpForce, JumpCancelForce, JumpBufferTimer);
        dash_script.HandleDash(delta, inputs.inputDirection, inputs.dashPressed, this, DashForce, DashDuration, DashCooldown);
        if (!dash_script.dashing)
        {
            move_script.HandleVelocity(delta, inputs.inputDirection, this, dash_script.dashing, Acceleration, MaxSpeed, Friction, AirResistance);
        }

        ManageAnimations();
        ManageState();

        jump_script.HandleGravity(delta, this, canWallJump, state, Gravity, WallSlideSpeed, CoyoteTimer);

        MoveAndSlide();
    }

    private void ManageState()
    {
        if (dash_script.dashing)
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

    private (Vector2 inputDirection, float jumpStrength, bool jumpPressed, bool jumpReleased, bool dashPressed) GetInputs()
    {
        return (
            GetInputDirection(),
            Input.GetActionStrength(ActionJump),
            Input.IsActionJustPressed(ActionJump),
            Input.IsActionJustReleased(ActionJump),
            Input.IsActionJustPressed(ActionDash)
        );
    }

    private Vector2 GetInputDirection()
    {
        float xDir = Input.GetActionStrength(ActionRight) - Input.GetActionStrength(ActionLeft);
        float yDir = Input.GetActionStrength(ActionDown) - Input.GetActionStrength(ActionUp);

        return new Vector2(JoystickMovement ? xDir : Mathf.Sign(xDir), JoystickMovement ? yDir : Mathf.Sign(yDir));
    }
}
