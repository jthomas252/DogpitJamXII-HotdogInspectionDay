using Godot;

public class ViewableDocument : ViewableObject
{
    private Button _prevButton;
    private Button _nextButton;

    [Export] public NodePath previous;
    [Export] public NodePath next;
    
    private ViewableObject _prev;
    private ViewableObject _next;

    private static bool _switchedThisFrame;
    
    public override void _Ready()
    {
        base._Ready();
        
        // Get the UI buttons for next and previous 
        _prevButton = GetTree().CurrentScene.GetNode<Button>("Interface/PrevButton");
        _nextButton = GetTree().CurrentScene.GetNode<Button>("Interface/NextButton");

        if (previous != null) _prev = GetNodeOrNull<ViewableObject>(previous);
        if (next != null) _next = GetNodeOrNull<ViewableObject>(next);
        
        _prevButton.Connect("pressed", this, nameof(OnPrevButton));
        _nextButton.Connect("pressed", this, nameof(OnNextButton));             
    }

    public override void Inspect()
    {
        base.Inspect();
        Cursor.ChangeGrabbedObject(this);
        if (_prev != null) _prevButton.Visible = true;
        if (_next != null) _nextButton.Visible = true;
        
        BaseScene.PlaySound(BaseScene.GetDocumentNoise());
    }

    public override void Drop()
    {
        base.Drop();
        if (_prev != null) _prevButton.Visible = false;
        if (_next != null) _nextButton.Visible = false;
    }

    public void OnPrevButton()
    {
        if (_inspecting && _prev != null && !_switchedThisFrame)
        {
            SwitchTo(_prev);
        }
    }

    public void OnNextButton()
    {
        if (_inspecting && _next != null && !_switchedThisFrame)
        {
            SwitchTo(_next);
        }
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        
        // This is to prevent issues with event propagation 
        if (_switchedThisFrame) _switchedThisFrame = false; 
    }

    private void SwitchTo(ViewableObject switchTarget)
    {
        _switchedThisFrame = true; 
        Drop();
        switchTarget.Inspect();
        GD.Print($"Switch document from ${Name} to ${switchTarget.Name}");
    }
}
