using Godot;
using System;
using System.Linq;
using System.Diagnostics;

public partial class MapGrid : Sprite2D
{
	// size of each vertex of the map
	// most map generators for colonization games cap out around 100-200 x 100-200
	// it runs HELLA slow > 600, but that's not a realistic problem.
	private const int edgeSize = 1024;
	private const int curseBlooms = 2;
	// ratio between the map and the perlinMap
	// a larger ration results in smaller, smoother blobs.
	private const int ratio = 50;
	private const int heatFactor = 2;
	private const int moistureFactor = 4;
	int perlinScale;
	TileSetter.Tiles[,] map;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Stopwatch timer = new Stopwatch();
		timer.Start();
		map = new TileSetter.Tiles[edgeSize, edgeSize];
		perlinScale = (int)(edgeSize / ratio);
		if (perlinScale < 2) {
			perlinScale = 2;
		}
		Perlin mapMaker = new Perlin(edgeSize, perlinScale);
		// heightMap is for generating sea level and terrain level.
		float[,] heightMap;
		heightMap = mapMaker.PerlinGenerator(true);
		// heatMap is for generating the temperature of the terrain - low temperature forms ice, high forms badlands.
		float[,] heatMap;
		mapMaker.UpdatePerlinMap(perlinScale * heatFactor);
		heatMap = mapMaker.PerlinGenerator(false);
		// moistureMap isn't really for moisture, but I dont know what a better name would be.
		// High values generate forest & overgrown areas, low generate swamps and "murky" areas.
		float[,] moistureMap;
		mapMaker.UpdatePerlinMap(perlinScale * moistureFactor);
		moistureMap = mapMaker.PerlinGenerator(false);
		
		// generates the map using these three maps.
		GenerateGrid(heightMap, heatMap, moistureMap);
		Position = new Vector2(0, 0);
		timer.Stop();
		GD.Print(timer.Elapsed);
	}

	private void GenerateGrid(float[,] heightMap, float[,] heatMap, float[,] moistureMap) {
		// Loop through the grid data and paste cells
		// loads the TileMap
		TileMap foundation = (TileMap)GetChild(0);
		Vector2I curseTile = new Vector2I(-1, -1);
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
		// max side length of the curse spread
		// should be an odd number > 1
		int curseMax = 15;
		for (int i = 0; i < curseBlooms; i++) {
			// serves a random... grass. A grassland tile "near" the center of the map.
			curseTile = Perlin.ServeRandomGrass(map);
			map[curseTile.Y, curseTile.X] = TileSetter.Tiles.CurseBody;
			GrowCurse(map, curseTile, curseMax);
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

	private void GrowCurse(TileSetter.Tiles[,] map, Vector2I curseTile, int curseMax) {
		// generates a 2d List
		int[,] validTiles = new int[curseMax, curseMax];
		int startX = curseTile.X - curseMax / 2;
		int startY = curseTile.Y - curseMax / 2;
		TileSetter.Tiles[] validTile = new[] {
			TileSetter.Tiles.Grassland, 
			TileSetter.Tiles.Swamp,
			TileSetter.Tiles.Forest,
			TileSetter.Tiles.Beach
		};
		for (int y = 0; y < curseMax; y++) {
			for (int x = 0; x < curseMax; x ++) {
				if (validTile.Contains(map[y + startY,x + startX])) {
					validTiles[y, x] = 0;
				} else {
					validTiles[y, x] = -1;
				}
			}
		}
		validTiles[curseMax / 2, curseMax / 2] = 1;
		Perlin.FinnesseTiles(curseMax / 2, validTiles, new Vector2I(curseMax/2, curseMax/2), 1, curseMax);
		// here's how we implement this:
		// we use the function, FinnesseTiles, to generate an nxn "body" for the curse.
		for (int y = 0; y < curseMax; y++) {
			for (int x = 0; x < curseMax; x++) {
				if (validTiles[y,x] == 1) {
					map[y + startY, x + startX] = TileSetter.Tiles.CurseBody;
				}
				if (validTiles[y,x] == 2) {
					map[y + startY, x + startX] = TileSetter.Tiles.CurseHead;
				}
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
