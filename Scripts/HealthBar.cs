using Godot;

public partial class HealthBar : TextureProgressBar
{
    [Export]
    private Health health;

    public override void _Ready()
    {
        if (health == null)
        {
            GD.PushWarning($"{Name} : Health non assigné");
            return;
        }

        health.HealthChanged += OnHealthChanged;
        OnHealthChanged(health.currentHealth, health.baseHealth); // état initial
    }

    public override void _ExitTree()
    {
        if (health != null && IsInstanceValid(health))
            health.HealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float current, float max)
    {
        MaxValue = max;
        Value = current;
    }
}
