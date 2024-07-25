extends Node

var canJump = false
var shouldJump = false
var wallJump = false
var jumping = false
var jumpCount = 0
var maxJumps = 2

enum JumpDirections { UP = -1, DOWN = 1 }

func handle_jump(delta, move_direction, jump_strength, jump_pressed, _jump_released, body, can_wall_jump, JumpForce, JumpCancelForce, JumpBufferTimer, element):
	if element == body.Element.AIR:
		maxJumps = 2
	else:
		maxJumps = 1

	if (jump_pressed or shouldJump) and (canJump or jumpCount < maxJumps):
		apply_jump(move_direction, body, JumpForce, JumpDirections.UP)
		jumpCount += 1
		if jumpCount == 2:
			body.change_element(body.Element.AIR, -10)
	elif jump_pressed:
		buffer_jump(JumpBufferTimer, body)
	elif jump_strength == 0 and body.velocity.y < 0:
		cancel_jump(delta, body, JumpCancelForce)
	elif can_wall_jump and not body.is_on_floor() and body.is_on_wall_only():
		canJump = true
		wallJump = true
		jumping = false
		jumpCount = 0

	if body.is_on_floor() and body.velocity.y >= 0:
		canJump = true
		wallJump = false
		jumping = false
		jumpCount = 0

func apply_jump(move_direction, body, JumpForce, jump_direction=JumpDirections.UP):
	canJump = false
	shouldJump = false
	jumping = true

	if wallJump:
		body.velocity = Vector2(body.velocity.x + JumpForce * -move_direction.x, 0)
		wallJump = false

	body.velocity = Vector2(body.velocity.x, JumpForce * int(jump_direction))

func cancel_jump(delta, body, JumpCancelForce):
	jumping = false
	body.velocity = Vector2(body.velocity.x, body.velocity.y - JumpCancelForce * sign(body.velocity.y) * float(delta))

# here is a link to a song that eli likes
# https://www.youtube.com/watch?v=ebDlVMri45Q&list=PLf9mYqBGoSyAx6plpzoxRVeuTvXmyF10U&index=5

func buffer_jump(JumpBufferTimer, body):
	shouldJump = true
	await body.get_tree().create_timer(JumpBufferTimer).timeout
	shouldJump = false

func handle_gravity(delta, body, can_wall_jump, state, Gravity, WallSlideSpeed, CoyoteTimer):
	body.velocity = Vector2(body.velocity.x, body.velocity.y + float(Gravity * delta))
	if can_wall_jump and state == body.CharacterState.WALL_SLIDE and not jumping:
		body.velocity = Vector2(body.velocity.x, clamp(body.velocity.y, 0.0, WallSlideSpeed))

	if not body.is_on_floor() and canJump:
		coyote_time(CoyoteTimer, body)

func coyote_time(CoyoteTimer, body):
	await body.get_tree().create_timer(CoyoteTimer).timeout
	canJump = false
