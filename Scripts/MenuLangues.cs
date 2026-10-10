using Godot;

public partial class MenuLangues : CanvasLayer
{
	[Export]
	private BaseButton btnRetour;

	[Export]
	private CanvasLayer menuPause;

	private static readonly Color CouleurNormale = Colors.White;
	private static readonly Color CouleurGrise = new Color(0.5f, 0.5f, 0.5f, 1f);

	private Button btnFR;
	private Button btnEN;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		Visible = false;

		if (btnRetour != null)
			btnRetour.Pressed += Retour;
		
		btnFR = GetNode<Button>("VBoxContainer/FR");
		btnEN = GetNode<Button>("VBoxContainer/EN");

		MettreAJourBoutons();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (!Visible)
			return;

		if (@event.IsActionPressed("ui_cancel"))
		{
			Retour();
			GetViewport().SetInputAsHandled();
		}
	}

	public void ChoisirLangue(string locale)
	{
		TranslationServer.SetLocale(locale);
		MettreAJourBoutons();
	}

	private void MettreAJourBoutons()
	{
		bool estFrancais = TranslationServer.GetLocale().StartsWith("fr");

		btnFR.Modulate = estFrancais ? CouleurNormale : CouleurGrise;
		btnEN.Modulate = estFrancais ? CouleurGrise : CouleurNormale;
	}

	private void Retour()
	{
		Visible = false;
		if (menuPause != null)
			menuPause.Visible = true;
	}
}
