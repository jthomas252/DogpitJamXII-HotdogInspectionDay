using Godot;
using System;

public class BaseScene : Spatial
{
	[Signal]
	public delegate void OnStateChange();
	
	public enum PlayerState
	{
		Normal,
		Grabbing,
		Inspecting
	}

	private static BaseScene _instance; 
	
	// TODO: Protect this with defined Get/Set methods if needed? 
	private static PlayerState _currentState = PlayerState.Normal;

	public static void ChangePlayerState(PlayerState newState)
	{
		// Emit an event and change the state accordingly 
		_currentState = newState;
	}

	public static PlayerState GetPlayerState()
	{
		return _currentState;
	}
	
	public override void _Ready()
	{
		_instance = this;
		
		// Set the mouse to be hidden, reconsider enabling when in menus (disable while working on MacOS)
		// Input.MouseMode = Input.MouseModeEnum.Hidden;
	}
	
	// Pause the game from progressing while the menu is active
	public void OnMenuButton()
	{
		GetTree().Paused = true; 
		GD.Print("Received an input from the menu button.");
	}
}
