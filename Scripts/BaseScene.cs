using Godot;
using System;
using System.Collections.Generic;

public class BaseScene : Spatial
{
	private readonly string[] inspectorRanks = new string[]
	{
		"Hotdog Inspection Intern",
		"Junior Hotdog Inspector",
		"Associate Hotdog Inspector",
		"Expert Hotdog Inspector",
		"Master Hotdog Inspector",
		"Grandmaster Hotdog Inspector",
		"Hotdog King",
		"Hotdog God",
		"Chosen One",
		"Impossible",
	};
	
	[Export] public PackedScene citationObject; 
	
	// Music
	[Export] public AudioStream titleTheme;
	[Export] public AudioStream gameTheme; 
	
	// Audio library
	[Export] public AudioStream hotdogNoise;
	[Export] public AudioStream[] documentNoises;
	
	private const int PLAYER_QUOTA = 0;
	private const int PLAYER_QUOTA_PER_LEVEL = 0;
	private const int PLAYER_QUOTA_EXCEED = 3; 
	private const int PLAYER_CITATION_THRESHOLD = 3;
	private const float PLAYER_LEVEL_LENGTH = 210f;
	private const int ALLOWED_MISTAKES_FOR_DEMOTION = 3; 

	private int _playerRank; // Inspector rank 
	private int _playerMistake;
	private int _playerCitations; 
	private int _playerScore;
	private int _ratLoss; 
	private int _playerLevel; 
	private int _playerQuota;
	private float _playerTimer;

	private AudioStreamPlayer _soundPlayer;
	private AudioStreamPlayer _musicPlayer; 
	
	private Label3D _timer;
	private Label _controlText;
	
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

	[Signal]
	public delegate void LevelReset();

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
				_instance._controlText.Text = "[Right Click] Dismiss\n[WASD] Rotate";				
				break; 
			
			case PlayerState.Grabbing:
				_instance.EmitEvent("Grabbed");
				_instance._controlText.Text = "[Right Click] Drop\n[Q] Inspect";
				break;
			
			default:
			case PlayerState.Normal:
				_instance.EmitEvent("Normal");
				_instance._controlText.Text = "";
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
			_instance._playerCitations++;
			if (_instance._playerCitations > 3) GameOver("Too many citations!");
		}
		UpdateScoreDisplay();
	}

	public static void IterateRatLoss()
	{
		_instance._ratLoss++;
		DecrementScore();
	}

	public static void UpdateScoreDisplay()
	{
		ComputerScreen.UpdateBodyBottomText($"QUOTA {_instance._playerScore} OF {_instance._playerQuota}");
	}
	
	public static void StartNextLevel()
	{
		_instance.EmitEvent("LevelStart");
		_instance._playerLevel++;

		_instance._playerCitations = 0;
		_instance._playerMistake = 0;
		_instance._playerScore = 0;
		_instance._ratLoss = 0;
		
		_instance._playerQuota = PLAYER_QUOTA + _instance._playerLevel * PLAYER_QUOTA_PER_LEVEL;
		_instance._playerTimer = PLAYER_LEVEL_LENGTH; 
		
		// Activate relevant objects
		_instance._timer.Visible = true; 
		
		// Start the score display 
		UpdateScoreDisplay();
		Computer.ActivateScreen();
	}

	public static void OnLevelEnd()
	{
		// Past level 5 just trigger the ending.
		if (_instance._playerLevel >= 5)
		{
			Fader.FadeOut("show_end");
			return; 
		}
		
		// Show the start screen, pass the relevant data.
		_instance.EmitEvent("LevelEnd");

		string dayText = $"DAY {_instance._playerLevel} SURVIVED";
		string statText = "HERE'S HOW YOU DID\n";
		string buttonText = "Proceed";

		if (_instance._playerScore > (_instance._playerQuota + PLAYER_QUOTA_EXCEED))
		{
			statText += "EXCEEDED YOUR QUOTA!\n\n";
			_instance._playerRank++; 
		} else if (_instance._playerScore >= _instance._playerQuota)
		{
			statText += "MET YOUR QUOTA\n\n";
		}
		else
		{
			// Don't let the player proceed without at least meeting quota.
			statText += "FAILED TO MEET YOUR QUOTA\n\n";
			dayText = "YOU GOT FIRED";
			buttonText = "Retry";
			_instance._playerRank--;
			_instance._playerLevel--;
		}
		
		statText += _instance._ratLoss > 0 ? $"{_instance._ratLoss} HOTDOGS LOST TO RATS\n" : "NO HOTDOGS LOST TO RATS\n";
		statText += _instance._playerCitations > 0 ? $"{_instance._playerCitations} CITATIONS ISSUED\n" : "NO CITATIONS ISSUED\n";		

		// Promote the player if they've earned it
		_instance._playerRank += _instance._playerMistake < ALLOWED_MISTAKES_FOR_DEMOTION ? 1 : -1;
		// Ensure we don't go out of bounds with inspector ranks
		_instance._playerRank = _instance._playerRank < 0 ? 0 : _instance._playerRank;

		if (_instance._playerScore >= _instance._playerQuota)
		{
			statText += $"\nYOUR NEW POSITION:";
			BetweenLevelScreen.SetText(dayText, statText, _instance.inspectorRanks[_instance._playerRank], buttonText);
		}
		else
		{
			BetweenLevelScreen.SetText(dayText, statText, "TRY AGAIN?", buttonText);			
		}

		// Show the stat screen and set the text
		Fader.FadeOut("show_stat_menu");
		
		// Deactivate relevant objects
		_instance._timer.Visible = false; 
		Computer.DeactiveScreen();
		
		// Force the cursor to clean up any objects that will be erased.
		Cursor.ForceReleaseObject(null, true);
	}

	// Play a generic sound at the world position
	public static void PlaySound(AudioStream stream, float volume = -10f)
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

	/**
	 * Trigger a game over 
	 */
	public static void GameOver(string reason, bool useFade = true)
	{
		// Make the player retry the current level
		_instance._playerLevel--;
		
		if (useFade) Fader.FadeOut("show_stat_menu");
		
		BetweenLevelScreen.SetText("YOU FAILED", reason, "", "Try Again");
		
		// Force the cursor to clean up any object references. 
		Cursor.ForceReleaseObject(null, true);
	}

	public override void _Ready()
	{
		// Set initial seed 
		GD.Randomize();
		_instance = this;
		
		// Set the mouse to be hidden, reconsider enabling when in menus (disable while working on MacOS)
		// Input.MouseMode = Input.MouseModeEnum.Hidden;

		_timer = GetNode<Label3D>("Environment/Timer/Main");
		_controlText = GetNode<Label>("Interface/ControlText");
		
		_soundPlayer = GetNode<AudioStreamPlayer>("Sound");
		_musicPlayer = GetNode<AudioStreamPlayer>("Music");

		_musicPlayer.Stream = titleTheme;
		_musicPlayer.Play(); 
	}

	public void OnFadeApex(string callback)
	{
		switch (callback)
		{
			case "hide_stat_menu":
			case "hide_start_menu":
				Input.MouseMode = Input.MouseModeEnum.Hidden;
				_musicPlayer.Stream = gameTheme;
				_musicPlayer.Play();
				break; 
			
			case "show_end":
			case "show_stat_menu":
				Input.MouseMode = Input.MouseModeEnum.Visible; 
				_musicPlayer.Stream = titleTheme;
				_musicPlayer.Play(); 
				EmitSignal(nameof(LevelReset));
				break;
		}
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

	/**
	 * So we can call Emit from a static function
	 */
	private void EmitEvent(string ev)
	{
		EmitSignal(ev);
	}
}
