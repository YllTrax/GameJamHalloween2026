using Godot;

public partial class Updgrape : CanvasLayer
{
	[Signal] public delegate void GermsChangedEventHandler(int germs);

	private Label _label;
	private int _dernier = -1;

	public int Germs => Personnage.Instance?.germs ?? 0;

	public override void _Ready()
	{
		_label = GetNodeOrNull<Label>("Label");
		ProcessMode = ProcessModeEnum.Always; // continue de s'actualiser si le jeu est en pause
	}

	public override void _Process(double delta)
	{
		int g = Germs;
		if (g == _dernier) return;

		_dernier = g;
		if (_label != null) _label.Text = $"Gems : {g}";
		EmitSignal(SignalName.GermsChanged, g);
	}

	public bool TrySpend(int amount)
	{
		var p = Personnage.Instance;
		if (p == null || p.germs < amount) return false;

		p.germs -= amount;
		return true;
	}
}
