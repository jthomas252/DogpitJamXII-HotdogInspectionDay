using Godot;
using System;

public class BaseScene : Spatial
{
	public override void _Ready()
	{
		// Start by setting up whatever gameplay variables might be relevant, like player score, day, rat timer, etc. 

		// Set the mouse to be hidden, reconsider enabling when in menus (disable while working on MacOS)
		// Input.MouseMode = Input.MouseModeEnum.Hidden;
	}

	public override void _Process(float delta)
	{
		
	}

	public void Hover()
	{
		
	}

	// Pause the game from progressing while the menu is active
	public void OnMenuButton()
	{
		GetTree().Paused = true; 
		GD.Print("Received an input from the menu button.");
	}
}
