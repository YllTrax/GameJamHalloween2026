using System;
using Godot;

public partial class Health : Node2D
{
    [Export]
    public float baseHealth;

    [Export]
    protected HealthBar healthBar;

    [Export]
    protected AnimatedSprite2D sprite;

    [Export]
    public bool IsPlayer; // a assigner dans l'inspecteur

    [Export]
    public bool IsSaltCircle; // a assigner dans l'inspecteur

    public float currentHealth;

    public event Action Died;
    private bool isDead;

    private float burningTimer;

    [Export]
    private float burningDuration;

    [Export]
    private float burnPercentPerSecond = 10f;

    public override void _Ready()
    {
        currentHealth = baseHealth;
        burningTimer = burningDuration;
    }

    public bool isBurning;

    public override void _Process(double delta)
    {
        Burn(delta);
    }

    /// <summary>
    /// Methode pour infliger des degats du montant "damage"
    /// </summary>
    /// <param name="damage"></param>
    public virtual void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        if (sprite != null)
        {
            sprite.Modulate = Colors.Red;
            CreateTween().TweenProperty(sprite, "modulate", Colors.White, 0.15f);
        }
        healthBar?.OnHealthCHangeDamage(damage);

        if (currentHealth <= 0)
        {
            isDead = true;
            GD.Print(
                $"[Health] {GetParent().Name} meurt | script={GetType().Name} | abonnés={(Died != null)}"
            );
            Died?.Invoke();
            GetParent().QueueFree();
        }
    }

    /// <summary>
    /// Methode pour soigner du montant "heal"
    /// </summary>
    public void ToHeal(float heal)
    {
        currentHealth += heal;
        sprite.Modulate = Colors.Green;
        CreateTween().TweenProperty(sprite, "modulate", Colors.White, 0.15f);
        healthBar.OnHealthCHangeHeal(heal);
        if (currentHealth > baseHealth)
            currentHealth = baseHealth;
    }

    public override void _EnterTree()
    {
        // if (IsSaltCircle)
        // {
        //     SaltCircle.Instance.SaltRefilled += OnRefill;
        // }
    }

    private void OnRefill(float saltAmount)
    {
        ToHeal(saltAmount);
    }

    public void Burn(double delta)
    {
        if (isBurning)
        {
            TakeDamage((baseHealth * burnPercentPerSecond / 100) * (float)delta);
            burningTimer -= (float)delta;
            if (burningTimer <= 0)
            {
                isBurning = false;
                burningTimer = burningDuration;
            }
        }
    }
}
