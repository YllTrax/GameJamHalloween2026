using Godot;
using System.Globalization;
using System.Text.RegularExpressions;

public partial class TextureButton : Godot.TextureButton
{
	[Signal] public delegate void BoughtEventHandler();

	[Export] public int Cost = 10;
	[Export] public TextureButton Prerequisite;
	[Export(PropertyHint.MultilineText)] public string Description = "";

	private Label _skillLevel;
	private Updgrape _tree;

	public bool Purchased { get; private set; } = false;
	private bool Unlocked => Prerequisite == null || Prerequisite.Purchased;

	public override void _Ready()
	{
		_skillLevel = GetNode<Label>("Cost");
		MettreAJourTooltip();
		_tree = TrouverUpgrade();

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
		AppliquerEffet();
		EmitSignal(SignalName.Bought);
		UpdateVisuals();
	}

	private void UpdateVisuals()
	{
		_skillLevel.Text = Purchased ? Tr("SHOP_ACQUIRED") : string.Format(Tr("SHOP_PRICE"), Cost);

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

	// Tr(Description) renvoie la Description telle quelle si ce n'est pas une clé
	private void MettreAJourTooltip()
	{
		string cout = string.Format(Tr("SHOP_COST"), Cost);
		TooltipText = string.IsNullOrEmpty(Description)
		? cout
		: $"{Tr(Description)}\n{cout}";
	}

	public override void _Notification(int what)
	{
		if (what != NotificationTranslationChanged || !IsNodeReady())
			return;

		MettreAJourTooltip();
		UpdateVisuals();
	}

	private Updgrape TrouverUpgrade()
	{
		Node n = GetParent();
		while (n != null && n is not Updgrape)
			n = n.GetParent();
		return n as Updgrape;
	}
	private void AppliquerEffet()
	{
		var m = Regex.Match(Description, @"\d+(\.\d+)?");
		if (!m.Success) return;

		float bonus = float.Parse(m.Value, CultureInfo.InvariantCulture) / 100f;
		string nom = Name.ToString().ToLower();

		if (nom.StartsWith("salt")) UpgradeStats.SaltBonus = bonus;
		else if (nom.StartsWith("door"))
		{
			UpgradeStats.DoorBonus = bonus;
			foreach (Node porte in GetTree().GetNodesInGroup("Doors"))
				UpgradeStats.ApplyDoorBonus(porte);
		}
		else if (nom.StartsWith("tourelledeg")) UpgradeStats.TurretDamageBonus = bonus;
		else if (nom.StartsWith("tourellemun")) UpgradeStats.TurretAmmoBonus = bonus;
	}
}
