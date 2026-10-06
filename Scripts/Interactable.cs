using System;
using Godot;

public partial class Interactable : Area2D
{
    [Export]
    private Polygon2D outline;

    [Export]
    private float range;

    public override void _Ready()
    {
        InputPickable = true;
        MouseEntered += () => OnMoueEntered();
        MouseExited += () => outline.Visible = false;
    }

    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Left)
        {
            if (GetParent() is IInteractable target) // mettre le inrange une fois le systeme  fait
            {
                GD.Print("clic detecte");
                if (DetecteIfPersonnageInRange())
                {
                    target.Interact();
                }
            }
        }
    }

    private void OnMoueEntered()
    {
        outline.Visible = true;
    }

    private bool DetecteIfPersonnageInRange()
    {
        float distanceToPersonnage = GlobalPosition.DistanceTo(Personnage.Instance.GlobalPosition);
        if (distanceToPersonnage <= range)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
