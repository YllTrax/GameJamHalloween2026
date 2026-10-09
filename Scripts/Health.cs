using System;
using Godot;

public partial class Health : Node2D
{
	[Export]
	public float baseHealth;

	[Export]
	protected AnimatedSprite2D sprite;

	[Export]
	public bool IsPlayer;

	[Export]
	public bool IsSaltCircle;

	[Export]
	private float burnPercentPerSecond = 10f;

	[Export]
	private float deltaWitch = 10f;

	[Export]
	private float witchDamage = 10f;

	[Export]
	private float witchTimer = 10f;

	[Export]
	private float burningDuration = 3f; // durée de la brûlure en secondes

	public float currentHealth;
	public bool isBurning;

	public event Action Died;
	public event Action<float, float> HealthChanged; // (vie actuelle, vie max)

	private bool isDead;
	private float burningTimer;

	public override void _Ready()
	{
		currentHealth = baseHealth;
		burningTimer = burningDuration;
		HealthChanged?.Invoke(currentHealth, baseHealth);
		deltaWitch = witchTimer;
	}

	public override void _Process(double delta)
	{
		Burn(delta);
		if (IsSaltCircle)
		{
			WitchAttack((float)delta);
		}
	}

	public virtual void TakeDamage(float damage)
	{
		if (isDead)
			return;

		currentHealth = Mathf.Max(currentHealth - damage, 0);
		Flash(Colors.Red);
		HealthChanged?.Invoke(currentHealth, baseHealth);

		if (currentHealth <= 0)
		{
			isDead = true;
			Died?.Invoke();
			GetParent().QueueFree();
		}
	}

	public void ToHeal(float heal)
	{
		if (isDead)
			return;

		currentHealth = Mathf.Min(currentHealth + heal, baseHealth);
		Flash(Colors.Green);
		HealthChanged?.Invoke(currentHealth, baseHealth);
	}

	public void StartBurn()
	{
		isBurning = true;
		burningTimer = burningDuration;
	}

	public void Burn(double delta)
	{
		if (!isBurning)
			return;

		TakeDamage((baseHealth * burnPercentPerSecond / 100f) * (float)delta);
		burningTimer -= (float)delta;
		if (burningTimer <= 0)
			isBurning = false;
	}

	private void Flash(Color color)
	{
		if (sprite == null)
			return;
		sprite.Modulate = color;
		CreateTween().TweenProperty(sprite, "modulate", Colors.White, 0.15f);
	}

	private void OnRefill(float saltAmount)
	{
		ToHeal(saltAmount);
	}

	private void WitchAttack(float delta)
	{
		deltaWitch -= delta;
		if (deltaWitch <= 0)
		{
			TakeDamage(witchDamage);
			deltaWitch = witchTimer;
		}
	}
}
