using System;
using Godot;

public partial class Bullet : Area2D
{
    private Node2D target;

    [Export]
    public float moveSpeed;

    [Export]
    private float lifeTime = 2;
    private Timer lifeTimer;
    public Vector2 dir;

    public override void _Ready()
    {
        //        dir = target.GlobalPosition;

        lifeTimer = new Timer();
        lifeTimer.WaitTime = lifeTime;
        lifeTimer.Timeout += SelfDestroy;
        AddChild(lifeTimer);
        lifeTimer.Start();
    }

    private void SelfDestroy()
    {
        QueueFree();
    }

    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition += dir * moveSpeed * (float)delta;
    }
}
