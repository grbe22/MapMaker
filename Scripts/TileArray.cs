using Godot;
using System;
using System.Threading.Tasks.Dataflow;

public partial class TileArray : Node
{
	
	enum TileTypes
	{
		Void = 0,
		Ocean = 1,
		Ground = 2
	}

	/*
		this is for testing out basic map generation.
		generates a 6x6 array of elements
	*/
	private TileTypes[,] tiles;
	private Random randomTileGen;


	public TileArray(int x, int y) {
		tiles = new TileTypes[x, y];
		randomTileGen = new Random();
		// fills the tile array with Ocean for the first iteration
		for (int i = 0; i < x; i++) {
			for (int j = 0; j < y; j++) {
				tiles[i, j] = TileTypes.Ocean;
			}
		}
		int size = (int)(x * .5) + 1;
		TileTypes[,] newIsland = GenerateIsland(size);
		int initX = randomTileGen.Next(0, (x - size));
		int initY = randomTileGen.Next(0, (y - size));

		// this fills the area starting at initX with tile stuff
		for (int i = 0; i < size; i ++) {
			for (int j = 0; j < size; j++) {
				if (newIsland[i, j] != TileTypes.Void) {
					tiles[i + initX, j + initY] = newIsland[i, j];
				}
			}
		}
		PrintIsland(tiles);
	}

	private TileTypes[,] GenerateIsland(int bounds) {
		// it'll fill a square of length 2n with 50% ground tiles
		TileTypes[,] islandMap = new TileTypes[bounds, bounds];
		for (int i = 0; i < bounds; i++) {
			for (int j = 0; j < bounds; j++) {
				if (randomTileGen.Next(0, 2) == 1)
				{
					islandMap[i,j] = TileTypes.Ground;
				}
				else
				{
					islandMap[i, j] = TileTypes.Void;
				}
			}
		}
		return islandMap;
	}
	
	private void PrintIsland(TileTypes[,] printer) {
		int x = printer.GetLength(0);
		int y = printer.GetLength(1);
		for (int i = 0; i < x; i ++) {
			string thisLine = "";
			for (int j = 0; j < y; j++) {
				thisLine = thisLine + (int)printer[i, j] + " ";
			}
		}
	}

}
