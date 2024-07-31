extends Node2D

@export var enemy : PackedScene = preload("res://Scenes/Enemies/homunculus.tscn")
@export var number = 1
@export var respawn_timer = 10
@export var xmin = -100
@export var xmax = 100

var spawning = false

# Called when the node enters the scene tree for the first time.
func _ready():
	spawn()
	

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if get_child_count() < number and not spawning:
		spawning = true
		await get_tree().create_timer(respawn_timer).timeout
		spawn()  
		spawning = false

func spawn():
	var obj = enemy.instantiate() as Node2D
	var rng = RandomNumberGenerator.new()
	var offset = rng.randi_range(xmin, xmax)
	obj.position = Vector2(offset, 0)
	add_child(obj)

