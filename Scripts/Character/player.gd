extends CharacterBody2D

enum CharacterState { IDLE, WALK, JUMP, FALL, WALL_SLIDE, ATTACK, DASH, HURT, DEATH }
enum Element { AIR, EARTH, FIRE, WATER, NONE }

signal damage_taken
signal resource_modified

@export var PlayerHealth = 5
@export var MaxPlayerHealth = 5
@export var PlayerFireResource = 100
@export var PlayerWaterResource = 100
@export var PlayerAirResource = 100
@export var PlayerEarthResource = 100

@export var PlayerSpritePath : NodePath
@export var AnimationPlayerPath : NodePath
var PlayerSprite : AnimatedSprite2D
var AP : AnimationPlayer

@export var EnableWallJumping = false
@export var JoystickMovement = false
  
@export var ActionUp = "up"
@export var ActionDown = "down"
@export var ActionLeft = "left"
@export var ActionRight = "right"
@export var ActionJump = "jump"
@export var ActionSprint = "sprint"
@export var ActionAttack = "attack"
@export var ActionDash = "dash"

@export var Acceleration = 200.0
@export var MaxSpeed = 200.0
@export var Friction = 1000.0
@export var AirResistance = 200.0
@export var Gravity = 900.0
@export var JumpForce = 300.0
@export var JumpCancelForce = 800.0
@export var WallSlideSpeed = 50.0
@export var CoyoteTimer = 0.08
@export var JumpBufferTimer = 0.1
@export var AttackCooldown = 0.5
@export var DashCooldown = 1.5
@export var DashForce = 1200.0
@export var DashDuration = 0.1
@export var KnockBackForce = 200.0

@export var MoveType = Element.NONE
@export var JumpType = Element.NONE
@export var AttackType = Element.NONE
@export var DashType = Element.NONE    

var last_facing = 1
var state = CharacterState.IDLE
var canWallJump
var canAttack = true

var move_script
var jump_script
var dash_script
var attack_script

func _ready():
	PlayerSprite = get_node_or_null(PlayerSpritePath) if PlayerSpritePath != null else $AnimatedSprite2D
	AP = get_node_or_null(AnimationPlayerPath) if AnimationPlayerPath != null else $AnimationPlayer
	canWallJump = EnableWallJumping
	PlayerHealth = MaxPlayerHealth

	move_script = preload("res://Scripts/Character/move.gd").new()
	jump_script = preload("res://Scripts/Character/jump.gd").new()
	dash_script = preload("res://Scripts/Character/dash.gd").new()
	attack_script = preload("res://Scripts/Character/attack.gd").new()

func _physics_process(delta):
	_physics_tick(delta)

func _physics_tick(delta):
	var inputs = get_inputs()
	if(inputs.input_direction.x != 0 && inputs.input_direction.x != last_facing):
		last_facing = inputs.input_direction.x
	
	jump_script.handle_jump(delta, inputs.input_direction, inputs.jump_strength, inputs.jump_pressed, inputs.jump_released, self, canWallJump, JumpForce, JumpCancelForce, JumpBufferTimer, JumpType)
	dash_script.handle_dash(delta, last_facing, inputs.dash_pressed, self, DashForce, DashDuration, DashCooldown, DashType)
	if not dash_script.dashing:
		move_script.handle_velocity(delta, inputs.input_direction, self, dash_script.dashing, Acceleration, MaxSpeed, Friction, AirResistance, MoveType)
	jump_script.handle_gravity(delta, self, canWallJump, state, Gravity, WallSlideSpeed, CoyoteTimer)
	attack_script.handle_attack(self, AttackCooldown, inputs.attack_pressed, AttackType, last_facing)
	manage_animations()
	if(state != CharacterState.DEATH):
		move_and_slide()

func blocking_state():
	var block := false
	if state == CharacterState.ATTACK:
		block = true
	elif state == CharacterState.HURT:
		block = true


func manage_state():
	if dash_script.dashing:
		state = CharacterState.DASH
	elif is_on_floor():
		if velocity.x == 0:
			state = CharacterState.IDLE
		else:
			state = CharacterState.WALK
	elif velocity.y < 0:
		state = CharacterState.JUMP
	elif not is_on_floor():
		if canWallJump and is_on_wall_only() and get_input_direction().x != 0:
			state = CharacterState.WALL_SLIDE
		else:
			state = CharacterState.FALL

func manage_animations():
	PlayerSprite.set_flip_h(last_facing < 0)

	match state:
		CharacterState.DEATH:
			AP.play("Death")
		CharacterState.HURT:
			AP.play("Hurt")
		CharacterState.ATTACK:
			AP.play("Attack")
		CharacterState.WALK:
			AP.play("Walk")
			manage_state()
		CharacterState.JUMP:
			AP.play("Jump")
			manage_state()
		CharacterState.FALL:
			AP.play("Fall")
			manage_state()
		CharacterState.WALL_SLIDE:
			AP.play("Fall")
		CharacterState.DASH:
			AP.play("Dash")
		CharacterState.IDLE:
			AP.play("Idle")
			manage_state()

func get_inputs() -> Dictionary:
	return {
		"input_direction": get_input_direction(),
		"jump_strength": Input.get_action_strength(ActionJump),
		"jump_pressed": Input.is_action_just_pressed(ActionJump),
		"jump_released": Input.is_action_just_released(ActionJump),
		"dash_pressed": Input.is_action_just_pressed(ActionDash),
		"attack_pressed": Input.is_action_just_pressed(ActionAttack)
	}

func get_input_direction() -> Vector2:
	var xDir = Input.get_action_strength(ActionRight) - Input.get_action_strength(ActionLeft)
	var yDir = Input.get_action_strength(ActionDown) - Input.get_action_strength(ActionUp)
	if JoystickMovement:
		return Vector2(xDir, yDir)
	else:
		return Vector2(sign(xDir), sign(yDir))

func do_take_damage(amt):
	state = CharacterState.HURT
	PlayerHealth -= amt
	damage_taken.emit(PlayerHealth)
	if PlayerHealth == 0:
		state = CharacterState.DEATH


func change_element(element, amount):
	if element == Element.FIRE:
		PlayerFireResource += amount
	if element == Element.AIR:
		PlayerAirResource += amount
	if element == Element.WATER:
		PlayerWaterResource += amount
	if element == Element.EARTH:
		PlayerEarthResource += amount
	clamp(PlayerFireResource, 0,100)
	clamp(PlayerAirResource, 0,100)
	clamp(PlayerWaterResource, 0,100)
	clamp(PlayerEarthResource, 0,100)
	resource_modified.emit(PlayerAirResource, PlayerEarthResource, PlayerFireResource, PlayerWaterResource)
	
func _on_player_space_body_entered(body: Node2D):
	if body.is_in_group("Enemy"):
		velocity = Vector2(sign(velocity.x) * -1 * KnockBackForce, KnockBackForce * 0.8 * -1)
		do_take_damage(1)
	if body.is_in_group("HurtPlayer"):
		do_take_damage(1)
		body.queue_free()
	
func _on_animation_player_animation_finished(anim_name: String):
	if(not state == CharacterState.DEATH):
		manage_state()
	elif state == CharacterState.DEATH:
		disable_mode = CollisionObject2D.DISABLE_MODE_MAKE_STATIC
		process_mode = Node.PROCESS_MODE_DISABLED
