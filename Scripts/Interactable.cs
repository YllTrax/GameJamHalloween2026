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
            if (GetParent() is IInteractable target)
            {
                GD.Print("clic detecte");
                if (DetecteIfPersonnageInRange())
                {
                    target.Interact();
                }
            }
        }
        if (
            @event is InputEventMouseButton mbRight
            && mbRight.Pressed
            && mbRight.ButtonIndex == MouseButton.Right
        )
        {
            if (GetParent() is IInteractable target)
            {
                GD.Print("clic detecte");
                if (DetecteIfPersonnageInRange())
                {
                    target.InteractRightclick();
                }
            }
        }
        if (
            @event is InputEventMouseButton mbMiddle
            && mbMiddle.Pressed
            && mbMiddle.ButtonIndex == MouseButton.Middle
        )
        {
            if (GetParent() is IInteractable target)
            {
                GD.Print("clic detecte");
                if (DetecteIfPersonnageInRange())
                {
                    target.InteractMiddleButton();
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
