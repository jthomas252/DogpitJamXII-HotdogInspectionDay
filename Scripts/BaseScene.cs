using Godot;
using System;
using System.Collections.Generic;

public class BaseScene : Spatial
{
	// Music
	[Export] public AudioStream titleTheme;
	[Export] public AudioStream gameTheme; 
	
	// Audio library
	[Export] public AudioStream hotdogNoise;
	[Export] public AudioStream[] documentNoises;
	
	private const int PLAYER_QUOTA = 8;
	private const int PLAYER_QUOTA_PER_LEVEL = 2;
	private const int PLAYER_CITATION_THRESHOLD = 3;
	private const float PLAYER_LEVEL_LENGTH = 180f; 
	
	private int _playerMistake;
	private int _playerScore;
	private int _playerLevel; 
	private int _playerQuota;
	private float _playerTimer;

	private AudioStreamPlayer _soundPlayer;
	private AudioStreamPlayer _musicPlayer; 
	
	private Label3D _timer; 
	
	[Signal]
	public delegate void Inspection();

	[Signal]
	public delegate void Grabbed();
	
	[Signal]
	public delegate void Normal();

	[Signal]
	public delegate void LevelEnd();

	[Signal]
	public delegate void LevelStart();

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

	/**
	 * Run when a valid Hotdog passes through the process trigger
	 */
	public static void IterateScore()
	{
		_instance._playerScore++;
		UpdateScoreDisplay();
	}

	public static int GetLevel()
	{
		return _instance._playerLevel; 
	}

	/**
	 * Only issue citations right now
	 */
	public static void DecrementScore()
	{
		_instance._playerMistake++;
		if (_instance._playerMistake > 0 && _instance._playerMistake % PLAYER_CITATION_THRESHOLD == 0)
		{
			// Replace this with a citation object
			_instance.GetNode<Spawner>("Spawner").SpawnGenericObject(new PackedScene());
		}
		UpdateScoreDisplay();
	}

	public static void UpdateScoreDisplay()
	{
		ComputerScreen.UpdateBodyBottomText($"QUOTA {_instance._playerScore} OF {_instance._playerQuota}");
	}
	
	public static void StartNextLevel()
	{
		// Clear any hotdogs still in the scene
		// Call the spawner and reset the spawn count / timers
		// Spawn any unique packed scenes 
		// Set the rat spawn timer to function 
		_instance._playerLevel++; 
		_instance._playerQuota = PLAYER_QUOTA + _instance._playerLevel * PLAYER_QUOTA_PER_LEVEL;
		_instance._playerTimer = PLAYER_LEVEL_LENGTH; 
	}

	public static void OnLevelEnd()
	{
		// Show the between levels score display
		// Tell the player how they did
		// Start the next level 
	}

	// Play a generic sound at the world position
	public static void PlaySound(AudioStream stream, float volume = 10f)
	{
		_instance._soundPlayer.VolumeDb = volume; 
		_instance._soundPlayer.Stream = stream; 
		_instance._soundPlayer.Play();
	}

	public static AudioStream GetHotdogNoise()
	{
		return _instance.hotdogNoise; 
	}
	
	public static AudioStream GetDocumentNoise()
	{
		return _instance.documentNoises[GD.Randi() % _instance.documentNoises.Length];
	}

	public static bool IsGameActive()
	{
		return _instance._playerTimer > 0f; 
	}

	public override void _Ready()
	{
		// Set initial seed 
		GD.Randomize();
		_instance = this;
		
		// Set the mouse to be hidden, reconsider enabling when in menus (disable while working on MacOS)
		// Input.MouseMode = Input.MouseModeEnum.Hidden;

		_timer = GetNode<Label3D>("Environment/Timer/Main");
		
		_soundPlayer = GetNode<AudioStreamPlayer>("Sound");
		_musicPlayer = GetNode<AudioStreamPlayer>("Music");

		_musicPlayer.Stream = titleTheme;
		_musicPlayer.Play(); 
		
		StartNextLevel();
	}

	public override void _Process(float delta)
	{
		if (_playerTimer > 0)
		{
			_playerTimer -= delta;
			_timer.Text = GetTimerText();
			if (_playerTimer < 0)
			{
				OnLevelEnd();
			}
		}
	}

	private string GetTimerText()
	{
		if (_playerTimer > 60f)
		{
			int minutes = Mathf.FloorToInt(_playerTimer / 60);
			int seconds = Mathf.FloorToInt(_playerTimer % 60);
			return minutes + ":" + (seconds > 9 ? seconds.ToString() : "0" + seconds.ToString());
		}

		return _playerTimer.ToString("0");
	}

	// Pause the game from progressing while the menu is active
	public void OnBeginButton()
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
