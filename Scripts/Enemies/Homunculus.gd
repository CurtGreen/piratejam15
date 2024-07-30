extends CharacterBody2D

var chase = false
var player = null
var gravity = 600
var damage = 1
@export var speed = 60
@export var maxSpeed = 150


func _physics_process(delta):
	#Apply Gravity
	if !is_on_floor():
		velocity.y += gravity * delta
	animate_state()
	move_and_slide()

func animate_state():
	match chase:
		true:
			$AnimatedSprite2D.play("Chase")
			$AnimatedSprite2D.set_flip_h(position.direction_to(player.position).x > 0)
			if $AnimatedSprite2D.flip_h:
				$DamageArea/CollisionShape2D.position.x = 17
			else:
				$DamageArea/CollisionShape2D.position.x = -17
			$DamageArea/CollisionShape2D.disabled = false
			velocity.x = position.direction_to(player.position).x * speed
			velocity.x = clamp(velocity.x, -maxSpeed, maxSpeed)
		false:
			$AnimatedSprite2D.play("Idle")
			$DamageArea/CollisionShape2D.disabled = true
			#Apply a Friction Force
			velocity.x = lerp(velocity.x, 0.0, 0.6)
			

func _on_threat_detector_body_entered(body):
	if body.is_in_group("Player"):
		chase = true
		player = body


func _on_threat_detector_body_exited(body):
	if body.is_in_group("Player"):
		chase = false
		player = null


func _on_area_2d_area_shape_entered(area_rid, area, area_shape_index, local_shape_index):
	if area.is_in_group("Damage"):
		process_mode = Node.PROCESS_MODE_DISABLED
		await get_tree().create_timer(0.2).timeout
		queue_free()

