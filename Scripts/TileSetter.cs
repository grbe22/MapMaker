using Godot;
using System;

public partial class TileSetter : Node
{
	// holds all defined tiles
	public enum Tiles {
		None = -1,
		Grassland = 0,
		Water = 1,
		Ice = 2,
		Badlands = 3,
		Tundra = 4,
		Beach = 5,
		Swamp = 6,
		Forest = 7,
		Mountain = 8,
		CurseBody = 9,
		CurseHead = 10
	}
	
	// takes three float inputs and outputs a single enum value
	public static Tiles tileFromArrays(float heightValue, float heatValue, float moistureValue) {
		if (heightValue < .5) {
			if (heatValue > .75 && heightValue < .26) { return GenFertile(moistureValue); }
			if (heatValue > .72 && heightValue < .31) { return Tiles.Beach; }
			// generates ocean, or icebergs in very cold climates.
			if (heatValue < .12) { return Tiles.Ice; }
			else { return Tiles.Water; }
		} else if (heightValue < .53) {
			// A small window that generates coastline. In exceedingly cold areas, generates tundra, and generates
			// badlands in very high temperatures.
			if (heatValue < .17) { return Tiles.Tundra; } 
			else if (heatValue > .83) { return Tiles.Badlands; } 
			else { return Tiles.Beach; }
		} else if (heightValue < .92 || moistureValue > .7) {
			// generates extreme temperatures in higher values. Badlands are moderately hot, tundra is moderately cold.
			if (heatValue < .2) { return Tiles.Tundra; }
			else if (heatValue > .8) { return Tiles.Badlands; } 
			else {
				// this is the default case which covers ~.6 of the tileSpan - it's distributed Gaussian-ly, so it's
				// much more than just 60% of the tiles between .55 and .92.
				// generates swamp in low temperatures, and forest in high temperatures.
				return GenFertile(moistureValue);
			}
		} else {
			// in VERY high temperatures, generates badlands, otherwise, generates mountains.
			if (heatValue > .85) { return Tiles.Badlands; } 
			// mountains are very round blobs. I want SCRATCHES, like long canals of mountain, but I don't
			// know how to go about this right now.
			else { return Tiles.Mountain; }
		}
	}
	
	public static Tiles GenFertile(float moistureValue) {
		if (moistureValue < .35) { return Tiles.Swamp; } 
		else if (moistureValue < .65) { return Tiles.Grassland; } 
		return Tiles.Forest;
	}
	
}
