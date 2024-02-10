using Godot;
using System;

public partial class WorldMap : Node2D
{
	Vector2 mousePos;
	Sprite2D allFather;
	Label mouseBlock;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		allFather = GetChild<Sprite2D>(0);
		mouseBlock = GetChild<Label>(2);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		mousePos = GetViewport().GetMousePosition();
		mousePos -= (GetViewport().GetVisibleRect().Size) / 2;
		int mapSize = Perlin.mapSize;
		// I choose 8 because that's the right number to choose
		// (Since each element of tileMap is 16x16, the 32x32 have 8-pixel overhangs.
		// This means the tile at 0,0 goes from -8 to 24, and -1,0 from -40 to -8)
		mousePos -= allFather.Position;
		mousePos += new Vector2((8 * allFather.Scale.X), (8 * allFather.Scale.Y));
		mousePos /= allFather.Scale;
		mousePos /= 16;
		mousePos += new Vector2(mapSize / 2, mapSize / 2);
		if (mousePos.X < 0 || mousePos.Y < 0 || mousePos.X > mapSize + 1 || mousePos.Y > mapSize + 1) {
			mouseBlock.Text = "";
		} else {
			mouseBlock.Text = (Math.Floor(mousePos.X) + " " + Math.Floor(mousePos.Y));
		}
	}
}
