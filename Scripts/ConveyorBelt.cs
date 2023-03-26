using Godot;
using System;

public class ConveyorBelt : Spatial
{
	AnimationPlayer animationPlayer;
	
	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("RootNode/AnimationPlayer");

		GetTree().CurrentScene.Connect("LevelStart", this, nameof(StartAnimation));
		GetTree().CurrentScene.Connect("LevelEnd", this, nameof(StopAnimation));
	}

	public void StartAnimation()
	{
		animationPlayer.GetAnimation("moving").Loop = true;
		animationPlayer.Play("moving");		
	}

	public void StopAnimation()
	{
		animationPlayer.Stop();
	}
}
