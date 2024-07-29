extends CharacterBody2D

@export var vel = 400
var direction = 1
# Called when the node enters the scene tree for the first time.
func _ready():
	await get_tree().create_timer(4).timeout 
	queue_free()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	velocity.x = vel * direction
	move_and_slide()

func _on_area_2d_body_entered(body):
	if(not body.is_in_group("Damage")):
		queue_free()
