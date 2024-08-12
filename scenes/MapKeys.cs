using Godot;
using System;

public partial class MapKeys : Node2D
{
	HSlider CentralSlider;
	Label CentralValue;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CentralSlider = GetNode<HSlider>("Centralization/cSlider");
		CentralValue = GetNode<Label>("Centralization/cValue");
		CentralSlider.Value = 0.60f;
		CentralValue.Text = "0.60";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		CentralValue.Text = CentralSlider.Value.ToString("0.00");
	}
}
