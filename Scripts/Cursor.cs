using Godot; 
using System;
using Godot.Collections;

/**
 * This represents the players hand for grabbing objects
 * TODO: Make this check the intersection better 
 */
public class Cursor : Sprite3D
{
    public override void _Process(float delta)
    {
        base._Process(delta);
        
        // Attempt to match the cursor position in world 
        Camera cam = GetViewport().GetCamera();
        Vector3 pos = cam.ProjectRayOrigin(GetViewport().GetMousePosition());
        Vector3 normal = cam.ProjectRayNormal(GetViewport().GetMousePosition());
        
        PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
        Dictionary dict = spaceState.IntersectRay(pos, pos + (normal * 1000f));
        
        if (Input.IsKeyPressed((int)KeyList.Q))
        {
            GD.Print(dict);
        }

        float zDist = 10f; // base distance the cursor should always appear away
        float zOffset = 2.5f; // offset distance from a collision 
        
        if (dict.Count > 0)
        {
            Vector3 hit = (Vector3)dict["position"];
            zDist = pos.DistanceTo(hit);
        }
        
        Translation = pos + (normal * (zDist - zOffset));
        
    }
    
}