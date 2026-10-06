using System;
using System.Net.Http.Headers;
using Godot;

public partial class HealthBar : ProgressBar
{
    [Export]
    private Health health;

    public override void _Ready() { }
}
