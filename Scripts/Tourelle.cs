using System;
using System.Collections.Generic;
using Godot;

public partial class Tourelle : Area2D
{
    private List<Node2D> bodies = new();

    [Export]
    private float attackCD;

    [Export]
    private PackedScene bulletPrefab;

    [Export]
    private Node2D firePoint;
    public Node2D target;
    private Timer _timerTarget;
    private Timer _timerFire;

    public override void _Ready()
    {
        BodyEntered += OnBodyEnter;
        _timerTarget = new Timer();
        _timerTarget.WaitTime = 0.5;
        _timerTarget.OneShot = false;
        _timerTarget.Timeout += FindTarget;
        AddChild(_timerTarget);
        _timerTarget.Start();
        _timerFire = new Timer();
        _timerFire.WaitTime = 0.5;
        _timerFire.OneShot = false;
        _timerFire.Timeout += Fire;
        AddChild(_timerFire);
        _timerFire.Start();
    }

    public override void _Process(double delta)
    {
        if (IsInstanceValid(target))
        {
            target = null;
            return;
        }
        LookAt(target.GlobalPosition);
    }

    private void Fire()
    {
        if (bodies.Count == 0)
            return;
        Vector2 dir = (target.GlobalPosition - GlobalPosition).Normalized();
        var bullet = bulletPrefab.Instantiate<Bullet>();
        bullet.dir = dir;
        bullet.GlobalPosition = firePoint.GlobalPosition;
        GetTree().CurrentScene.AddChild(bullet);
    }

    private void OnBodyEnter(Node2D body)
    {
        // if (body == GetParent())
        //     return;
        // if (body is Personnage || body is Tourelle)
        //     return;
        bodies.Add(body);
    }

    private void FindTarget()
    {
        if (bodies.Count == 0)
            return;
        float dist;
        float lastShortestDist = 5000;
        foreach (Node2D body in bodies)
        {
            dist = body.GlobalPosition.DistanceTo(GlobalPosition);
            if (dist < lastShortestDist)
            {
                lastShortestDist = dist;
                target = body;
            }
        }
    }

    private void OnBodyExit(Node2D body)
    {
        if (body == GetParent())
            return;
        if (body is Personnage || body is Tourelle)
            return;
        bodies.Remove(body);
    }
}
