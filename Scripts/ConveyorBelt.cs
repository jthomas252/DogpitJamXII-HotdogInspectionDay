using Godot;
using System;

public class ConveyorBelt : Spatial
{
	AnimationPlayer animationPlayer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("RootNode/AnimationPlayer");
		animationPlayer.GetAnimation("moving").Loop = true;
		animationPlayer.Play("moving");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
	}
}
