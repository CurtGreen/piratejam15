extends TextureRect


func _on_player_resource_modified(fire, air, water, earth):
	$Fire.value = fire
	$Air.value = air
	$Water.value = water
	$Earth.value = earth
