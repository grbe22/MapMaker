using Godot;
using System;

public partial class MapHandler : Node
{
	TileArray tileyBoy;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tileyBoy = new TileArray(20, 20);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
