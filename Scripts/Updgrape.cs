using Godot;

public partial class Updgrape : Node2D
{
	[Signal] public delegate void GermsChangedEventHandler(int germs);

	[Export] public int StartGerms = 50;
	private Label _label;

	private int _germs;
	public int Germs
	{
		get => _germs;
		private set
		{
			_germs = Mathf.Max(0, value);
			if (_label != null) _label.Text = $"Germes : {_germs}";
			EmitSignal(SignalName.GermsChanged, _germs);
		}
	}

	public override void _Ready()
	{
		_label = GetNodeOrNull<Label>("CanvasLayer/Label");
		Germs = StartGerms;
	}

	public bool TrySpend(int amount)
	{
		if (Germs < amount) return false;
		Germs -= amount;
		return true;
	}
}
