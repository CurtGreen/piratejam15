extends Node2D


func _on_area_2d_area_shape_entered(area_rid, area, area_shape_index, local_shape_index):
	if area != null and area.is_in_group("Player"):
		print("Player Entered")
		$Area2D/AnimationPlayer.play("Open")
		await $Area2D/AnimationPlayer.animation_finished
		get_tree().change_scene_to_file("res://Levels/EndGame.tscn")


func _on_area_2d_area_shape_exited(area_rid, area, area_shape_index, local_shape_index):
	print("Player Exited")
	$Area2D/AnimationPlayer.play_backwards("Open")
