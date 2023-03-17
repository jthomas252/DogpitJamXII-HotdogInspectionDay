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
    private List<Hotdog> hotdogList;
    
    public override void _Ready()
    {
        hotdogList = new List<Hotdog>();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventKey eventKey)
        {
            if (eventKey.Pressed && eventKey.Scancode == (int)KeyList.O)
            {
                this.TriggerSpawn();
            }
        }
    }

    public void TriggerSpawn()
    {
        Hotdog dog = (Hotdog)hotdog.Instance();
        hotdogList.Add(dog);
        GetTree().CurrentScene.AddChild(dog);
        
        dog.Translation = Translation + spawnPoint;
    }
}
