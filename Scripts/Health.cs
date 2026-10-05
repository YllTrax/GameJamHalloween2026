using System;
using Godot;

public partial class Health : Node
{
	[Export]
	private float baseHealth;

<<<<<<< Updated upstream
    [Export]
    private AnimatedSprite2D sprite;

    [Export]
    public bool IsPlayer; // a assigner dans l'inspecteur

    [Export]
    public bool IsSaltCircle; // a assigner dans l'inspecteur

    public float currentHealth;
=======
	[Export]
	private AnimatedSprite2D sprite;
	public float currentHealth;
>>>>>>> Stashed changes

	public override void _Ready()
	{
		currentHealth = baseHealth;
	}

	public override void _Process(double delta) { }

<<<<<<< Updated upstream
    /// <summary>
    /// Methode pour infliger des degats du montant "damage"
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        sprite.Modulate = Colors.Red;
        CreateTween().TweenProperty(sprite, "modulate", Colors.White, 0.15f);

        if (currentHealth <= 0)
            GetParent().QueueFree();
    }

    /// <summary>
    /// Methode pour soigner du montant "heal"
    /// </summary>
    public void ToHeal(float heal)
    {
        currentHealth += heal;
        sprite.Modulate = Colors.Green;
        CreateTween().TweenProperty(sprite, "modulate", Colors.White, 0.15f);

        if (currentHealth < baseHealth)
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
=======
	public void TakeDamage(float damage)
	{
		currentHealth -= damage;
		sprite.Modulate = Colors.Red;
		CreateTween().TweenProperty(sprite, "modulate", Colors.White, 0.15f);

		if (currentHealth <= 0)
			GetParent().QueueFree();
	}
>>>>>>> Stashed changes
}
