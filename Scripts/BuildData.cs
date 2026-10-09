using Godot;

[GlobalClass]
public partial class BuildData : Resource
{
    [Export]
    public string Name { get; set; }

    [Export]
    public int Cost { get; set; }

    [Export]
    public Texture2D Icon { get; set; }

    [Export]
    public PackedScene Scene { get; set; } // la scène à instancier
}
