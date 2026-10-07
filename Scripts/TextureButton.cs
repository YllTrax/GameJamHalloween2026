using Godot;

public partial class TextureButton : Godot.TextureButton
{
	[Export] public int Cost = 10;                      // germes pour acheter
	[Export] public int TestGerms = 25;                 // germes de test
	[Export] public TextureButton Prerequisite;         // bouton parent (facultatif)

	private Label _skillLevel;
	private Line2D _skillBranch;

	public bool Purchased { get; private set; } = false;

	private bool Unlocked => Prerequisite == null || Prerequisite.Purchased;
	private bool CanBuy => !Purchased && Unlocked && TestGerms >= Cost;

	public override void _Ready()
	{
		_skillLevel = GetNode<Label>("Cost");
		_skillBranch = GetNode<Line2D>("Line2D");

		Pressed += OnPressed;
		UpdateVisuals();
	}

	private void OnPressed()
	{
		if (!CanBuy) return;

		TestGerms -= Cost;
		Purchased = true;
		UpdateVisuals();
		GD.Print($"Acheté ! Germes restants : {TestGerms}");
	}

	private void UpdateVisuals()
	{
		if (Purchased)
			_skillLevel.Text = "Acquis";
		else
			_skillLevel.Text = $"{Cost} germes";

		// Opaque si acquis, grisé sinon
		Modulate = Purchased ? Colors.White : new Color(1, 1, 1, 0.5f);

		// Désactive le clic si déjà acheté
		Disabled = Purchased;
	}
}
