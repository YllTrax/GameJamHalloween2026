using System;
using System.Collections.Generic;
using Godot;

public partial class SlimeZone : Area2D
{
    //-------au vol --------------
    [Export]
    private float moveSpeed = 300f;

    [Export]
    private float arcHeight = 100f;

    //--------------a terre--------------
    [Export]
    private float lifeTime = 2f;

    [Export]
    private PackedScene munition;
    private List<CharacterBody2D> targets = new List<CharacterBody2D>();

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
            if (munition != null)
            {
                Vector2 pos = GlobalPosition;
                var ammo = munition.Instantiate<Munition>();
                ammo.Position = pos;
                ammo.SetDrop(
                    pos + Vector2.FromAngle(GD.Randf() * Mathf.Tau) * (40f + GD.Randf() * 60f)
                );

                GetTree().CurrentScene.CallDeferred(Node.MethodName.AddChild, ammo);
            }

            tween.TweenInterval(lifeTime);
            tween.TweenCallback(Callable.From(ToReleaseAll));
            tween.TweenProperty(this, "scale", Vector2.Zero, 0.2f).SetEase(Tween.EaseType.In);
            tween.TweenCallback(Callable.From(QueueFree));
        }
    }

    public override void _Ready()
    {
        Monitoring = false;
        BodyEntered += OnBodyEntered;
        // BodyExited -= OnBodyExited;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is CharacterBody2D chara)
        {
            targets.Add(chara);
            chara.SetPhysicsProcess(false);
        }
    }

    private void ToReleaseAll()
    {
        Monitoring = false; // ca pour eviter les recaptures

        foreach (CharacterBody2D chara in targets)
        {
            chara.SetPhysicsProcess(true);
        }
    }

    public override void _ExitTree()
    {
        ToReleaseAll();
    }
}
