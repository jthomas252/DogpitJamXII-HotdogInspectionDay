using Godot;
using System;

public class BaseScene : Spatial
{
	[Signal]
	public delegate void Inspection();

	[Signal]
	public delegate void Grabbed();
	
	[Signal]
	public delegate void Normal();
	
	public enum PlayerState
	{
		Normal,
		Grabbing,
		Inspecting
	}

	private static BaseScene _instance; 
	
	// TODO: Protect this with defined Get/Set methods if needed? 
	private static PlayerState _currentState = PlayerState.Normal;

	public static bool Inspecting()
	{
		return _currentState == PlayerState.Inspecting;
	}
	
	public static void ChangePlayerState(PlayerState newState)
	{
		GD.Print($"SWITCHED TO STATE {_currentState.ToString()}");
		
		_currentState = newState;
		ComputerScreen.UpdateBodyBottomText(_currentState.ToString());
		
		// Emit an event and change the state accordingly 
		switch (_currentState)
		{
			case PlayerState.Inspecting:
				_instance.EmitEvent("Inspection");
				break; 
			
			case PlayerState.Grabbing:
				_instance.EmitEvent("Grabbed");
				break;
			
			default:
			case PlayerState.Normal:
				_instance.EmitEvent("Normal");
				break;
		}
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

	/**
	 * So we can call Emit from a static function
	 */
	private void EmitEvent(string ev)
	{
		EmitSignal(ev);
	}
}
