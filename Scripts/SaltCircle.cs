using System;
using Godot;

public partial class SaltCircle : Area2D, IInteractable
{
	[Export]
	private Health health;
	public Health Health => health;

	// public event Action<float> SaltRefilled;

	public static SaltCircle Instance { get; private set; }

	private float saltAmount;

	public override void _EnterTree()
	{
		if (Instance != null && Instance != this)
		{
			QueueFree();
			return;
		}
		Instance = this;
	}

	public override void _Ready()
	{
		InputPickable = true;
	}

	public override void _ExitTree()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	public void ReFill() { }

	public override void _Process(double delta) { }

	public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
	{
		if (@event is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Left)
		{ }
	}

	public void Interact()
	{
		float tempSalt = Personnage.Instance.carriedSalt;
		saltAmount += tempSalt;
		health.ToHeal(tempSalt);
	}

	private float ToPasserLeSel(float montantDeSelRecu)
	{
		return montantDeSelRecu;
	}
}
