using System;
using Godot;

public partial class FireZone : Area2D
{
    //-------au vol --------------
    [Export]
    private float moveSpeed = 300f;

    [Export]
    private float arcHeight = 60f;

    //--------------a terre--------------
    [Export]
    private float lifeTime = 3f;

    [Export] private DamageSource damageSource;

    public float Damage
    {
        get => damageSource.Damage;
        set => damageSource.Damage = value;
    }

    /// <summary>
    /// Lance la zone de "from" vers "to". À appeler APRÈS AddChild.
    /// </summary>
    public void Launch(Vector2 from, Vector2 to)
    {
        GlobalPosition = from;
        Monitoring = false;

        float duration = from.DistanceTo(to) / moveSpeed;

        var tween = CreateTween();
        tween.SetProcessMode(Tween.TweenProcessMode.Physics);

        tween.TweenMethod(
            Callable.From<float>(t =>
            {
                Vector2 pos = from.Lerp(to, t);
                pos.Y -= Mathf.Sin(t * Mathf.Pi) * arcHeight;
                GlobalPosition = pos;
            }),
            0f,
            1f,
            duration
        );

        tween.TweenCallback(Callable.From(OnLanded));
    }

    private void OnLanded()
    {
        Monitoring = true;

        Vector2 baseScale = Scale;
        Scale = baseScale * 0.5f;

        var tween = CreateTween();
        tween
            .TweenProperty(this, "scale", baseScale, 0.2f)
            .SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);

        if (lifeTime > 0)
        {
            tween.TweenInterval(lifeTime);
            tween.TweenProperty(this, "scale", Vector2.Zero, 0.2f).SetEase(Tween.EaseType.In);
            tween.TweenCallback(Callable.From(QueueFree));
        }
    }

    public override void _Ready()
    {
        Monitoring = false;
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Pumpkin)
        {
            Health health = body.GetNodeOrNull<Health>("Health");
            health.isBurning = true;
        }
    }
}
