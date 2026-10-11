using System;
using Godot;

public partial class Endurance : TextureProgressBar
{
    public override void _Ready()
    {
        Personnage.Instance.PlayerDash += OnDash;
    }

    public override void _Process(double delta)
    {
        if (Value < MaxValue)
        {
            Value += MaxValue / 1.5 * (float)delta;
        }
    }

    private void OnDash()
    {
        Value = 0;
    }
}
