using System;
using Godot;

public partial class Range : Area2D
{
    [Export]
    Interactable interactable;

    public override void _Ready()
    {
        BodyEntered += PlayerInRange;
        BodyExited += PlayerExitRange;
    }

    public override void _Process(double delta) { }

    private void PlayerInRange(Node2D body)
    {
        if (body is Personnage player)
        {
            Personnage.Instance.iSIndoor = true;
        }
    }

    private void PlayerExitRange(Node2D body)
    {
        // if (body is Personnage player)
        // {
        //     interactable.IsPlayerInRange = false;
        // }
    }
}
