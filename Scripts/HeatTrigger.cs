using Godot;

public class HeatTrigger:Trigger
{
    private readonly float HEAT_AMOUNT = 0.35f;

    [Export] public float extraHeat;
    
    public override void _Process(float delta)
    {
        base._Process(delta);
        var bodies = GetOverlappingBodies();
        foreach (var body in bodies)
        {
            if (body is RigidBody rigidBody && rigidBody.GetParent() is Hotdog hotdog)
            {
                hotdog.ApplyHeat((HEAT_AMOUNT + extraHeat) * delta);
            }
        }
    }
}
