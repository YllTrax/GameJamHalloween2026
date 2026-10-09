using System;
using Godot;

public partial class Toarch : Node2D, IBuildable
{
    [Export]
    private DamageSource areaOfEffect;

    [Export]
    private AnimatedSprite2D anim;

    [Export]
    private float attackCD;
    private Tween burnTween;
    private Timer _timer;

    [Export]
    public int Cost { get; set; } = 10;

    public override void _Ready()
    {
        anim.Play();
        _timer = new Timer();
        _timer.WaitTime = attackCD; // Ca repete toutes les attackCD secondes
        _timer.OneShot = false; // pour faire la repetition
        _timer.Timeout += Burn; // on abone la bonne fonction
        AddChild(_timer); // on ajoute le timer de facon dynamique
        _timer.Start(); // bon je pense que c'est explicite XD
    }

    private void Burn()
    {
        burnTween?.Kill();

        areaOfEffect.Scale = Vector2.One;
        areaOfEffect.Visible = true;
        areaOfEffect.ProcessMode = ProcessModeEnum.Inherit;

        burnTween = CreateTween();
        burnTween
            .TweenProperty(areaOfEffect, "scale", new Vector2(1.5f, 1.5f), 0.5f)
            .SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);
        burnTween.TweenInterval(1.0);
        burnTween.TweenProperty(areaOfEffect, "scale", Vector2.One, 0.15f);
        burnTween.TweenCallback(
            Callable.From(() =>
            {
                areaOfEffect.Visible = false;
                areaOfEffect.ProcessMode = ProcessModeEnum.Disabled;
            })
        );
    }
}
