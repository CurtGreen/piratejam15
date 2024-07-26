extends Node

func handle_velocity(delta, input_direction, body, dashing, Acceleration, MaxSpeed, Friction, AirResistance, element):
	if input_direction.x != 0:
		apply_velocity(delta, input_direction, body, dashing, Acceleration, MaxSpeed, element)
	else:
		apply_friction(delta, body, Friction, AirResistance)
	
	if element == body.Element.FIRE and body.is_on_floor():
		var inFire = false
		var space = body.get_node("PlayerSpace") as Area2D
		for O in space.get_overlapping_areas():
			if O.is_in_group("Fire"):
				inFire = true
				break
		if not inFire:
			var scene = preload("res://Scenes/Effects/Fire.tscn")
			var fire = scene.instantiate() as Node2D
			body.get_parent().add_child(fire)
			fire.position = Vector2(body.position.x, body.position.y + 30) # Adjust the position as needed

func apply_velocity(delta, move_direction, body, dashing, Acceleration, MaxSpeed, element):
	body.velocity.x = body.velocity.x + move_direction.x * Acceleration * delta
	if not dashing:
		if element == body.Element.AIR:
			body.velocity.x = clamp(body.velocity.x, -MaxSpeed * 1.5, MaxSpeed * 1.5)
		else:
			body.velocity.x = clamp(body.velocity.x, -MaxSpeed, MaxSpeed)

func apply_friction(delta, body, Friction, AirResistance):
	var fric
	if body.is_on_floor():   
		fric = Friction * delta * sign(body.velocity.x) * -1
	else:
		fric = AirResistance * delta * sign(body.velocity.x) * -1

	if abs(body.velocity.x) <= abs(fric):
		body.velocity.x = 0
	else:
		body.velocity.x += fric
