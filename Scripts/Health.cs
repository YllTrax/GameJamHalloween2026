using System;
using Godot;

public partial class Health : Node
{
    [Export]
    private float baseHealth;

    [Export]
    private AnimatedSprite2D sprite;
    public float currentHealth;

    public override void _Ready()
    {
        currentHealth = baseHealth;
    }

    public override void _Process(double delta) { }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        sprite.Modulate = Colors.Red;
        CreateTween().TweenProperty(sprite, "modulate", Colors.White, 0.15f);

        if (currentHealth <= 0)
            GetParent().QueueFree();
    }
}
