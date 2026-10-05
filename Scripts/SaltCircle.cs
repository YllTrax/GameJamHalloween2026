using System;
using Godot;

public partial class SaltCircle : Area2D
{
    public static SaltCircle Instance { get; private set; }

    private float saltAmount;

    public override void _EnterTree()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }
        Instance = this;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public override void _Ready() { }

    public override void _Process(double delta) { }
}
