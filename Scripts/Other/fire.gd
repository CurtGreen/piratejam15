extends Node2D

# Called when the node enters the scene tree for the first time.
func _ready():
	var fire = $Flame as GPUParticles2D
	await get_tree().create_timer(1.0).timeout
	fire.amount_ratio = 0.5
	await get_tree().create_timer(1.0).timeout
	fire.amount_ratio = 0.25
	await get_tree().create_timer(1.0).timeout
	fire.amount_ratio = 0.05
	await get_tree().create_timer(1.5).timeout
	queue_free()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	pass
