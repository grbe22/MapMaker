using Godot;
using System;

public partial class MapHandler : Node
{
	Perlin tileyBoy;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tileyBoy = new Perlin(20, 4);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
