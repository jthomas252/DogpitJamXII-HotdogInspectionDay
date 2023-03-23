using Godot;
using System;

public class ConveyorBelt : Spatial
{
	AnimationPlayer animationPlayer;
	
	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("RootNode/AnimationPlayer");
		animationPlayer.GetAnimation("moving").Loop = true;
		animationPlayer.Play("moving");
	}
}
