using System;
using Godot;

public partial class BuildSlot : Node2D, IInteractable
{
    [Export]
    private PackedScene toarch;

    public void Interact()
    {
        var building = toarch.Instantiate<Toarch>();
        GetTree().CurrentScene.AddChild(building);
        building.GlobalPosition = GlobalPosition;
    }
}
