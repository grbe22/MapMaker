using Godot;
using System;

public partial class MapGrid : Sprite2D
{
	private const int CELL_SIZE = 32;
	float[,] map;
	int edgeSize = 150;
	int perlinScale = 5;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Perlin mapMaker = new Perlin(edgeSize, perlinScale);
		map = mapMaker.PerlinGenerator();
		
		GenerateGrid();
		Position = new Vector2(0, 0);
	}

	private void GenerateGrid() {
		Image image = Image.Create(edgeSize * CELL_SIZE, edgeSize * CELL_SIZE, false, Image.Format.Rgba8);
		// textures for later
		Image oceanTexture = GD.Load<Image>("res://Assets/Tiles/ocean.png");
		Image groundTexture = GD.Load<Image>("res://Assets/Tiles/ground.png");
		Image beachTexture = GD.Load<Image> ("res://Assets/Tiles/beach.png");

		// Loop through the grid data and draw cells
		for (int y = 0; y < edgeSize; y++) {
			for (int x = 0; x < edgeSize; x++) {
				// Calculate the grayscale value
				float value = map[y, x];
				
				// Choose texture based on grayscale value
				Image texture = value < 0.5f ? oceanTexture : groundTexture;
				if (value > .5f && value < .55f) {
					texture = beachTexture;
				}
				// Draw the texture onto the image
				DrawTextureOnImage(image, new Vector2(x * CELL_SIZE, y * CELL_SIZE), texture);
			}
		}
		Texture tex = new ImageTexture();
		tex = ImageTexture.CreateFromImage(image);
		Texture = (Texture2D)tex;
	}
	
	private void DrawTextureOnImage(Image image, Vector2 position, Image texture) {
		// Get the texture's data

		// Loop through the texture's pixels and copy them to the image
		for (int y = 0; y < texture.GetHeight(); y++)
		{
			for (int x = 0; x < texture.GetWidth(); x++)
			{
				Color color = texture.GetPixel(x, y);
				image.SetPixel((int)(position.X + x), (int)(position.Y + y), color);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
