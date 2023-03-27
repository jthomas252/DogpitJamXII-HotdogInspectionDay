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
    [Export] public Texture handTextureGrab; 

    private static Cursor _instance;
    private static Label3D _tooltip; 
    private Camera camera;
    private Node hoverObject;
    private Spatial grabbedObject;
    private Vector3 objectHoldPoint;
    private Vector3 objectHoldRotation; 
    private Array ignoreObjects;
    private PhysicsDirectSpaceState spaceState;

    private bool _revertToNormalOnDrop = false; 

    public enum CursorState
    {
        HandOpen,
        HandClosed,
        HandPoint,
        HandGrab,
        HandMagnify,
        HandClicked,
    }

    public override void _Ready()
    {
        _instance = this;
        
        // Find relevant world objects
        inspectPoint = GetTree().CurrentScene.GetNode<Spatial>("Points/InspectPoint");
        objectHoldPoint = Vector3.Zero;
        objectHoldRotation = Vector3.Zero; 
        spaceState = GetWorld().DirectSpaceState;
        camera = GetViewport().GetCamera();
        _tooltip = GetNode<Label3D>("Tooltip");
        
        GetTree().CurrentScene.Connect("LevelReset", this, nameof(FreeObject));
    }

    public void FreeObject()
    {
        DropObject();
        BaseScene.ChangePlayerState(BaseScene.PlayerState.Normal);
    }

    private void AttemptInteraction()
    {
        if (hoverObject != null && !IsGrabbing())
        {
            ClearTooltip();
            
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
                BaseScene.ChangePlayerState(BaseScene.PlayerState.Inspecting);

                _revertToNormalOnDrop = true; 
            }

            if (hoverObject is GrabbableObject grabbableObject)
            {
                grabbedObject = grabbableObject;
                ignoreObjects = new Array() { grabbedObject };
                grabbableObject.Grab();
                ChangeCursorState(CursorState.HandClosed);
                BaseScene.ChangePlayerState(BaseScene.PlayerState.Grabbing);

                _revertToNormalOnDrop = false; 
            }
        }
    }

    private void DropObject()
    {
        // Holding onto an object
        if (IsInstanceValid(grabbedObject) && IsGrabbing())
        {
            if (grabbedObject is GrabbableObject grabbableObject) grabbableObject.Drop();
            if (grabbedObject is ViewableObject viewableObject) viewableObject.Drop();

            // Drop the grabbed object, release the reference
            ChangeCursorState(CursorState.HandOpen);
            BaseScene.ChangePlayerState(BaseScene.PlayerState.Normal);
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
                        if (!BaseScene.Inspecting() || _revertToNormalOnDrop)
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

        //_tooltip.GlobalTranslation = objectHoldPoint;
        Translation = pos + (normal * cursorDistance);
    }

    private void CheckForInteractiveObjects(Vector3 pos, Vector3 normal)
    {
        Dictionary interacts = spaceState.IntersectRay(
            pos, 
            pos + (normal * 1000f), 
            ignoreObjects, 
            LAYER_INTERACTIVE,
            true, 
            IsGrabbing() // Resolves a bug where a hotdog / interaction cannot be accessed through a snaptrigger
        );

        // Reveal the pointing finger when over something that can be clicked
        if (interacts.Count > 0)
        {
            hoverObject = (Node)interacts["collider"];

            if (hoverObject is InteractableObject && !Input.IsMouseButtonPressed((int)ButtonList.Left) && !IsGrabbing())
            {
                ChangeCursorState(CursorState.HandPoint);
                ChangeTooltip("Interact");
            }
            else if (hoverObject is ViewableObject && !IsGrabbing())
            {
                ChangeCursorState(CursorState.HandMagnify);
                ChangeTooltip("View");
            }
            else if (hoverObject is Rat rat)
            {
                ChangeCursorState(CursorState.HandGrab);
                ChangeTooltip("Rat!");
                rat.Alert();
            }            
            else if (hoverObject is GrabbableObject && !IsGrabbing())
            {
                ChangeCursorState(CursorState.HandGrab);
                ChangeTooltip("Grab");
            }
            else if (hoverObject is SnapTrigger snapTrigger)
            {
                objectHoldPoint = snapTrigger.GetSnapPoint();
                objectHoldRotation = snapTrigger.GetSnapRotation();
                ClearTooltip();
            }
            else if (hoverObject is Trigger trigger)
            {
                ChangeTooltip(trigger.GetTooltip());
            }
        }
        else
        {
            ClearTooltip();
            ChangeCursorState(CursorState.HandOpen);
        }
    }

    private void UpdateGrabbedObjectPosition(float delta)
    {
        if (IsInstanceValid(grabbedObject) && grabbedObject is GrabbableObject grabbableObject)
        {
            if (BaseScene.Inspecting())
            {
                ChangeCursorState(CursorState.HandOpen);
                grabbableObject.UpdateTargetPosition(inspectPoint.GlobalTranslation);
            }
            else
            {
                ChangeCursorState(CursorState.HandClosed);
                grabbableObject.UpdateTargetPosition(objectHoldPoint);
                grabbableObject.UpdateTargetRotation(objectHoldRotation, delta);
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
        if (IsGrabbing()) UpdateGrabbedObjectPosition(delta);
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

            case CursorState.HandGrab:
                Texture = handTextureGrab;
                break; 
            
            default:
            case CursorState.HandOpen:
                Texture = handTextureOpen;
                break;
        }
    }

    public static void ChangeTooltip(string text)
    {
        if (_tooltip != null) _tooltip.Text = text; 
        else GD.PrintErr("Tooltip not assigned");
    }

    public static void ClearTooltip()
    {
        if (_tooltip != null) _tooltip.Text = "";
        else GD.PrintErr("Tooltip not assigned");
    }

    public static void ChangeGrabbedObject(Spatial newGrab)
    {
        _instance.grabbedObject = newGrab;
    }

    public static void ForceReleaseObject(Spatial obj, bool ignoreMatch = false)
    {
        if (ignoreMatch)
        {
            _instance.DropObject();
            return;
        }
        
        if (IsInstanceValid(_instance.grabbedObject) && obj == _instance.grabbedObject)
        {
            _instance.DropObject();
        }
    }
}