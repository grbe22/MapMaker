using Godot;
using System;
using System.Diagnostics;
using System.Threading;

public partial class MapGrid : Sprite2D
{
	int edgeSize = 500;
	// a larger ratio results in smaller, smoother blobs
	int perlinScale;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
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
		GD.Print("HeightMap Complete at time ", timer.Elapsed);
		// heatMap is for generating the temperature of the terrain - low temperature forms ice, high forms badlands.
		float[,] heatMap;
		mapMaker.UpdatePerlinMap(perlinScale);
		heatMap = mapMaker.PerlinGenerator(false);
		GD.Print("HeatMap Complete at ", timer.Elapsed);
		// moistureMap isn't really for moisture, but I dont know what a better name would be.
		// High values generate forest & overgrown areas, low generate swamps and "murky" areas.
		float[,] moistureMap;
		mapMaker.UpdatePerlinMap(perlinScale * 4);
		moistureMap = mapMaker.PerlinGenerator(false);
		GD.Print("MoistureMap Complete at ", timer.Elapsed);
		
		// generates the map using these three maps.
		GenerateGrid(heightMap, heatMap, moistureMap, timer);
		GD.Print("Texture loaded at ", timer.Elapsed);
		Position = new Vector2(0, 0);
	}

	private void GenerateGrid(float[,] heightMap, float[,] heatMap, float[,] moistureMap, Stopwatch timer) {
		// Loop through the grid data and paste cells
		// loads the TileMap
		TileMap foundation = (TileMap)GetChild(0);
		for (int y = 0; y < edgeSize; y++) {
			if (y % 25 == 0) {
				GD.Print("Y-Value ", y, " was completed at ", timer.Elapsed);
			}
			for (int x = 0; x < edgeSize; x++) {
				// creates the 
				int tileId;
				
				// Grab the info for the tile from the three arrays
				float heightValue = heightMap[y, x];
				float heatValue = heatMap[y, x];
				float moistureValue = moistureMap[y, x];
				// Choose texture based on each array
				/*
					0 - grassland
					1 - water
					2 - ice
					3 - badlands
					4 - tundra
					5 - beach
					6 - swamp
					7 - forest
					8 - mountain
				*/
				if (heightValue < .5) {

					// generates ocean, or icebergs in very cold climates.
					if (heatValue < .12) { tileId = 2; }

					else { tileId = 1; }
				} else if (heightValue < .55) {

					// A small window that generates coastline. In exceedingly cold areas, generates tundra, and generates
					// badlands in very high temperatures.
					if (heatValue < .17) { tileId = 4; } 

					else if (heatValue > .83) { tileId = 3; } 

					else { tileId = 5; }
				} else if (heightValue < .92 || moistureValue > .7) {
					
					// generates extreme temperatures in higher values. Badlands are moderately hot, tundra is moderately cold.
					if (heatValue < .2) { tileId = 4; }
					
					else if (heatValue > .8) { tileId = 3; } 

					else {

						// this is the default case which covers ~.6 of the tileSpan - it's distributed Gaussian-ly, so it's
						// much more than just 60% of the tiles between .55 and .92.
						// generates swamp in low temperatures, and forest in high temperatures.
						if (moistureValue < .35) { tileId = 6; } 

						else if (moistureValue < .65) { tileId = 0; } 
						
						else { tileId = 7; }
					}
				} else {
					
					// in VERY high temperatures, generates badlands, otherwise, generates mountains.
					if (heatValue > .85) { tileId = 3; } 
					
					// mountains are very round blobs. I want SCRATCHES, like long canals of mountain, but I don't
					// know how to go about this right now.
					else { tileId = 8; }
				}
				// Draw the texture onto the image
				PasteTexture(new Vector2I(2 * x - edgeSize, 2 * y - edgeSize), tileId, foundation);
			}
		}
		GD.Print(foundation.GetCellAtlasCoords(0, new Vector2I(0, 0)));
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
