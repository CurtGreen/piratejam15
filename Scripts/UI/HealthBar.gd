extends HBoxContainer



func _on_player_damage_taken(PlayerHealth):
	if PlayerHealth == 5:
		$HP5.modulate = Color(1,1,1,1)
		$HP4.modulate = Color(1,1,1,1)
		$HP3.modulate = Color(1,1,1,1)
		$HP2.modulate = Color(1,1,1,1)
		$HP1.modulate = Color(1,1,1,1)
	if PlayerHealth == 4:
		$HP5.modulate = Color(0,0,0,1)
		$HP4.modulate = Color(1,1,1,1)
		$HP3.modulate = Color(1,1,1,1)
		$HP2.modulate = Color(1,1,1,1)
		$HP1.modulate = Color(1,1,1,1)
	if PlayerHealth == 3:
		$HP5.modulate = Color(0,0,0,1)
		$HP4.modulate = Color(0,0,0,1)
		$HP3.modulate = Color(1,1,1,1)
		$HP2.modulate = Color(1,1,1,1)
		$HP1.modulate = Color(1,1,1,1)
	if PlayerHealth == 2:
		$HP5.modulate = Color(0,0,0,1)
		$HP4.modulate = Color(0,0,0,1)
		$HP3.modulate = Color(0,0,0,1)
		$HP2.modulate = Color(1,1,1,1)
		$HP1.modulate = Color(1,1,1,1)
	if PlayerHealth == 1:
		$HP5.modulate = Color(0,0,0,1)
		$HP4.modulate = Color(0,0,0,1)
		$HP3.modulate = Color(0,0,0,1)
		$HP2.modulate = Color(0,0,0,1)
		$HP1.modulate = Color(1,1,1,1)
	if PlayerHealth == 0:
		$HP5.modulate = Color(0,0,0,1)
		$HP4.modulate = Color(0,0,0,1)
		$HP3.modulate = Color(0,0,0,1)
		$HP2.modulate = Color(0,0,0,1)
		$HP1.modulate = Color(0,0,0,1)
