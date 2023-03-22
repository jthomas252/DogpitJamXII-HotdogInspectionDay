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
    private const uint LAYER_PHYSICAL = 1;
    private const uint LAYER_INTERACTIVE = 2;
    private const uint LAYER_ENVIRONMENT = 4;
    private const uint LAYER_MOUSE = 8;

    private Spatial inspectPoint;

    [Export] public float cursorDistance = 100f;
    [Export] public float holdOffset = 5f;

    [Export] public Texture handTextureOpen;
    [Export] public Texture handTextureClosed;
    [Export] public Texture handTexturePoint;
    [Export] public Texture handTextureMagnify;
    [Export] public Texture handTextureClicked;

    private Camera camera;
    private Node hoverObject;
    private Spatial grabbedObject;
    private Vector3 objectHoldPoint;
    private Array ignoreObjects;
    private PhysicsDirectSpaceState spaceState;
    private MeshInstance debugCursor;

    public enum CursorState
    {
        HandOpen,
        HandClosed,
        HandPoint,
        HandMagnify,
        HandClicked,
    }

    public override void _Ready()
    {
        // Find relevant world objects
        inspectPoint = GetTree().CurrentScene.GetNode<Spatial>("Points/InspectPoint");
        objectHoldPoint = Vector3.Zero;
        spaceState = GetWorld().DirectSpaceState;
        camera = GetViewport().GetCamera();
        debugCursor = GetNode<MeshInstance>("Debug");
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

            if (hoverObject is ViewableObject viewableObject)
            {
                viewableObject.Inspect();
                grabbedObject = viewableObject;
                ChangeCursorState(CursorState.HandOpen);
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
        if (IsGrabbing() && Input.IsMouseButtonPressed((int)ButtonList.Right))
        {
            if (grabbedObject is GrabbableObject grabbableObject) grabbableObject.Drop();
            if (grabbedObject is ViewableObject viewableObject) viewableObject.Drop();

            // Drop the grabbed object, release the reference
            ChangeCursorState(CursorState.HandOpen);
            grabbedObject = null;
            ignoreObjects = null;
        }
    }

    // TODO: Rig this to a input signal dispatcher
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
                        // Don't allow dropping the hotdog if it's currently in inspection mode, but cancel out instead
                        if (BaseScene.GetPlayerState() != BaseScene.PlayerState.Inspecting)
                        {
                            DropObject();
                        }
                        else
                        {
                            BaseScene.ChangePlayerState(BaseScene.PlayerState.Grabbing);
                        }

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
                    case (int)KeyList.Q:
                        BaseScene.ChangePlayerState(
                            BaseScene.GetPlayerState() == BaseScene.PlayerState.Inspecting
                                ? (IsGrabbing() ? BaseScene.PlayerState.Grabbing : BaseScene.PlayerState.Normal)
                                : BaseScene.PlayerState.Inspecting
                        );
                        break;
                }
            }
        }
    }

    private void UpdateWorldPosition(Vector3 pos, Vector3 normal)
    {
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
            objectHoldPoint = pos + (normal * (distance)) + (Vector3.Up * 10f);
        }
        else
        {
            objectHoldPoint = pos + (normal * cursorDistance);
        }

        debugCursor.GlobalTranslation = objectHoldPoint;
        Translation = pos + (normal * cursorDistance);
    }

    private void CheckForInteractiveObjects(Vector3 pos, Vector3 normal)
    {
        Dictionary interacts = spaceState.IntersectRay(
            pos, 
            pos + (normal * 1000f), 
            ignoreObjects, 
            LAYER_INTERACTIVE,
            true, true
        );

        // Reveal the pointing finger when over something that can be clicked
        if (interacts.Count > 0)
        {
            hoverObject = (Node)interacts["collider"];
            ComputerScreen.UpdateBodyBottomText($"{hoverObject.Name}");
            
            if (hoverObject is InteractableObject && !Input.IsMouseButtonPressed((int)ButtonList.Left) && !IsGrabbing())
            {
                ChangeCursorState(CursorState.HandPoint);
            }
            else if (hoverObject is ViewableObject && !IsGrabbing())
            {
                ChangeCursorState(CursorState.HandMagnify);
            }
            else if (hoverObject is SnapTrigger snapTrigger)
            {
                objectHoldPoint = snapTrigger.GetSnapPoint();
            }
        }
        else
        {
            ComputerScreen.UpdateBodyBottomText("NO INTERACT");
            ChangeCursorState(CursorState.HandOpen);
        }
    }

    private void UpdateGrabbedObjectPosition()
    {
        if (grabbedObject is GrabbableObject grabbableObject)
        {
            if (BaseScene.Inspecting())
            {
                grabbableObject.UpdateTargetPosition(inspectPoint.GlobalTranslation);
            }
            else
            {
                grabbableObject.UpdateTargetPosition(objectHoldPoint);
            }
        }
    }

    public override void _Process(float delta)
    {
        hoverObject = null;

        Vector3 pos = camera.ProjectRayOrigin(GetViewport().GetMousePosition());
        Vector3 normal = camera.ProjectRayNormal(GetViewport().GetMousePosition());

        UpdateWorldPosition(pos, normal);

        // Check for object interactions
        CheckForInteractiveObjects(pos, normal);
        
        // Update the position of the grabbed object
        if (IsGrabbing()) UpdateGrabbedObjectPosition();
    }

    public bool IsGrabbing()
    {
        return (grabbedObject != null);
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

            case CursorState.HandMagnify:
                Texture = handTextureMagnify;
                break;

            default:
            case CursorState.HandOpen:
                Texture = handTextureOpen;
                break;
        }
    }

    public void ChangeGrabbedObject(Spatial s)
    {
        grabbedObject = s;
    }
}