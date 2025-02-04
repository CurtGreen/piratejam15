using Godot;
using System;
using System.Reflection.Emit;

public partial class Player : CharacterBody2D
{
	public enum CharacterState { IDLE, WALK, JUMP, FALL, WALL_SLIDE, ATTACK, DASH, HURT }
	public enum Element { AIR, EARTH, FIRE, WATER, NONE };

	[Export] public int PlayerHealth = 5;
	[Export] public int MaxPlayerHealth = 5;

	[Export] public NodePath PlayerSpritePath;
	[Export] public NodePath AnimationPlayerPath;
	private AnimatedSprite2D PlayerSprite;
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

	[Export] public float Acceleration = 200.0f;
	[Export] public float MaxSpeed = 200.0f;
	[Export] public float Friction = 1000.0f;
	[Export] public float AirResistance = 200.0f;
	[Export] public float Gravity = 900.0f;
	[Export] public float JumpForce = 300.0f;
	[Export] public float JumpCancelForce = 800.0f;
	[Export] public float WallSlideSpeed = 50.0f;
	[Export] public float CoyoteTimer = 0.08f;
	[Export] public float JumpBufferTimer = 0.1f;
	[Export] public float AttackCooldown = 0.5f;
	[Export] public float DashCooldown = 1.5f;
	[Export] public float DashForce = 1200.0f;
	[Export] public float DashDuration = 0.1f;
	[Export] public float KnockBackForce = 200.0f;

	[Export] public Element MoveType = Element.NONE;
	[Export] public Element JumpType = Element.NONE;
	[Export] public Element AttackType = Element.NONE;
	[Export] public Element DashType = Element.NONE;

	public CharacterState state = CharacterState.IDLE;
	private bool canWallJump;
	private bool canAttack = true;

	private move move_script;
	private jump jump_script;
	private dash dash_script;
	private attack attack_script;

	public override void _Ready()
	{
		PlayerSprite = GetNode<AnimatedSprite2D>(PlayerSpritePath) ?? GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AnimationPlayer = GetNode<AnimationPlayer>(AnimationPlayerPath) ?? GetNode<AnimationPlayer>("AnimationPlayer");
		canWallJump = EnableWallJumping;

		PlayerHealth=MaxPlayerHealth;

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
		var inputs = GetInputs();
		jump_script.HandleJump(delta, inputs.inputDirection, inputs.jumpStrength, inputs.jumpPressed, inputs.jumpReleased, this, canWallJump, JumpForce, JumpCancelForce, JumpBufferTimer, JumpType);
		dash_script.HandleDash(delta, inputs.inputDirection, inputs.dashPressed, this, DashForce, DashDuration, DashCooldown, DashType);
		if (!dash_script.dashing)
		{
			move_script.HandleVelocity(delta, inputs.inputDirection, this, dash_script.dashing, Acceleration, MaxSpeed, Friction, AirResistance, MoveType);
		}
		jump_script.HandleGravity(delta, this, canWallJump, state, Gravity, WallSlideSpeed, CoyoteTimer);
		attack_script.HandleAttack(this, AttackCooldown, inputs.attackPressed);
		ManageAnimations();
		ManageState();
		MoveAndSlide();
	}

	private void ManageState()
	{
		if(state != CharacterState.HURT)
		{
			
			if (dash_script.dashing)
			{
				state = CharacterState.DASH;
			}
			else if (IsOnFloor())
			{
				state = Velocity.X == 0 ? CharacterState.IDLE : CharacterState.WALK;
			}
			else if (Velocity.Y < 0)
			{
				state = CharacterState.JUMP;
			}
			else if (!IsOnFloor())
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
	}

	private void ManageAnimations()
	{
		PlayerSprite.FlipH = Velocity.X < 0;
		
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
			case CharacterState.HURT:
				AnimationPlayer.Play("Hurt");
				break;
		}

		if (state == CharacterState.HURT && !AnimationPlayer.IsPlaying())
		{
			state = CharacterState.IDLE;
		}
	}

	private (Vector2 inputDirection, float jumpStrength, bool jumpPressed, bool jumpReleased, bool dashPressed, bool attackPressed) GetInputs()
	{
		return (
			GetInputDirection(),
			Input.GetActionStrength(ActionJump),
			Input.IsActionJustPressed(ActionJump),
			Input.IsActionJustReleased(ActionJump),
			Input.IsActionJustPressed(ActionDash),
			Input.IsActionJustPressed(ActionAttack)
		);
	}

	private Vector2 GetInputDirection()
	{
		float xDir = Input.GetActionStrength(ActionRight) - Input.GetActionStrength(ActionLeft);
		float yDir = Input.GetActionStrength(ActionDown) - Input.GetActionStrength(ActionUp);

		return new Vector2(JoystickMovement ? xDir : Mathf.Sign(xDir), JoystickMovement ? yDir : Mathf.Sign(yDir));
	}

	public void _on_player_space_body_entered(Node2D body)
	{
		if (body.IsInGroup("Enemy"))
		{
			Velocity = new Vector2(Mathf.Sign(Velocity.X) * -1 * KnockBackForce, KnockBackForce * 0.8f * -1 );
			state = CharacterState.HURT;
		}
	}

	public void _on_animation_player_animation_finished(string anim_name)
	{
		if (anim_name == "Hurt")
		{
			state = CharacterState.IDLE;
		}
	}
}
