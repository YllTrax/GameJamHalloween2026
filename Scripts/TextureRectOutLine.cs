using System;
using Godot;

public partial class TextureRectOutLine : TextureRect
{
    [Export]
    public TextureRect outline;

    public void OutlineHandler(bool _bool)
    {
        outline.Visible = _bool;
    }
}
