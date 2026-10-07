using Godot;

public partial class HealthBar : ProgressBar
{
	[Export]
	private Health health;

	[Export]
	public ProgressBar BarreVie;

	[Export]
	public float VieMax;
	private float vie;

	public void OnHealthCHangeHeal(float value)
	{
		vie = Mathf.Min(vie + value, VieMax);
		BarreVie.Value = vie;
	}

	public void OnHealthCHangeDamage(float value)
	{
		vie = Mathf.Max(vie - value, 0);

		BarreVie.Value = vie;
	}
}
