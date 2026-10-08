using System;
using Godot;

public partial class Personnage : CharacterBody2D
{
    public enum TempAnim
    {
        North,
        South,
        West,
        East,
    }

    public static Personnage Instance { get; private set; }
    public event Action<TempAnim> DirectionChange;

    [ExportGroup("Références")]
    [Export]
    private AnimatedSprite2D anim;

    [ExportGroup("Mouvement")]
    [Export]
    private float startSpeed = 200f;

    [Export]
    private float accel = 1500f;

    [ExportGroup("Dash")]
    [Export]
    private float dashSpeed = 2000f;

    [ExportGroup("Inventaire")]
    [Export]
    public float carriedSalt = 0;

    [Export]
    public int germs = 0;

    [ExportGroup("Debug (runtime)")]
    [Export]
    public TempAnim lastAnim;

    [Export]
    private float moveSpeed;

    [ExportGroup("")] // fin des groupes
    private float maxCarriedSalt;
    private bool isDashing = false;
    private bool canDash = true;
    public bool iSIndoor = false;
    private Vector2 mouseDir;

    public override void _EnterTree()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }
        Instance = this;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public override void _Ready()
    {
        moveSpeed = startSpeed;
    }

    public override void _Process(double delta)
    {
        mouseDir = GetLocalMousePosition();
        ColorisePlayerInDoor();
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

        UpdateAnimation(dir);
    }

    private void UpdateAnimation(Vector2 dir)
    {
        // On convertit en -1, 0 ou 1
        int x = (int)Mathf.Sign(dir.X);
        int y = (int)Mathf.Sign(dir.Y);

        TempAnim dirAnim = lastAnim;

        switch (x, y)
        {
            case (0, -1):
                dirAnim = TempAnim.North;
                break;
            case (0, 1):
                dirAnim = TempAnim.South;
                break;
            case (-1, 0):
                dirAnim = TempAnim.West;
                break;
            case (1, 0):
                dirAnim = TempAnim.East;
                break;
            case (_, -1):
                dirAnim = TempAnim.North;
                break;
            case (_, 1):
                dirAnim = TempAnim.South;
                break;
        }

        if (dirAnim != lastAnim)
        {
            lastAnim = dirAnim;
            DirectionChange?.Invoke(dirAnim);
        }

        anim.Play(GetAnim(lastAnim));
        anim.FlipH = lastAnim == TempAnim.East;
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

    private void ColorisePlayerInDoor()
    {
        if (iSIndoor)
        {
            anim.Modulate = Colors.Crimson;
        }
        else
        {
            anim.Modulate = Colors.White;
        }
    }

    private void ToCarrySalt() { }

    private void ToDropSalt() { }

    internal void OnCollect(int value)
    {
        germs += value;
    }

    private void StoreTempAnim(TempAnim temp)
    {
        lastAnim = temp;
    }

    private string GetAnim(TempAnim temp)
    {
        switch (temp)
        {
            case TempAnim.North:
                return "IdleNorth";
            case TempAnim.West:
            case TempAnim.East:
                return "IdleWest";
            default:
                return "IdleSouth";
        }
    }
}
