using System.Collections.Generic;
using Godot;

public partial class Tourelle : Area2D
{
    public enum TowerType
    {
        Slime,
        Fire,
    }

    [Export]
    private float attackCD = 0.5f;

    [Export]
    private TowerType towerType;

    [Export]
    private PackedScene bulletPrefab;

    [Export]
    private Node2D firePoint;

    public Node2D target;

    private List<Node2D> bodies = new();
    private Timer _timerTarget;
    private Timer _timerFire;

    [Export]
    private bool shouldFireInArc;

    public override void _Ready()
    {
        BodyEntered += OnBodyEnter;
        BodyExited += OnBodyExit;

        _timerTarget = new Timer { WaitTime = 0.5, OneShot = false };
        _timerTarget.Timeout += FindTarget;
        AddChild(_timerTarget);
        _timerTarget.Start();

        _timerFire = new Timer { WaitTime = attackCD, OneShot = false };
        _timerFire.Timeout += Fire;
        AddChild(_timerFire);
        _timerFire.Start();
    }

    public override void _Process(double delta)
    {
        if (!IsInstanceValid(target))
        {
            target = null;
            return;
        }
        LookAt(target.GlobalPosition);
    }

    private void Fire()
    {
        if (target == null || !IsInstanceValid(target))
            return;
        if (!shouldFireInArc)
        {
            var bullet = bulletPrefab.Instantiate<Bullet>();
            GetTree().CurrentScene.AddChild(bullet);
            bullet.GlobalPosition = firePoint.GlobalPosition;
            bullet.dir = (target.GlobalPosition - firePoint.GlobalPosition).Normalized();
        }
        else
            switch (towerType)
            {
                case TowerType.Fire:

                    var zone = bulletPrefab.Instantiate<FireZone>(); //---------------------------ici on tire une une zone j'ai aps encore chnage rle nom
                    GetTree().CurrentScene.AddChild(zone); // d'abord dans l'arbre
                    zone.Launch(firePoint.GlobalPosition, target.GlobalPosition); // puis on lance
                    return;
                case TowerType.Slime:
                    var zoneSlime = bulletPrefab.Instantiate<SlimeZone>(); //---------------------------ici on tire une une zone j'ai aps encore chnage rle nom
                    GetTree().CurrentScene.AddChild(zoneSlime); // d'abord dans l'arbre
                    zoneSlime.Launch(firePoint.GlobalPosition, target.GlobalPosition); // puis on lance
                    return;
            }
    }

    private void OnBodyEnter(Node2D body)
    {
        if (!bodies.Contains(body))
            bodies.Add(body);
    }

    private void OnBodyExit(Node2D body)
    {
        bodies.Remove(body);
        if (body == target)
            target = null;
    }

    private void FindTarget()
    {
        bodies.RemoveAll(b => !IsInstanceValid(b));

        float bestDist = float.MaxValue;
        Node2D best = null;

        foreach (Node2D body in bodies)
        {
            float dist = body.GlobalPosition.DistanceTo(GlobalPosition);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = body;
            }
        }
        target = best;
    }
}
