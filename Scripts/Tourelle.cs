using System.Collections.Generic;
using Godot;

public partial class Tourelle : Area2D, IBuildable
{
    public enum TowerType
    {
        Slime,
        Fire,
        Bullet,
    }

    [Export]
    private float attackCD = 0.5f;

    [Export]
    private int munitions = 10;

    [Export]
    private Label ammo;

    [Export]
    private int munitionMax;

    [Export]
    private TowerType towerType;

    [Export]
    private AnimatedSprite2D animatedSprite2D;

    [Export]
    private AnimatedSprite2D fireSprite;

    [Export]
    private PackedScene bulletPrefab;

    [Export]
    private Node2D firePoint;

    public Node2D target;

    private List<Node2D> bodies = new();
    private Timer _timerTarget;
    private Timer _timerFire;
    private int MaxAmmo => Mathf.RoundToInt(munitionMax * UpgradeStats.TurretAmmoMult);
    private int _lastShown = -1;

    private int _lastMax;

    [Export]
    private bool shouldFireInArc;

    [Export]
    private AudioStreamPlayer2D audioShoot;

    [Export]
    private AudioStreamPlayer2D audioReload;

    [Export]
    public int Cost { get; set; } = 10;

    public override void _Ready()
    {
        _lastMax = MaxAmmo;
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

        animatedSprite2D.Play("Idle");
    }

    public override void _Process(double delta)
    {
        CheckCapacityUpgrade();
        UpdateLabel();
        if (!IsInstanceValid(target))
        {
            target = null;
            return;
        }
        //
        //tourelleBody.LookAt(target.GlobalPosition);
        // supprime la ligne ammo.Text = munitions.ToString();
        // tourelleBody.LookAt(target.GlobalPosition);

        ammo.Text = munitions.ToString();
    }

    private void Fire()
    {
        if (target == null || !IsInstanceValid(target))
            return;

        if (munitions == 0)
            return;

        float dmgMult = UpgradeStats.TurretDamageMult;

        if (!shouldFireInArc)
        {
            fireSprite.Play("Fire");
            var bullet = bulletPrefab.Instantiate<Bullet>();
            bullet.Damage *= dmgMult;
            GD.Print($"[Tourelle] Bullet : x{dmgMult:0.00} -> dégâts {bullet.Damage}");
            GetTree().CurrentScene.AddChild(bullet);
            bullet.GlobalPosition = firePoint.GlobalPosition;
            bullet.dir = (target.GlobalPosition - firePoint.GlobalPosition).Normalized();
            munitions--;
            if (munitions == 0)
            {
                animatedSprite2D.Play("Inactive");
            }
            audioShoot.Play();
        }
        else
        {
            switch (towerType)
            {
                case TowerType.Fire:
                    fireSprite.Play("Fire");
                    var zone = bulletPrefab.Instantiate<FireZone>();
                    zone.Damage *= dmgMult;
                    GD.Print($"[Tourelle] FireZone : x{dmgMult:0.00} -> dégâts {zone.Damage}");
                    GetTree().CurrentScene.AddChild(zone);
                    zone.Launch(firePoint.GlobalPosition, target.GlobalPosition);
                    munitions--;
                    if (munitions == 0)
                    {
                        animatedSprite2D.Play("Inactive");
                    }
                    audioShoot.Play();
                    break;
                case TowerType.Slime:
                    fireSprite.Play("Fire");
                    var zoneSlime = bulletPrefab.Instantiate<SlimeZone>();
                    GetTree().CurrentScene.AddChild(zoneSlime);
                    zoneSlime.Launch(firePoint.GlobalPosition, target.GlobalPosition);
                    munitions--;
                    if (munitions == 0)
                    {
                        animatedSprite2D.Play("Inactive");
                    }
                    audioShoot.Play();
                    break;
            }
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

    public void Reload()
    {
        var p = Personnage.Instance;
        int space = MaxAmmo - munitions;
        if (space <= 0 || p.munitions <= 0)
            return;

        int transfer = Mathf.Min(space, p.munitions);
        munitions += transfer;
        p.munitions -= transfer;
        audioReload.Play();
        animatedSprite2D.Play("Idle");
    }

    private void UpdateLabel()
    {
        int shown = munitions * 1000 + MaxAmmo; // détecte un changement de l'un ou l'autre
        if (shown == _lastShown)
            return;
        _lastShown = shown;
        ammo.Text = $"{munitions}/{MaxAmmo}";
    }

    private void CheckCapacityUpgrade()
    {
        int max = MaxAmmo;

        if (max > _lastMax)
            munitions = Mathf.Min(munitions + (max - _lastMax), max);

        _lastMax = max;
    }
}
