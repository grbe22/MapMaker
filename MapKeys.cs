using Godot;
using System;

public partial class MapKeys : Node2D
{
	HSlider CentralSlider;
	Label CentralValue;
	HSlider SizeSlider;
	Label SizeValue;
	HSlider ClimateSlider;
	Label ClimateValue;
	HSlider PerlinSlider;
	Label PerlinValue;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CentralSlider = GetNode<HSlider>("Centralization/Slider");
		CentralValue = GetNode<Label>("Centralization/Value");
		
		SizeSlider = GetNode<HSlider>("MapSize/Slider");
		SizeValue = GetNode<Label>("MapSize/Value");
		
		ClimateSlider = GetNode<HSlider>("Climate/Slider");
		ClimateValue = GetNode<Label>("Climate/Value");
		
		PerlinSlider = GetNode<HSlider>("Perlin/Slider");
		PerlinValue = GetNode<Label>("Perlin/Value");
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		CentralValue.Text = CentralSlider.Value.ToString("0.00");
		SizeValue.Text = SizeSlider.Value.ToString();
		ClimateValue.Text = ClimateSlider.Value.ToString();
		PerlinValue.Text = PerlinSlider.Value.ToString();
	}

	private void _on_line_edit_text_changed(string new_text)
	{
		if (!int.TryParse(new_text, out _)) {
			LineEdit thisIs = GetNode<LineEdit>("Seed/LineEdit");
			thisIs.Clear();
		} else {
			LineEdit thisIs = GetNode<LineEdit>("Seed/LineEdit");
			thisIs.Clear();
		}
	}
}
