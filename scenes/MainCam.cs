using Godot;
using System;

public partial class MainCam : Camera2D
{
	bool RMBDown;
	Vector2 mousePos;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		RMBDown = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (Input.IsActionJustPressed("RMB")) {
			mousePos = GetViewport().GetMousePosition();
			RMBDown = true;
		}
		if (Input.IsActionJustReleased("RMB")) {
			mousePos = GetViewport().GetMousePosition();
			RMBDown = false;
		}
		if (RMBDown) {
			Vector2 newMousePos = GetViewport().GetMousePosition();
			GD.Print(newMousePos, " ", mousePos);
			Vector2 newClicker = new Vector2(newMousePos.X - mousePos.X, newMousePos.Y - mousePos.Y);
			// this should now represent the change in x and y.
			Position = new Vector2(Position.X - newClicker.X, Position.Y - newClicker.Y);
			mousePos = newMousePos;
		}
	}
}
