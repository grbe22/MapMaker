using Godot;
using System;

public partial class MapKeys : Node2D
{
	HSlider CentralSlider;
	Label CentralValue;
	HSlider SizeSlider;
	Label SizeValue;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CentralSlider = GetNode<HSlider>("Centralization/cSlider");
		CentralValue = GetNode<Label>("Centralization/cValue");
		CentralSlider.Value = 0.60f;
		CentralValue.Text = "0.60";
		
		SizeSlider = GetNode<HSlider>("MapSize/mSlider");
		SizeValue = GetNode<Label>("MapSize/mValue");
		SizeSlider.Value = 32;
		SizeValue.Text = "32";
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		CentralValue.Text = CentralSlider.Value.ToString("0.00");
		SizeValue.Text = SizeSlider.Value.ToString();
	}
}
