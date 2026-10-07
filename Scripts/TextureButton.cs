using Godot;

public partial class TextureButton : Godot.TextureButton
{
	[Signal] public delegate void BoughtEventHandler();

	[Export] public int Cost = 10;
	[Export] public TextureButton Prerequisite;

	private Label _skillLevel;
	private Updgrape _tree;

	public bool Purchased { get; private set; } = false;
	private bool Unlocked => Prerequisite == null || Prerequisite.Purchased;

	public override void _Ready()
	{
		_skillLevel = GetNode<Label>("Cost");
		_tree = GetTree().CurrentScene as Updgrape;

		Pressed += OnPressed;
		_tree.GermsChanged += _ => UpdateVisuals();
		if (Prerequisite != null)
			Prerequisite.Bought += UpdateVisuals;

		UpdateVisuals();
	}

	private void OnPressed()
	{
		if (Purchased || !Unlocked) return;
		if (!_tree.TrySpend(Cost)) return;

		Purchased = true;
		EmitSignal(SignalName.Bought);
		UpdateVisuals();
	}

	private void UpdateVisuals()
	{
		_skillLevel.Text = Purchased ? "Acquis" : $"{Cost} germes";

		bool canAfford = _tree.Germs >= Cost;
		if (Purchased)
			Modulate = Colors.White;
		else if (!Unlocked)
			Modulate = new Color(0.3f, 0.3f, 0.3f, 0.5f);
		else if (!canAfford)
			Modulate = new Color(1, 0.5f, 0.5f, 0.6f);
		else
			Modulate = new Color(1, 1, 1, 0.8f);

		Disabled = Purchased;
	}
}
