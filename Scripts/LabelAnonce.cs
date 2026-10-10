using System;
using Godot;

public partial class LabelAnonce : Label
{
    [Export]
    string label;

    public override void _Ready()
    {
        Text = Tr(label);
    }
}
