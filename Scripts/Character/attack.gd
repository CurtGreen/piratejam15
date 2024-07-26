extends Node

var CanAttack = true
var AttackTimer = 0.3

func handle_attack(character, attack_cooldown, attack_pressed, element, direction):
	if CanAttack and attack_pressed:
		character.state = character.CharacterState.ATTACK
		if element == character.Element.FIRE:
			CanAttack = false
			var scene = preload("res://Scenes/Effects/Fireball.tscn")
			var fireball = scene.instantiate() as Node2D
			character.get_parent().add_child(fireball)
			fireball.position = Vector2(character.position.x +(30*direction), character.position.y) # Adjust the position as needed
			fireball.direction = direction
			await character.get_tree().create_timer(attack_cooldown).timeout
			CanAttack = true
		elif element == character.Element.WATER:
			CanAttack = false
			var scene = preload("res://Scenes/Effects/Waterball.tscn")
			var waterball = scene.instantiate() as Node2D
			character.get_parent().add_child(waterball)
			waterball.position = Vector2(character.position.x +(30*direction), character.position.y) # Adjust the position as needed
			waterball.direction = direction
			await character.get_tree().create_timer(attack_cooldown).timeout
			CanAttack = true
		else:
			var attack_hitbox = character.get_node("BasicAttackCollider") as Area2D
			attack_hitbox.monitorable = true
			attack_hitbox.monitoring = true
			CanAttack = false
			await character.get_tree().create_timer(AttackTimer).timeout
			attack_hitbox.monitorable = false
			attack_hitbox.monitoring = false
			await character.get_tree().create_timer(attack_cooldown).timeout
			CanAttack = true
