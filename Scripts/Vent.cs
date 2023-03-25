using Godot;
using System;

public class Vent : Spatial
{
	AnimationPlayer animationPlayer;
	float timeElapsed = 0.0f;
	bool smashed = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("RootNode/AnimationPlayer");
		// unbroken
		// smash will play the break open animation
		// broken
		animationPlayer.Play("unbroken");
	}
	
	public override void _Process(float delta)
	{
		timeElapsed += delta;
		
		if(!smashed && timeElapsed > 2.9f) {
			animationPlayer.Play("smash");
			smashed = true;
		}
	}
}
