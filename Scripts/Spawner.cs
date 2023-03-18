using Godot;
using System;
using System.Collections.Generic;

public class Spawner : KinematicBody
{
    [Export] 
    public PackedScene hotdog;

    [Export] 
    public Vector3 spawnPoint;
    
    // Re-usable list of hotdogs we have spawned in the scene already
    private List<Spatial> hotdogList;
    
    public override void _Ready()
    {
        hotdogList = new List<Spatial>();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventKey eventKey)
        {
            if (eventKey.Pressed && eventKey.Scancode == (int)KeyList.O)
            {
                TriggerSpawn();
            }
        }
    }

    public void TriggerSpawn()
    {
        Spatial dog = (Spatial)hotdog.Instance();
        hotdogList.Add(dog);
        
        dog.Translation = GlobalTranslation + spawnPoint;
        dog.Rotation = new Vector3(GD.Randf() * 45, GD.Randf() * 45, GD.Randf() * 45);
        
        GetTree().CurrentScene.AddChild(dog);
    }
}
