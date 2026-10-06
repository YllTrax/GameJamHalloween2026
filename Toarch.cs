using System;
using Godot;

public partial class Toarch : Node2D
{
    [Export]
    private DamageSource areaOfEffect;

    [Export]
    private AnimatedSprite2D anim;

    [Export]
    private float attackCD;

    private Timer _timer;

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

    private void Burn() { }
}
