extends Node

var CanAttack = true
var AttackTimer = 0.3

func handle_attack(character, attack_cooldown, attack_pressed):
	if CanAttack and attack_pressed:
		var attack_hitbox = character.get_node("BasicAttackCollider") as Area2D
		attack_hitbox.monitorable = true
		attack_hitbox.monitoring = true
		CanAttack = false
		await character.get_tree().create_timer(AttackTimer).timeout
		attack_hitbox.monitorable = false
		attack_hitbox.monitoring = false
		await character.get_tree().create_timer(attack_cooldown).timeout
		CanAttack = true
