extends Node

var dashing = false
var canDash = true
var dashCount = 0
var maxDashes = 2

func handle_dash(delta, move_direction, dash_pressed, body, DashForce, DashDuration, DashCooldown, element):
	if element == body.Element.AIR:
		maxDashes = 2
		
	else:
		maxDashes = 1

	if dash_pressed and canDash and not dashing and dashCount < maxDashes:
		dashing = true
		dashCount += 1
#		if element == body.Element.AIR:
#			body.change_element(body.Element.AIR, -10)
		if element == body.Element.EARTH and body.PlayerEarthResource > 0 :
			body.change_element(body.Element.EARTH, -20)
			body.velocity = Vector2(DashForce * move_direction, -1 * DashForce * 1.5 )
		else:
			body.velocity = Vector2(DashForce * move_direction, body.velocity.y)
			var scene = preload("res://Scenes/Effects/DASH_Effect.tscn")
			var dashEffect = scene.instantiate() as Node2D
			body.add_child(dashEffect)
			dashEffect.process_material.direction.x = -100 * move_direction
			dashEffect.position = Vector2((-30*move_direction), 0) # Adjust the position as needed


		# Optionally: Play dash animation here
		await body.get_tree().create_timer(DashDuration).timeout
		dashing = false

		if dashCount >= maxDashes:
			await body.get_tree().create_timer(DashCooldown).timeout
			dashCount = 0
			canDash = true
