extends Node2D

func _on_area_2d_area_entered(area):
	if area.is_in_group("Damage"):
		$Area2D/AnimationPlayer.play("Switch")
		await get_tree().create_timer(1).timeout
		print("timerisworking")
		$Area2D/AnimationPlayer.play_backwards("Switch")
		var scene = preload("res://Scenes/Items/prima_materia.tscn")
		var materia = scene.instantiate() as Node2D
		materia.position = Vector2(position.x + 50, position. y)
		get_parent().add_child(materia)
 		
