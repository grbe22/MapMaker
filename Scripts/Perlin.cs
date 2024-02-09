using Godot;
using System;

public partial class Perlin {
	public static int mapSize;
	int gradientSize;
	
	Random perlinBuilder;

	float[,,] gradient;
	float[] xValues;
	float[] yValues;

	float[,] noise;

	// this is for seeding the run - you pass the seed as an integer value.
	public Perlin(int _mapSize, int perlinSize, int seed)
	{
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
		for (int i = 0; i < mapSize; i ++) {
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
		}
		// normalizes the data between 0 and 1
		float difference = max - min;
		for (int i = 0; i < mapSize; i++) {
			for (int j = 0; j < mapSize; j++) {
				float thisOne = noise[i, j];
				thisOne = (thisOne - min) / difference;
				noise[i, j] = thisOne;
			}
		}
		return noise;
	}

	// when you want to change the number of perlinys 
	public void UpdatePerlinMap(int size) {
		gradientSize = size;
	}
	
	// simple helper function that helps ease the transition between sections.
	public float Smooth(float val)
	{
		return (val * val * (3 - (2 * val)));
	}

	// special function that calculates the dot product between an edge, and the distance.
	public float DotProduct(Vector2 edge, Vector2 point)
	{
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
}
