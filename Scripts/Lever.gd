extends Node2D

func _on_area_2d_area_entered(area):
	print("Shits workin")
	if area.is_in_group("Damage"):
		print("Shits workin real good")
		$Area2D/AnimationPlayer.play("Switch")
		await get_tree().create_timer(1).timeout
		print("timerisworking")
		$Area2D/AnimationPlayer.play_backwards("Switch")
