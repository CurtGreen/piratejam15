extends Area2D


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_area_entered(area):
	print("Player Entered")
	$AnimationPlayer.play("Open")

func _on_area_exited(area):
	print("player left")
	$AnimationPlayer.play_backwards("Open")
