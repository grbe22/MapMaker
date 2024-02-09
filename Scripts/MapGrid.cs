using Godot;
using System;
using System.Diagnostics;
using System.Threading;

public partial class MapGrid : Sprite2D
{
	int edgeSize = 2;
	// a larger ratio results in smaller, smoother blobs
	int perlinScale;
	TileSetter.Tiles[,] map;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		map = new TileSetter.Tiles[edgeSize, edgeSize];
		perlinScale = (int)(edgeSize / 50);
		if (perlinScale < 2) {
			perlinScale = 2;
		}
		Perlin mapMaker = new Perlin(edgeSize, perlinScale);
		var timer = new Stopwatch();
		timer.Start();
		// heightMap is for generating sea level and terrain level.
		float[,] heightMap;
		heightMap = mapMaker.PerlinGenerator(true);
		// heatMap is for generating the temperature of the terrain - low temperature forms ice, high forms badlands.
		float[,] heatMap;
		mapMaker.UpdatePerlinMap(perlinScale);
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
		// Loop through the grid data and paste cells
		// loads the TileMap
		TileMap foundation = (TileMap)GetChild(0);
		for (int y = 0; y < edgeSize; y++) {
			for (int x = 0; x < edgeSize; x++) {
				// creates the 
				int tileId;
				
				// Grab the info for the tile from the three arrays
				float height = heightMap[y, x];
				float heat = heatMap[y, x];
				float moisture = moistureMap[y, x];
				TileSetter.Tiles meterMaid = TileSetter.tileFromArrays(height, heat, moisture);
				tileId = (int)meterMaid;
				map[y, x] = meterMaid;
				// Choose texture based on each array
				// Draw the texture onto the image
				PasteTexture(new Vector2I(2 * x - edgeSize, 2 * y - edgeSize), tileId, foundation);
			}
		}
	}
	
	private void PasteTexture(Vector2I pos, int tileId, TileMap foundation) {
		Vector2I atlasLoc = new Vector2I(tileId % 8, (int)(tileId / 8));
		foundation.SetCell(0, pos, 0, atlasLoc, 0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
