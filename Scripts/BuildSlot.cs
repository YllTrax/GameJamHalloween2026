using System;
using System.ComponentModel.Design.Serialization;
using Godot;

public partial class BuildSlot : Node2D, IInteractable
{
    [Export]
    private Godot.Collections.Array<BuildData> buildables;

    private IBuildable build;

    private bool isBuilt;

    public void Interact()
    {
        if (isBuilt)
            return;
        int selection = (int)Personnage.Instance.selectTower;
        var cost = buildables[selection].Cost;
        if (!CheckCost(cost))
        {
            return;
        }
        if (selection == 0)
        {
            var building = buildables[0].Scene.Instantiate<Toarch>();
            build = building;
            GetTree().CurrentScene.AddChild(building);
            building.GlobalPosition = GlobalPosition;
            PayTheCost(cost);
            isBuilt = true;
        }
        else
        {
            var building = buildables[selection].Scene.Instantiate<Tourelle>();
            build = building;
            GetTree().CurrentScene.AddChild(building);
            building.GlobalPosition = GlobalPosition;
            isBuilt = true;
            PayTheCost(cost);
        }
    }

    public void InteractMiddleButton()
    {
        throw new NotImplementedException();
    }

    public void InteractRightclick()
    {
        if (build == null)
            return;
        build.Reload();
    }

    private bool CheckCost(int cost)
    {
        if (cost > Personnage.Instance.germs)
        {
            return false;
        }
        return true;
    }

    private void PayTheCost(int cost)
    {
        Personnage.Instance.germs -= cost;
    }
}


//-------------------- Je savais pas ou garder ca donc je le laisse ici  ----------------------

// public void Interact()
// {
//     var cost = buildables[0].Cost;
//     if (!CheckCost(cost))
//         return;

//     var building = buildables[0].Scene.Instantiate<Toarch>();
//     GetTree().CurrentScene.AddChild(building);
//     building.GlobalPosition = GlobalPosition;
//     PayTheCost(cost);
// }

// public void InteractMiddleButton()
// {
//     var cost = buildables[1].Cost;
//     if (!CheckCost(cost))
//         return;

//     var building = buildables[1].Scene.Instantiate<Tourelle>();
//     GetTree().CurrentScene.AddChild(building);
//     building.GlobalPosition = GlobalPosition;
//     PayTheCost(cost);
// }

// public void InteractRightclick()
// {
//     var cost = buildables[2].Cost;
//     if (!CheckCost(cost))
//         return;

//     var building = buildables[2].Scene.Instantiate<Tourelle>();
//     GetTree().CurrentScene.AddChild(building);
//     building.GlobalPosition = GlobalPosition;
//     PayTheCost(cost);
// }
