using Godot;
using System;

public partial class MainCam : Camera2D
{
	bool RMBDown;
	Vector2 mousePos;
	Sprite2D allFather;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		RMBDown = false;
		allFather = GetNode<Sprite2D>("../MapGrid");
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
			Vector2 zoom = allFather.Scale;
			if (zoom.X > .06) {
				zoom.X *= .7f;
				zoom.Y *= .7f;
				allFather.Scale = zoom;
				GD.Print(allFather.Scale);
			}
		}
		if (Input.IsActionJustPressed("phZoom2")) {
			Vector2 zoom = allFather.Scale;
			if (zoom.X < 2) {
				zoom.X *= (1 / .7f);
				zoom.Y *= (1 / .7f);
				allFather.Scale = zoom;
				GD.Print(allFather.Scale);
			}
		}
		if (RMBDown) {
			Vector2 newMousePos = GetViewport().GetMousePosition();
			Vector2 newClicker = new Vector2(newMousePos.X - mousePos.X, newMousePos.Y - mousePos.Y);
			// now we multiply by zoom to get a more reponsive feeling zoom.
			newClicker.X = newClicker.X;
			newClicker.Y = newClicker.Y;
			// this should now represent the change in x and y.
			allFather.Position = new Vector2(allFather.Position.X + newClicker.X, allFather.Position.Y + newClicker.Y);
			mousePos = newMousePos;
		}
	}
}
