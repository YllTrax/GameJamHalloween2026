using System.Collections.Generic;
using Godot;

public partial class DamageSource : Area2D
{
    public enum TargetMode
    {
        All,
        NotPersonnage,
    }

    [Export]
    private TargetMode targetMode = TargetMode.All;

    [Export]
    public float Damage { get; set; } = 100;

    [Export]
    private bool shouldSelfDestroy = false;

    private bool _used;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private bool CanHit(Node2D body)
    {
        switch (targetMode)
        {
            case TargetMode.NotPersonnage:
                return !(body is Personnage);
            default:
                return true;
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        if (_used)
            return;

        if (body == GetParent())
            return;

        if (!CanHit(body))
            return;

        var health = body.GetNodeOrNull<Health>("Health");
        if (health == null)
            return;

        health.TakeDamage(Damage);
        ;

        if (shouldSelfDestroy)
        {
            _used = true;

            GetParent().QueueFree();
        }
    }
}
