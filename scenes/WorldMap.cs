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
		Vector2 position = allFather.Position;
		Vector2 scale = allFather.Scale;
		mousePos /= scale;
		mousePos -= position;
		mousePos /= 32;
		mouseBlock.Text = (Math.Floor(mousePos.X) + " " + Math.Floor(mousePos.Y));
	}
}
