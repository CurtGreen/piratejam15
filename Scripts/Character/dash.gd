extends Node

var dashing = false
var canDash = true
var dashCount = 0
var maxDashes = 2

func handle_dash(_delta, move_direction, dash_pressed, body, DashForce, DashDuration, DashCooldown, element):
	if element == body.Element.AIR:
		maxDashes = 2
	else:
		maxDashes = 1

	if dash_pressed and canDash and not dashing and dashCount < maxDashes:
		dashing = true
		dashCount += 1
		if element == body.Element.EARTH:
			body.velocity = Vector2(DashForce * 1.0 * move_direction.x, DashForce * 0.8 * -1)
		else:
			body.velocity = Vector2(DashForce * move_direction.x, body.velocity.y)

		# Optionally: Play dash animation here
		await body.get_tree().create_timer(DashDuration).timeout
		dashing = false

		if dashCount >= maxDashes:
			await body.get_tree().create_timer(DashCooldown).timeout
			dashCount = 0
			canDash = true
