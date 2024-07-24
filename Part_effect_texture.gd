extends CPUParticles2D


# Called when the node enters the scene tree for the first time.
func _ready():
	var sprites = get_parent().find_children("*", "Sprite2D")
	var newEmission = []	
	if sprites.size() == 0:
		var Asprites = get_parent().find_children("*", "AnimatedSprite2D")
		var sFrame = Asprites[0].sprite_frames
		var textureAtlas = sFrame.get_frame_texture("idle", 0)
		#var img_tex = ImageTexture.new();
		#img_tex.create_from_image(textureAtlas.atlas.get_image().get_region(textureAtlas.region));
		#img_tex = img_tex.create_from_image(textureAtlas.atlas.get_image());
		var img_tex = textureAtlas.atlas.get_image()
		var msize = img_tex.get_size()
		print(msize)
		for i in range(textureAtlas.region.position.x, textureAtlas.region.end.x):
			print(i)
			for j in range(textureAtlas.region.position.y, textureAtlas.region.end.y):
				if img_tex.get_pixel(i,j).a != 0:
					newEmission.push_back(Vector2(i-(textureAtlas.region.size.x/2),j-(textureAtlas.region.size.y/2)))
	else:
		var sprite = sprites[0] as Sprite2D
		var tscale = sprite.transform.get_scale()
		var text = sprite.texture as Texture2D  
		var msize = text.get_size()
		for i in range(-msize.x/2, msize.x/2):
			for j in range(-msize.y/2, msize.y/2):
				if sprite.is_pixel_opaque(Vector2(i,j)):
					newEmission.push_back(Vector2(i*tscale.x,j*tscale.y))
	set_emission_points(newEmission)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
