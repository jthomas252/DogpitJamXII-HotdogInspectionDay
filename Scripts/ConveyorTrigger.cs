using Godot;
using Godot.Collections;

public class ConveyorTrigger : Trigger
{
    private readonly float MOVEMENT_SCALE = 35f;
    private Vector3 _direction;
    private bool _moving; 
    
    public override void _Ready()
    {
        base._Ready();
        _direction = GetNode<Spatial>("Direction").GlobalTranslation;
        
        GetTree().CurrentScene.Connect("LevelStart", this, nameof(Start));
        GetTree().CurrentScene.Connect("LevelEnd", this, nameof(Stop));
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        if (_moving)
        {
            Array bodies = GetOverlappingBodies();

            foreach (var body in bodies)
            {
                if (body is RigidBody rigidBody)
                {
                    rigidBody.SetAxisVelocity(rigidBody.GlobalTranslation.DirectionTo(_direction) * MOVEMENT_SCALE);
                }

                if (body is Rat rat)
                {
                    rat.Stun(10f);
                }
            }
        }
    }

    public void Start()
    {
        _moving = true; 
    }

    public void Stop()
    {
        _moving = false; 
    }
}