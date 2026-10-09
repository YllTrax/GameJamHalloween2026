using System;
using Godot;

public partial class SelectBuilding : CanvasLayer
{
    public static SelectBuilding Instance { get; private set; }

    [Export]
    private Godot.Collections.Array<TextureRectOutLine> selectedTower;

    private int lastSelected = 0;
    private int selected;

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

    public override void _Process(double delta)
    {
        ToHighLight();
    }

    private void ToHighLight()
    {
        if (Personnage.Instance == null)
            return;

        int selected = (int)Personnage.Instance.selectTower;

        for (int i = 0; i < selectedTower.Count; i++)
            selectedTower[i].OutlineHandler(i == selected);
    }
}
