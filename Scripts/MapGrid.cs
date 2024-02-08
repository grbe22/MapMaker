using Godot;
using System;

public partial class MapGrid : Sprite2D
{
	private const int CELL_SIZE = 32;
	int edgeSize = 150;
	int perlinScale = 7;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Perlin mapMaker = new Perlin(edgeSize, perlinScale);
		float[,] heightMap;
		heightMap = mapMaker.PerlinGenerator(true);
		float[,] heatMap;
		mapMaker.UpdatePerlinMap(perlinScale / 2);
		heatMap = mapMaker.PerlinGenerator(false);
		float[,] moistureMap;
		mapMaker.UpdatePerlinMap(perlinScale * 4);
		moistureMap = mapMaker.PerlinGenerator(false);
		
		GenerateGrid(heightMap, heatMap, moistureMap);
		Position = new Vector2(0, 0);
	}

	private void GenerateGrid(float[,] heightMap, float[,] heatMap, float[,] moistureMap) {
		Image image = Image.Create(edgeSize * CELL_SIZE, edgeSize * CELL_SIZE, false, Image.Format.Rgba8);
		// textures for later
		Image oceanTexture = GD.Load<Image>("res://Assets/Tiles/ocean.png");
		Image groundTexture = GD.Load<Image>("res://Assets/Tiles/ground.png");
		Image beachTexture = GD.Load<Image> ("res://Assets/Tiles/beach.png");
		Image mountainTexture = GD.Load<Image> ("res://Assets/Tiles/mountain.png");
		Image iceTexture = GD.Load<Image> ("res://Assets/Tiles/ice.png");
		Image tundraTexture = GD.Load<Image> ("res://Assets/Tiles/tundra.png");
		Image badlandsTexture = GD.Load<Image> ("res://Assets/Tiles/badlands.png");
		Image forestTexture = GD.Load<Image> ("res://Assets/Tiles/forest.png");
		Image swampTexture = GD.Load<Image> ("res://Assets/Tiles/swamp.png");

		// Loop through the grid data and draw cells
		for (int y = 0; y < edgeSize; y++) {
			for (int x = 0; x < edgeSize; x++) {
				// Calculate the grayscale value
				float heightValue = heightMap[y, x];
				float heatValue = heatMap[y, x];
				float moistureValue = moistureMap[y, x];
				// Choose texture based on grayscale value
				Image imagery;
				if (heightValue < .5) {
					if (heatValue < .23) {
						imagery = iceTexture;
					} else {
						imagery = oceanTexture;
					}
				} else if (heightValue < .55) {
					if (heatValue < .2) {
						imagery = tundraTexture;
					} else {
						imagery = beachTexture;
					}
				} else if (heightValue < .92) {
					if (heatValue < .25) {
						imagery = tundraTexture;
					} else if (heatValue > .75){
						imagery = badlandsTexture;
					} else {
						if (moistureValue < .35) {
							imagery = swampTexture;
						} else if (moistureValue < .65) {
							imagery = groundTexture;
						} else {
							imagery = forestTexture;	
						}
					}
				} else {
					if (heatValue > .85) {
						imagery = badlandsTexture;
					} else {
						imagery = mountainTexture;
					}
				}
				// Draw the texture onto the image
				DrawTextureOnImage(image, new Vector2(x * CELL_SIZE, y * CELL_SIZE), imagery);
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
