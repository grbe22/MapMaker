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
		// heightMap is for generating sea level and terrain level.
		float[,] heightMap;
		heightMap = mapMaker.PerlinGenerator(true);
		// heatMap is for generating the temperature of the terrain - low temperature forms ice, high forms badlands.
		float[,] heatMap;
		mapMaker.UpdatePerlinMap(perlinScale / 2);
		heatMap = mapMaker.PerlinGenerator(false);
		// moistureMap isn't really for moisture, but I dont know what a better name would be.
		// High values generate forest & overgrown areas, low generate swamps and "murky" areas.
		float[,] moistureMap;
		mapMaker.UpdatePerlinMap(perlinScale * 4);
		moistureMap = mapMaker.PerlinGenerator(false);
		
		// generates the map using these three maps.
		GenerateGrid(heightMap, heatMap, moistureMap);
		Position = new Vector2(0, 0);
	}

	private void GenerateGrid(float[,] heightMap, float[,] heatMap, float[,] moistureMap) {
		Image image = Image.Create(edgeSize * CELL_SIZE, edgeSize * CELL_SIZE, false, Image.Format.Rgba8);
		// textures for each terrain type.
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
				
				// Grab the info for the tile from the three arrays
				
				float heightValue = heightMap[y, x];
				float heatValue = heatMap[y, x];
				float moistureValue = moistureMap[y, x];

                Image imagery;
                
				// Choose texture based on each array
                if (heightValue < .5) {

					// generates ocean, or icebergs in very cold climates.
					if (heatValue < .16) { imagery = iceTexture; }

					else { imagery = oceanTexture; }

				} else if (heightValue < .55) {

					// A small window that generates coastline. In exceedingly cold areas, generates tundra, and generates
					// badlands in very high temperatures.
					if (heatValue < .20) { imagery = tundraTexture; } 

					else if (heatValue > .80) { imagery = badlandsTexture; } 

					else { imagery = beachTexture; }
				} else if (heightValue < .92) {
					
					// generates extreme temperatures in higher values. Badlands are moderately hot, tundra is moderately cold.
					if (heatValue < .2) { imagery = tundraTexture; }
					
					else if (heatValue > .8) { imagery = badlandsTexture; } 

					else {

						// this is the default case which covers ~.6 of the tileSpan - it's distributed Gaussian-ly, so it's
						// much more than just 60% of the tiles between .55 and .92.
						// generates swamp in low temperatures, and forest in high temperatures.
						if (moistureValue < .35) { imagery = swampTexture; } 

						else if (moistureValue < .65) { imagery = groundTexture; } 
						
						else { imagery = forestTexture;	}
					}
				} else {
					
					// in VERY high temperatures, generates badlands, otherwise, generates mountains.
					if (heatValue > .85) { imagery = badlandsTexture; } 
					
					else {

						// in wet areas, generates forests. I want to "break up" mountains, turn them from
						// round blobs into LONG blobs. This isn't how I should do it.
						if (moistureValue > .7) { imagery = forestTexture; }
						else { imagery = mountainTexture; }
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
