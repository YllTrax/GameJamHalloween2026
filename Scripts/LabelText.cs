using System;
using Godot;

public partial class LabelText : Button
{
    [Export]
    string label;

    public override void _Ready()
    {
        Text = Tr(label);
    }
}
