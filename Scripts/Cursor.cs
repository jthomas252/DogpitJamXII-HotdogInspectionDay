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
    // Bitmasks for checks
    private const uint LAYER_PHYSICAL    = 1;
    private const uint LAYER_INTERACTIVE = 2;
    private const uint LAYER_ENVIRONMENT = 4;
    private const uint LAYER_MOUSE = 8;

    private Spatial inspectPoint;
    
    [Export] public float cursorDistance = 100f;
    [Export] public float holdOffset = 5f;
    
    [Export] public Texture handTextureOpen;
    [Export] public Texture handTextureClosed;
    [Export] public Texture handTexturePoint;
    [Export] public Texture handTextureClicked;

    private Camera camera;
    private Node hoverObject;
    private GrabbableObject grabbedObject;
    private Vector3 objectHoldPoint;
    private Array ignoreObjects;
    private PhysicsDirectSpaceState spaceState;
    
    public override void _Ready()
    {
        // Find relevant world objects
        inspectPoint = GetTree().CurrentScene.GetNode<Spatial>("Points/InspectPoint");
        objectHoldPoint = Vector3.Zero;
        spaceState = GetWorld().DirectSpaceState;
    }

    private void AttemptInteraction()
    {
        if (hoverObject != null)
        {
            // Validate the object has an InteractiveObject script 
            if (hoverObject is InteractableObject interactiveObject)
            {
                interactiveObject.OnInteractedWith();
                ChangeCursorState(CursorState.HandClicked);
            }

            if (hoverObject is GrabbableObject grabbableObject)
            {
                grabbedObject = grabbableObject;
                ignoreObjects = new Array() { grabbedObject };
                grabbableObject.Grab();
                ChangeCursorState(CursorState.HandClosed);
            }
        }
    }

    private void DropObject()
    {
        // Holding onto an object
        if (isGrabbing() && Input.IsMouseButtonPressed((int)ButtonList.Right))
        {
            // Drop the grabbed object, release the reference
            grabbedObject.Drop();
            ChangeCursorState(CursorState.HandOpen);
            grabbedObject = null;
        }    
    }
    
    // TODO: Move this somewhere more appropriate and set up signals where relevant
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouse eventMouse)
        {
            if (eventMouse.IsPressed())
            {
                switch (eventMouse.ButtonMask)
                {
                    case (int)ButtonList.Left:
                        AttemptInteraction();
                        break;
                    
                    case (int)ButtonList.Right:
                        DropObject();
                        break;
                }
            }
        }
        
        if (@event is InputEventKey eventKey)
        {
            if (eventKey.IsPressed())
            {
                switch (eventKey.Scancode)
                {
                    case (int)KeyList.Shift:
                        BaseScene.ChangePlayerState(
                            BaseScene.GetPlayerState() == BaseScene.PlayerState.Inspecting ? 
                            (isGrabbing() ? BaseScene.PlayerState.Grabbing : BaseScene.PlayerState.Normal) : 
                            BaseScene.PlayerState.Inspecting
                        );
                        break;
                }
            }
        }        
    }

    public override void _Process(float delta)
    {
        camera = GetViewport().GetCamera();
        
        // Attempt to match the cursor position in world 
        Vector3 pos = camera.ProjectRayOrigin(GetViewport().GetMousePosition());
        Vector3 normal = camera.ProjectRayNormal(GetViewport().GetMousePosition());

        // Always update cursor position
        Dictionary hand = spaceState.IntersectRay(
            pos, 
            pos + (normal * 1000f), 
            ignoreObjects,
            LAYER_PHYSICAL | LAYER_MOUSE
        );
        
        if (hand.Count > 0)
        {
            Vector3 hitPoint = (Vector3)hand["position"];
            float distance = pos.DistanceTo(hitPoint);
            objectHoldPoint = pos + (normal * (distance - holdOffset));
        }
        else
        {
            objectHoldPoint = pos + (normal * cursorDistance);
        }
        
        Translation = pos + (normal * cursorDistance);

        // Check for object interactions
        if (!isGrabbing())
        {
            Dictionary interacts = spaceState.IntersectRay(pos, pos + (normal * 1000f), null, LAYER_INTERACTIVE);
            
            // Reveal the pointing finger when over something that can be clicked
            if (interacts.Count > 0)
            {
                hoverObject = (Node)interacts["collider"];
                
                if (interacts["collider"] is InteractableObject interactiveObject)
                {
                    ChangeCursorState(CursorState.HandPoint);
                }
            }
            else
            {
                hoverObject = null;
                ChangeCursorState(CursorState.HandOpen);
            }
        }
        // Update the position of the grabbed object
        else
        {
            if (BaseScene.GetPlayerState() == BaseScene.PlayerState.Inspecting)
            {
                grabbedObject.UpdateTargetPosition(inspectPoint.GlobalTranslation);
            }
            else
            {
                grabbedObject.UpdateTargetPosition(objectHoldPoint);
            }
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