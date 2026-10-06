using System.Collections.Generic;
using System.Linq;
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
    private float damage = 100;

    [Export]
    private float hitCD = 0.5f;

    [Export]
    private bool debug = true; // décoche pour couper les prints

    private Timer hitTimer;
    private List<Health> targets = new();

    public override void _Ready()
    {
        hitTimer = new Timer { WaitTime = hitCD, OneShot = false };
        hitTimer.Timeout += OnHitTimeout;
        AddChild(hitTimer);

        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;

        Log(
            $"prêt | mode={targetMode} dmg={damage} cd={hitCD} "
                + $"mask={CollisionMask} monitoring={Monitoring}"
        );
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body == GetParent())
            return;
        Log($"ENTRÉE -> {body.Name} ({body.GetType().Name})");

        if (!CanHit(body))
        {
            Log("  ✗ bloqué par CanHit");
            return;
        }

        var health = body.GetNodeOrNull<Health>("Health");
        if (health == null)
        {
            var enfants = string.Join(
                ", ",
                body.GetChildren().Select(c => $"{c.Name} ({c.GetType().Name})")
            );
            Log($"  ✗ pas de Health. Enfants : {enfants}");
            return;
        }

        if (!targets.Contains(health))
            targets.Add(health);

        Log(
            $"  ✓ cible ajoutée | PV={health.currentHealth}/{health.baseHealth} "
                + $"| cibles={targets.Count}"
        );

        if (hitTimer.IsStopped())
        {
            HitAll();
            hitTimer.Start();
            Log("  timer démarré");
        }
    }

    private void OnBodyExited(Node2D body)
    {
        if (body.GetNodeOrNull<Health>("Health") is Health health)
        {
            targets.Remove(health);
            Log($"SORTIE -> {body.Name} | cibles={targets.Count}");
        }
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

    private void OnHitTimeout()
    {
        int avant = targets.Count;
        targets.RemoveAll(h => !IsInstanceValid(h));
        if (avant != targets.Count)
            Log($"nettoyage : {avant - targets.Count} cible(s) morte(s) retirée(s)");

        if (targets.Count > 0)
            HitAll();
        else
        {
            hitTimer.Stop();
            Log("plus de cibles, timer stoppé");
        }
    }

    private void HitAll()
    {
        foreach (var h in targets)
        {
            if (!IsInstanceValid(h))
                continue;

            string nom = h.GetParent().Name;
            float avant = h.currentHealth;
            Log($"  → frappe {nom} ({avant} PV)");

            try
            {
                h.TakeDamage(damage);
                Log($"  ✓ {nom} : {avant} -> {h.currentHealth} PV");
            }
            catch (System.Exception e)
            {
                GD.PrintErr(
                    $"[{Name}] ✗ TakeDamage a planté sur {nom} : "
                        + $"{e.GetType().Name} - {e.Message}"
                );
            }
        }
    }

    private void Log(string msg)
    {
        if (debug)
            GD.Print($"[{Name}] {msg}");
    }
}
