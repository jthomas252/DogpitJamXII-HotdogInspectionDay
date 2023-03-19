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

    private Spatial inspectPoint;
    
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
    
    public static bool inInspectionMode = false; 

    public override void _Ready()
    {
        // Find relevant world objects
        inspectPoint = GetTree().CurrentScene.GetNode<Spatial>("Points/InspectPoint");
        objectHoldPoint = Vector3.Zero;
        cursorState = CursorState.HandOpen;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouse eventMouse)
        {
            if (eventMouse.IsPressed() && eventMouse.ButtonMask == (int)ButtonList.Left)
            {
                // Attempt to send a one-off to the relevant interact / grab object
                GD.Print("One click");
            }
        }
        
        if (@event is InputEventKey eventKey)
        {
            if (eventKey.IsPressed() && eventKey.Scancode == (int)KeyList.Shift)
            {
                inInspectionMode = !inInspectionMode;
            }
        }        
    }

    public override void _Process(float delta)
    {
        camera = GetViewport().GetCamera();
        
        // Attempt to match the cursor position in world 
        Vector3 pos = camera.ProjectRayOrigin(GetViewport().GetMousePosition());
        Vector3 normal = camera.ProjectRayNormal(GetViewport().GetMousePosition());
        
        PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;

        // TODO: Regenerate this when picking up a new object
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

        if (inInspectionMode)
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