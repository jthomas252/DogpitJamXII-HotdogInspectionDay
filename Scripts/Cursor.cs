using Godot; 
using System;
using Godot.Collections;
using Array = Godot.Collections.Array;

/**
 * This represents the players hand for grabbing objects
 * TODO: Make this check the intersection better 
 */
public class Cursor : Sprite3D
{
    // Bitmasks for raycasting
    private const uint LAYER_PHYSICAL    = 1;
    private const uint LAYER_INTERACTIVE = 2;
    private const uint LAYER_ENVIRONMENT = 4;
    private const uint LAYER_MOUSE = 8;

    private WorldPoint inspectPoint;
    
    [Export]
    public float cursorDistance = 100f;

    [Export] public Texture handTextureOpen;
    [Export] public Texture handTextureClosed;
    [Export] public Texture handTexturePoint;
    [Export] public Texture handTextureClicked;

    private Camera camera;
    private GrabableObject grabbedObject;
    private Vector3 objectHoldPoint;
    private CursorState cursorState;
    private Array ignoreObjects;

    public override void _Ready()
    {
        // Find relevant world objects
        inspectPoint = GetTree().CurrentScene.GetNode<WorldPoint>("Points/InspectionPoint");
        objectHoldPoint = Vector3.Zero;
        cursorState = CursorState.HandOpen;
    }

    public override void _Process(float delta)
    {
        camera = GetViewport().GetCamera();
        
        // Attempt to match the cursor position in world 
        Vector3 pos = camera.ProjectRayOrigin(GetViewport().GetMousePosition());
        Vector3 normal = camera.ProjectRayNormal(GetViewport().GetMousePosition());
        
        PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;

        if (grabbedObject != null) ignoreObjects = new Array() { grabbedObject };
        
        // Always update cursor position
        Dictionary hand = spaceState.IntersectRay(
            pos, 
            pos + (normal * 1000f), 
            ignoreObjects,
            LAYER_PHYSICAL | LAYER_MOUSE
        );
        if (hand.Count > 0)
        {
            // Translation = (Vector3)hand["position"];
            objectHoldPoint = (Vector3)hand["position"];
        }
        else
        {
            // Translation = pos + (normal * cursorDistance);
            objectHoldPoint = pos + (normal * cursorDistance);
        }
        
        Translation = pos + (normal * cursorDistance);

        // Skip this if already grabbing an object
        if (cursorState != CursorState.HandClosed)
        {

            Dictionary interacts = spaceState.IntersectRay(pos, pos + (normal * 1000f), null, LAYER_INTERACTIVE);
            // Reveal the pointing finger when over something that can be clicked
            if (interacts.Count > 0)
            {
                if (interacts["collider"] is InteractableObject interactiveObject)
                {
                    ChangeCursorState(CursorState.HandPoint);
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
                    }

                    if (interacts["collider"] is GrabableObject grabbableObject)
                    {
                        grabbedObject = grabbableObject;
                        grabbableObject.Grab();
                        ChangeCursorState(CursorState.HandClosed);
                    }
                }
            }
        }
        
        // Holding onto an object
        if (isGrabbing() && Input.IsMouseButtonPressed((int)ButtonList.Right))
        {
            // Drop the grabbed object, release the reference
            grabbedObject.Drop();
            ChangeCursorState(CursorState.HandOpen);
            grabbedObject = null;
        };

        if (Input.IsKeyPressed((int)KeyList.Shift))
        {
            grabbedObject?.UpdateTargetPosition(inspectPoint.GlobalTranslation);
        }
        else
        {
            grabbedObject?.UpdateTargetPosition(objectHoldPoint);
        }
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
        cursorState = state;
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