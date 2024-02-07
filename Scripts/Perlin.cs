using Godot;
using System;
using System.Numerics;

public static class Perlin {
	int mapSize;
	int gradientSize;
    int seed;

	float gradient[,,];
    float xValues[];
    float yValues[];

    float noise[,];

	// this is for seeding the run - you pass the seed as an integer value.
	public Perlin(int mapSize, int perlinSize, int seed)
    {
        this.seed = seed;
        this.mapSize = mapSize;
        this.gradientSize = perlinSize;

        Random perlinBuilder = new Random(seed);
        // I don't understand why I used +1 in python, but it doesn't run without it.
        // the third is so that it holds Vector2s.
        gradient = new float[perlinSize + 1, perlinSize + 1, 2];

        // builds the gradient randomly with help of the seed and size of the perlin.
        for (int i = 0; i < perlinSize + 1; i++) {
            for (int j = 0; j < perlinSize + 1; j++) {
                gradient[i, j, 0] = perlinBuilder.Next();
                gradient[i, j, 1] = perlinBuilder.Next();
            }
        }

        // builds the x and y values. These are the positions on the board.
        xValues = new float[mapSize];
        yValues = new float[mapSize];
        // iterates through both lists, assigning them to (perlinSize / mapSize) * i 
        for (int i = 0; i < mapSize; i++) {
            float equitablePos = (perlinSize / mapSize) * i;
            xValues[i] = equitablePos;
            yValues[i] = equitablePos;
        }

        // builds the empty noise array.
        noise = new float[mapSize,mapSize];
        for (int i = 0; i < mapSize; i ++) {
            for (int j = 0; j < mapSize; j++) {
                noise[i, j] = perlin(xValues[i], yValues[j]);
            }
        }
        GD.Print(noise);
    }

	// all we need to take as input is the map size, and the perlinsize.
	// larger perlinsize means more, smaller islands.
    // this constructor creates its own seed.
	public Perlin(int mapSize, int perlinSize) {
        Random rand = new Random();
        // I'm pretty sure I can make this better... but I don't care enough to.
        Perlin(mapSize, perlinSize, rand.Next);
    }

    // simple helper function that helps ease the transition between sections.
    public float Smooth(float val)
    {
        return (val * val * (3 - (2 * val)));
    }

    // special function that calculates the dot product between an edge, and the distance.
    public float DotProduct(Vector2 edge, Vector2 point)
    {
        float xDiff = edge.x - point.x;
        float yDiff = edge.y - point.y;
        
        // finds the x and y values of the targeted edge.
        float x = gradient[edge.x, edge.y, 0];
        float y = graident[edge.x, edge.y, 1];
        
        // returns the dot product, plain and simple.
        return (xDiff * x + yDiff * y);
    }

    // actually generates the noise
    public float perlin(float x, float y) {
        // generates int values
        float left = (float)(int)x;
        float top = (float)(int)y;

        // runs smooth - negates edges very well.
        float xSmooth = Smooth(x - left);
        float ySmooth = Smooth(y - left);

        // calculates noise along the edge, with respect to the next edge.
        float ltDot = DotProduct(new Vector2(left, top), new Vector2(x, y));
        float rtDot = DotProduct(new Vector2(left + 1, top), new Vector2(x, y));
        float lbDot = DotProduct(new Vector2(left, top + 1), new Vector2(x, y));
        float rbDot = DotProduct(new Vector2(left + 1, top + 1), new Vector2(x, y));

        float topNoise = ltDot + xSmooth * (rtDot - ltDot);
        float bottomNoise = lbDot + xSmooth * (rbDot - ltDot);

        return (topNoise + ySmooth * (bottomNoise - topNoise);
    }
}
