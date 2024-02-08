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
		if (Input.IsActionJustPressed("phZoom1")) {
			Vector2 zoom = Zoom;
			if (zoom.X > .00) {
				zoom.X *= .7f;
				zoom.Y *= .7f;
				Zoom = zoom;
			}
		}
		if (Input.IsActionJustPressed("phZoom2")) {
			Vector2 zoom = Zoom;
			if (zoom.X < 3) {
				zoom.X *= (1f / .7f);
				zoom.Y *= (1f / .7f);
				Zoom = zoom;
			}
		}
		if (RMBDown) {
			Vector2 newMousePos = GetViewport().GetMousePosition();
			Vector2 newClicker = new Vector2(newMousePos.X - mousePos.X, newMousePos.Y - mousePos.Y);
			// now we multiply by zoom to get a more reponsive feeling zoom.
			newClicker.X = newClicker.X * (1 / Zoom.X);
			newClicker.Y = newClicker.Y * (1 / Zoom.X);
			// this should now represent the change in x and y.
			Position = new Vector2(Position.X - newClicker.X, Position.Y - newClicker.Y);
			mousePos = newMousePos;
		}
	}
}
