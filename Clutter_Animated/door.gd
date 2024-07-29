extends Node2D


func _on_area_2d_area_shape_entered(area_rid, area, area_shape_index, local_shape_index):
	print("Player Entered")
	$Area2D/AnimationPlayer.play("Open")


func _on_area_2d_area_shape_exited(area_rid, area, area_shape_index, local_shape_index):
	print("Player Exited")
	$Area2D/AnimationPlayer.play_backwards("Open")
