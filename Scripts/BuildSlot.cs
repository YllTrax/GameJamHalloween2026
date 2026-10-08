using System;
using Godot;

public partial class BuildSlot : Node2D, IInteractable
{
    [Export]
    private PackedScene toarch;

    [Export]
    private PackedScene tourelle;

    [Export]
    private PackedScene tourelleBrulante;

    public void Interact()
    {
        var building = toarch.Instantiate<Toarch>();
        GetTree().CurrentScene.AddChild(building);
        building.GlobalPosition = GlobalPosition;
    }

    public void InteractMiddleButton()
    {
        var building = tourelleBrulante.Instantiate<Tourelle>();
        GetTree().CurrentScene.AddChild(building);
        building.GlobalPosition = GlobalPosition;
    }

    public void InteractRightclick()
    {
        var building = tourelle.Instantiate<Tourelle>();
        GetTree().CurrentScene.AddChild(building);
        building.GlobalPosition = GlobalPosition;
    }
}
