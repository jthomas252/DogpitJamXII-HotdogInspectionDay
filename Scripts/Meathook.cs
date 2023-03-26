using Godot;
using System;

public class Meathook : Spatial
{
	MeshInstance meathook01;
	MeshInstance meathook02;
	MeshInstance meathook03;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		meathook01 = GetNode<MeshInstance>("RootNode/Meathook01");
		meathook02 = GetNode<MeshInstance>("RootNode/Meathook02");
		meathook03 = GetNode<MeshInstance>("RootNode/Meathook03");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{		
		meathook01.GlobalTranslation = new Vector3(meathook01.GlobalTranslation.x + 15.0f * delta, meathook01.GlobalTranslation.y, meathook01.GlobalTranslation.z);

		if(meathook01.GlobalTranslation.x >= 330.0f) {
			meathook01.GlobalTranslation = new Vector3(50.0f, meathook01.GlobalTranslation.y, meathook01.GlobalTranslation.z);
		}
		
		meathook02.GlobalTranslation = new Vector3(meathook02.GlobalTranslation.x + 15.0f * delta, meathook02.GlobalTranslation.y, meathook02.GlobalTranslation.z);

		if(meathook02.GlobalTranslation.x >= 330.0f) {
			meathook02.GlobalTranslation = new Vector3(50.0f, meathook02.GlobalTranslation.y, meathook02.GlobalTranslation.z);
		}
		
		meathook03.GlobalTranslation = new Vector3(meathook03.GlobalTranslation.x + 15.0f * delta, meathook03.GlobalTranslation.y, meathook03.GlobalTranslation.z);

		if(meathook03.GlobalTranslation.x >= 330.0f) {
			meathook03.GlobalTranslation = new Vector3(50.0f, meathook03.GlobalTranslation.y, meathook03.GlobalTranslation.z);
		}
	}
}
