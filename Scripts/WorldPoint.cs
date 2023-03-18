using Godot;
using System;
using System.Collections.Generic;

public class WorldPoint : Spatial
{
    // private static Dictionary<string,List<WorldPoint>> worldPointList;
    // public static WorldPoint getWorldPoint (string pointName, int index = 0)
    // {
    //     if (worldPointList.ContainsKey(pointName) == false || worldPointList.Count > index)
    //     {
    //         GD.PushError($"Missing point name: ${pointName} i: ${index}");
    //     }
    //     return worldPointList[pointName][index];
    // }
    
    // [Export]
    // public string labelText
    // {
    //     get => worldLabel.Text;
    //     set => worldLabel.Text = value;
    // }
    //
    // [Export]
    // public Texture background
    // {
    //     get => worldIcon.Texture;
    //     set => worldIcon.Texture = value;
    // }

    // [Export] public string pointName = "make_unique";

    // private Label3D worldLabel;
    // private Sprite3D worldIcon;

    public override void _Ready()
    {
        base._Ready();
        
        // Find and set child objects
        // worldLabel = GetChild<Label3D>(0);
        // worldIcon = GetChild<Sprite3D>(1);
        
        // Register self with global Dictionary, and/or create it
        // if (worldPointList != null) worldPointList = new Dictionary<string, List<WorldPoint>>();
        //
        // if (!worldPointList.ContainsKey(pointName))
        // {
        //     worldPointList.Add(pointName, new List<WorldPoint>());
        // }
        // else
        // {
        //     worldPointList[pointName].Add(this);
        // }
    }
}
