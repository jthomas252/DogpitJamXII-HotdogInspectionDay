using Godot; 
using System;
using Godot.Collections;

/**
 * This represents the players hand for grabbing objects
 * TODO: Make this check the intersection better 
 */
public class Cursor : Sprite3D
{
    // Bitmasks for raycasting
    private const int LAYER_PHYSICAL    = 1;
    private const int LAYER_INTERACTIVE = 2;
    private const int LAYER_ENVIRONMENT = 4;

    [Export]
    public WorldPoint inspectionPoint;
    
    [Export]
    public float cursorDistance = 10f;

    [Export] public Texture handTextureOpen;
    [Export] public Texture handTextureClosed;
    [Export] public Texture handTexturePoint;
    [Export] public Texture handTextureClicked;

    private Camera camera;
    private GrabableObject grabbedObject;
    private Vector3 objectHoldPoint;

    public override void _Ready()
    {
        // Find relevant world objects
        objectHoldPoint = GetTree().CurrentScene.GetNode<WorldPoint>("Points/InspectionPoint").GlobalTranslation;
    }

    public override void _Process(float delta)
    {
        camera = GetViewport().GetCamera();
        
        // Attempt to match the cursor position in world 
        Vector3 pos = camera.ProjectRayOrigin(GetViewport().GetMousePosition());
        Vector3 normal = camera.ProjectRayNormal(GetViewport().GetMousePosition());
        
        PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;

        Translation = pos + (normal * cursorDistance);

        Dictionary hand = spaceState.IntersectRay(
            pos, 
            pos + (normal * 1000f), 
            new Godot.Collections.Array{grabbedObject}, 
            LAYER_PHYSICAL
        );
        
        if (hand.Count > 0)
        {
            objectHoldPoint = (Vector3)hand["position"];
        }
        else
        {
            objectHoldPoint = pos + (normal * cursorDistance);
        }

        if (Input.IsKeyPressed((int)KeyList.Key8))
        {
            GD.Print(hand);
        }

        Dictionary interacts = spaceState.IntersectRay(pos, pos + (normal * 1000f), null, LAYER_INTERACTIVE);

        // Reveal the pointing finger when over something that can be clicked
        if (interacts.Count > 0)
        {
            if (interacts["collider"] is InteractableObject interactiveObject && !isGrabbing())
            {
                ChangeCursorState(CursorState.HandPoint);
                GD.Print("Point");
            }
        }
        else
        {
            ChangeCursorState(CursorState.HandOpen);
        }

        // Attempt to send a raycast only for interactable objects
        if (Input.IsMouseButtonPressed((int)ButtonList.Left))
        {
            if (interacts.Count > 0)
            {
                // Validate the object has an InteractiveObject script 
                if (interacts["collider"] is InteractableObject interactiveObject)
                {
                    interactiveObject.OnInteractedWith();
                    ChangeCursorState(CursorState.HandClicked);
                    GD.Print("Clicked");
                }
                
                if (interacts["collider"] is GrabableObject grabbableObject)
                {
                    grabbedObject = grabbableObject;
                    grabbableObject.Grab();
                    ChangeCursorState(CursorState.HandClosed);
                    GD.Print("Grabbed");
                }
            } 
        }
        
        if (Input.IsMouseButtonPressed((int)ButtonList.Right))
        {
            // Drop the grabbed object, release the reference
            grabbedObject = null; 
            ChangeCursorState(CursorState.HandOpen);
            GD.Print("Dropped");
        }

        grabbedObject?.UpdateTargetPosition(objectHoldPoint);
    }

    public bool isGrabbing()
    {
        return (grabbedObject != null);
    }

    private enum CursorState
    {
        HandOpen,
        HandClosed,
        HandPoint,
        HandClicked,
    }

    private void ChangeCursorState(CursorState state)
    {
        switch (state)
        {
            case CursorState.HandClicked:
                Texture = handTextureClicked;
                break;
            
            case CursorState.HandPoint:
                Texture = handTexturePoint;
                break;
                
            case CursorState.HandClosed:
                Texture = handTextureClosed;
                break; 
            
            default:
            case CursorState.HandOpen:
                Texture = handTextureOpen;
                break;
        }
    }
    
}