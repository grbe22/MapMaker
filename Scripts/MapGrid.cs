using Godot;
using System;
using System.Linq;
using System.Diagnostics;

public partial class MapGrid : Sprite2D
{
	// size of each vertex of the map
	// most map generators for colonization games cap out around 100-200 x 100-200
	// it runs slow @ > 600, but that's not a realistic problem.
	private int edgeSize = 64;
	// ratio between the map and the perlinMap
	// a larger ration results in smaller, smoother blobs.
	private const int ratio = 32;
	// results in smaller blobs of "heat"
	private const int heatFactor = 2;
	// results in much smaller patches of swamp / forest
	private const int moistureFactor = 4;
	int perlinScale;
	TileSetter.Tiles[,] map;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CreateHeatMaps();
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (Input.IsActionJustPressed("Reload")) {
			CreateHeatMaps();
		}
	}
		
	// creates a new set of maps, and updates the old one.
	public void CreateHeatMaps() {
		
		float centralized = (float)GetNode<HSlider>("../MapKeys/Centralization/cSlider").Value;
		edgeSize = (int)GetNode<HSlider>("../MapKeys/MapSize/mSlider").Value;
		
		map = new TileSetter.Tiles[edgeSize, edgeSize];
		perlinScale = (int)(edgeSize / ratio);
		if (perlinScale < 2) {
			perlinScale = 2;
		}
		Perlin mapMaker = new Perlin(edgeSize, perlinScale);
		// heightMap is for generating sea level and terrain level.
		float[,] heightMap;
		heightMap = mapMaker.PerlinGenerator(centralized);
		// heatMap is for generating the temperature of the terrain - low temperature forms ice, high forms badlands.
		float[,] heatMap;
		mapMaker.UpdatePerlinMap(perlinScale * heatFactor);
		heatMap = mapMaker.PerlinGenerator(0.0f);
		// moistureMap isn't really for moisture, but I dont know what a better name would be.
		// High values generate forest & overgrown areas, low generate swamps and "murky" areas.
		float[,] moistureMap;
		mapMaker.UpdatePerlinMap(perlinScale * moistureFactor);
		moistureMap = mapMaker.PerlinGenerator(0.0f);
		
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
			}
		}
		// Draw the texture onto the image
		PasteTexture(map, foundation);
	}
	
	private void PasteTexture(TileSetter.Tiles[,] map, TileMap foundation) {
		for (int y = 0; y < edgeSize; y++) {
			for (int x = 0; x < edgeSize; x++) {
				int tileId = (int)map[y,x];
				int half = edgeSize / 2;
				Vector2I atlasLoc = new Vector2I(tileId % 8, (int)(tileId / 8));
				foundation.SetCell(0, new Vector2I(y - half, x - half), 0, atlasLoc, 0);
			}
		}
	}
}
