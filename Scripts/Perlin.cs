using Godot;
using System;
using System.Threading.Tasks;

public partial class Perlin {
	public static int mapSize;
	int gradientSize;
	
	static Random perlinBuilder;

	float[,,] gradient;
	float[] xValues;
	float[] yValues;

	float[,] noise;

	// this is for seeding the run - you pass the seed as an integer value.
	public Perlin(int _mapSize, int perlinSize, int seed) {
		mapSize = _mapSize;
		gradientSize = perlinSize;
		perlinBuilder = new Random(seed);
		// PerlinGenerator();
	}

	// all we need to take as input is the map size, and the perlinsize.
	// larger perlinsize means more, smaller islands.
	// this constructor creates its own seed.
	public Perlin(int _mapSize, int perlinSize) {
		mapSize = _mapSize;
		gradientSize = perlinSize;
		perlinBuilder = new Random();
		// I'm pretty sure I can make this better... but I don't care enough to.
		// PerlinGenerator();
	}

	public float[,] PerlinGenerator (bool centralized) {
		// I don't understand why I used +1 in python, but it doesn't run without it.
		// the third is so that it holds Vector2s.
		gradient = new float[gradientSize + 1, gradientSize + 1, 2];
		// builds the gradient randomly with help of the seed and size of the perlin.
		for (int i = 0; i < gradientSize + 1; i++) {
			for (int j = 0; j < gradientSize + 1; j++) {
				gradient[i, j, 0] = (float)perlinBuilder.NextDouble();
				gradient[i, j, 1] = (float)perlinBuilder.NextDouble();
			}
		}

		// builds the x and y values. These are the positions on the board.
		xValues = new float[mapSize];
		yValues = new float[mapSize];
		// iterates through both lists, assigning them to (perlinSize / mapSize) * i 
		for (int i = 0; i < mapSize; i++) {
			float equitablePos = ((float)gradientSize / (float)mapSize) * (float)i;
			xValues[i] = equitablePos;
			yValues[i] = equitablePos;
		}
		
		float min = 10f;
		float max = -10f;
		// builds the empty noise array.
		noise = new float[mapSize,mapSize];
		int center = (int)(mapSize / 2);
		// fills the noise array
		Parallel.For(0, mapSize, i => {
			// this is already run in parallel; no sense in running it again.
			// I don't think any major benefit can be attained by
			// using Parallel.For inside a Parallel.For. It's already using the max num of threads.
			for (int j = 0; j < mapSize; j++) {
				float output = NoiseMaker(xValues[i], yValues[j]);
				if (centralized) {
					float left = (center - j) * (center - j);
					float right = (center - i) * (center - i);
					float maxDist = (float)(Math.Sqrt(left + right)) * 1.5f;
					// .75f influences how spread out the land is
					// higher means more centralized, lower means less.
					// negative would make the edge land and center water.
					// if that means anything.
					output -= .75f * maxDist / mapSize;
					// output -= maxDist / mapSize;
				}
				noise[i, j] = output;
				if (output > max) {
					max = output;
				} 
				if (output < min) {
					min = output;
				}
			}
		});
		// normalizes the data between 0 and 1
		float difference = max - min;
		Parallel.For (0, mapSize, i => {
			for (int j = 0; j < mapSize; j++) {
				float thisOne = noise[i, j];
				thisOne = (thisOne - min) / difference;
				noise[i, j] = thisOne;
			}
		});
		return noise;
	}

	// when you want to change the number of perlinys 
	public void UpdatePerlinMap(int size) {
		gradientSize = size;
	}
	
	// simple helper function that helps ease the transition between sections.
	public float Smooth(float val){
		return (val * val * (3 - (2 * val)));
	}

	// special function that calculates the dot product between an edge, and the distance.
	public float DotProduct(Vector2 edge, Vector2 point){
		float xDiff = edge.X - point.X;
		float yDiff = edge.Y - point.Y;
		// finds the x and y values of the targeted edge.
		float x = gradient[(int)edge.X, (int)edge.Y, 0];
		float y = gradient[(int)edge.X, (int)edge.Y, 1];
		
		// returns the dot product, plain and simple.
		return (xDiff * x + yDiff * y);
	}

	// actually generates the noise
	public float NoiseMaker(float x, float y) {
		// generates int values
		float left = (float)(int)x;
		float top = (float)(int)y;
		
		// runs smooth - negates edges very well.
		float xSmooth = Smooth((float)x - left);
		float ySmooth = Smooth((float)y - top);

		// calculates noise along the edge, with respect to the next edge.
		float ltDot = DotProduct(new Vector2(left, top), new Vector2(x, y));
		float rtDot = DotProduct(new Vector2(left + 1, top), new Vector2(x, y));
		float lbDot = DotProduct(new Vector2(left, top + 1), new Vector2(x, y));
		float rbDot = DotProduct(new Vector2(left + 1, top + 1), new Vector2(x, y));
		
		float topNoise = ltDot + xSmooth * (rtDot - ltDot);
		float bottomNoise = lbDot + xSmooth * (rbDot - lbDot);

		return (topNoise + ySmooth * (bottomNoise - topNoise));
	}
	
	public static Vector2I ServeRandomGrass(TileSetter.Tiles[,] tiles) {
		while (true) {
			// center position
			int displacement = mapSize / 2;
			int xPos = perlinBuilder.Next(displacement / 2, 3 * displacement / 2);
			int yPos = perlinBuilder.Next(displacement / 2, 3 * displacement / 2);
			// i dont know where I went wrong in ths. But I have to put xPos, Ypos for
			// the conditional and yPos, xPos for the output.
			if (tiles[xPos, yPos] == TileSetter.Tiles.Grassland) {
				return new Vector2I(yPos, xPos);
			}
		}
	}
	
	// finnessTiles contains the tiles. -1 is invalid - wrong terrain or already checked.
	// 0 is valid and unchecked.
	// 1 is curse.
	public static void FinnesseTiles(int maxDepth, int[,] validTiles, Vector2I startingPos, int depth, int size) {
		if (maxDepth <= depth) {
			return;
		}
		Vector2I[] orthags = GetValidOrthagonal(validTiles, startingPos, size);
		int pointer = 0;
		int[] isValid = new int[orthags.Length];
		foreach (Vector2I orth in orthags) {
			double probability = Math.Pow(.95, depth - 1);
			double randGen = perlinBuilder.NextDouble();
			if (probability > randGen) {
				validTiles[orth.Y, orth.X] = 1;
				isValid[pointer] = 1;
			} else {
				if (perlinBuilder.NextDouble() > .3) {
					validTiles[orth.Y, orth.X] = -1;
				} else {
					validTiles[orth.Y, orth.X] = 2;
				}
				isValid[pointer] = 0;
			}
			pointer += 1;
		}
		pointer = 0;
		foreach (Vector2I orth in orthags) {
			if (isValid[pointer] == 1) {
				FinnesseTiles(maxDepth, validTiles, orth, depth + 1, size);
			}
			pointer += 1;
		}
		string nape = "";
		int loop = 0;
	}
	
	public static Vector2I[] GetValidOrthagonal(int[,] ValidTiles, Vector2I origin, int size) {
		Vector2I[] orthags = new Vector2I[4];
		int numValid = 0;
		Vector2I curr = new Vector2I(origin.X - 1, origin.Y);
		if (ValidTiles[curr.Y, curr.X] == 0) {
			orthags[numValid] = curr;
			numValid += 1;
		}
		curr = new Vector2I(origin.X + 1, origin.Y);
		if (ValidTiles[curr.Y, curr.X] == 0) {
			orthags[numValid] = curr;
			numValid += 1;
		}
		curr = new Vector2I(origin.X, origin.Y - 1);
		if (ValidTiles[curr.Y, curr.X] == 0) {
			orthags[numValid] = curr;
			numValid += 1;
		}
		curr = new Vector2I(origin.X, origin.Y + 1);
		if (ValidTiles[curr.Y, curr.X] == 0) {
			orthags[numValid] = curr;
			numValid += 1;
		}
		Vector2I[] trimmedOrthags = new Vector2I[numValid];
		for (int i = 0; i < numValid; i++) {
			trimmedOrthags[i] = orthags[i];
		}
		return trimmedOrthags;
	}
	
}
