extends CharacterBody2D

@export var vel = 400
var direction = 1
# Called when the node enters the scene tree for the first time.
func _ready():
	print("Waterball!!!")
	await get_tree().create_timer(4).timeout
	print("waterball dying") 
	queue_free()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	velocity.x = vel * direction
	move_and_slide()

func _on_area_2d_body_entered(body):
	if body.is_in_group("Player"):
		print("Hitting Player and Dying")
		queue_free()
