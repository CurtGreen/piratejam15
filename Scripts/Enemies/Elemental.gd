extends CharacterBody2D
var attacking = false
var player = null
var health = 3
var gravity = 600
var damage = 1
var canFireProjectile = true
var projectile
var animationBusy = false
var attackDirection = 1
@export var speed = 60
@export var maxSpeed = 150

func _ready():
	projectile = preload("res://Scenes/Effects/Waterball.tscn")
	
func _physics_process(delta):
	#Apply Gravity
	if !is_on_floor():
		velocity.y += gravity * delta
	animate_state()
	move_and_slide()

func animate_state():
	match attacking:
		true:
			if position.direction_to(player.global_position).x < 0:
				attackDirection = -1
			else:
				attackDirection = 1
			if not animationBusy:
				$AnimatedSprite2D.play("Walk")
				
			$AnimatedSprite2D.set_flip_h(position.direction_to(player.global_position).x < 0)
			
			if attackDirection > 0:
				$DamageArea/CollisionShape2D.position.x = 33.5
			else:
				$DamageArea/CollisionShape2D.position.x = -33.5
			
			$DamageArea/CollisionShape2D.disabled = false
			velocity.x = position.direction_to(player.global_position).x * speed
			velocity.x = clamp(velocity.x, -maxSpeed, maxSpeed)
			if canFireProjectile:
				$AnimatedSprite2D.play("Attack")
				animationBusy = true
				canFireProjectile = false
				var waterball = projectile.instantiate() as Node2D
				self.get_parent().add_child(waterball)
				waterball.direction = attackDirection
				waterball.position = Vector2(self.global_position.x + (40 * waterball.direction), self.global_position.y) # Adjust the position as needed
				$Waterball.play()
				await get_tree().create_timer(2).timeout
				canFireProjectile = true
		false:
			$AnimatedSprite2D.play("Idle")
			$DamageArea/CollisionShape2D.disabled = true
			#Apply a Friction Force
			velocity.x = lerp(velocity.x, 0.0, 0.6)
			

func _on_threat_detector_body_entered(body):
	if body.is_in_group("Player"):
		attacking = true
		player = body


func _on_threat_detector_body_exited(body):
	if body.is_in_group("Player"):
		attacking = false
		player = null


func _on_area_2d_area_shape_entered(area_rid, area, area_shape_index, local_shape_index):
	pass


func _on_damagable_area_entered(area):
	if area.is_in_group("Fire"):
		health -= 1
		$AnimatedSprite2D.play("Hurt")
		animationBusy = true
		if health <= 0:
			process_mode = Node.PROCESS_MODE_DISABLED
			await get_tree().create_timer(0.2).timeout
			queue_free()


func _on_animated_sprite_2d_animation_finished():
	animationBusy = false
