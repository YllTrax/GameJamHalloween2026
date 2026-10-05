using System.Security;
using System.Threading.Tasks.Dataflow;
using Godot;

public partial class Personnage : CharacterBody2D
{
    public static Personnage Instance { get; private set; }

    [Export]
    private AnimatedSprite2D anim;

    [Export]
    private float moveSpeed;

    [Export]
    private float startSpeed = 200f;

    [Export]
    private float dashSpeed = 2000f;

    [Export]
    private float accel = 1500f;

    private bool isDashing = false;
    private bool canDash = true;

    public bool iSIndoor = false;

    private Vector2 mouseDir;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        moveSpeed = startSpeed;
        // anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _Process(double delta)
    {
        mouseDir = GetLocalMousePosition();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("dash"))
        {
            Dash();
        }
        Vector2 dir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        Velocity = Velocity.MoveToward(dir * moveSpeed, accel * (float)delta);
        MoveAndSlide();

        if (dir == Vector2.Zero)
        {
            anim.Play("Idle");
        }
        else
        {
            anim.Play("Run");
        }
        anim.FlipH = mouseDir.X < 0f;
    }

    public override void _Input(InputEvent @event) { }

    private async void Dash()
    {
        if (!isDashing && canDash)
        {
            isDashing = true;
            canDash = false;
            moveSpeed = dashSpeed;
            await ToSignal(GetTree().CreateTimer(0.5), SceneTreeTimer.SignalName.Timeout);
            moveSpeed = startSpeed;
            isDashing = false;
            await ToSignal(GetTree().CreateTimer(1.5f), SceneTreeTimer.SignalName.Timeout);
            canDash = true;
        }
    }
}
