using Godot;
using System;

public partial class MenuPause : CanvasLayer
{
	// Scène du menu principal (laisser vide = le bouton quitte le jeu)
	[Export(PropertyHint.File, "*.tscn")] 
	public string ScenePrincipale = "";

	// Langues proposées par le bouton "traduction" (codes de locale) avoir si on peut
	[Export] public string[] Langues = { "fr", "en" };

	private bool _ouvert = false;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		Visible = false;

		Connecter("Play", Reprendre);
		Connecter("BtnQuitter", Quitter);
		Connecter("BtnReglages", OuvrirReglages);
		Connecter("BtnLangue", ChangerLangue);
	}

	private void Connecter(string nom, Action action)
	{
		var bouton = GetNodeOrNull<Godot.TextureButton>(nom);
		if (bouton == null)
		{
			GD.PushError($"[MenuPause] Bouton introuvable : {nom}");
			return;
		}

		bouton.Pressed += action;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (!@event.IsActionPressed("ui_cancel")) return; // Échap par défaut
		GD.Print("[MenuPause] Échap détecté");
		if (_ouvert)
			Reprendre();
		else if (!GetTree().Paused) // évite d'ouvrir le menu sur l'écran Game Over (déjà en pause)
			MettreEnPause();
		else
			return;

		GetViewport().SetInputAsHandled();
	}

	private void MettreEnPause()
	{
		_ouvert = true;
		Visible = true;
		GetTree().Paused = true;
	}

	private void Reprendre()
	{
		GD.Print("Play cliqué");
		_ouvert = false;
		Visible = false;
		GetTree().Paused = false;
	}

	private void Quitter()
	{
		GetTree().Paused = false; // toujours dépauser avant de changer de scène
		if (!string.IsNullOrEmpty(ScenePrincipale))
			GetTree().ChangeSceneToFile(ScenePrincipale);
		else
			GetTree().Quit();
	}

	private void OuvrirReglages()
	{
		GD.Print("Réglages : à brancher"); // TODO : ouvrir votre scène de réglages
	}

	private void ChangerLangue()
	{
		string actuelle = TranslationServer.GetLocale();
		int i = Array.FindIndex(Langues, l => actuelle.StartsWith(l));
		TranslationServer.SetLocale(Langues[(i + 1) % Langues.Length]);
	}

	public override void _Input(InputEvent @event)
	{
		if (Visible && @event is InputEventMouseButton mb && mb.Pressed)
			GD.Print($"[MenuPause] Clic reçu par : {GetViewport().GuiGetHoveredControl()?.GetPath()}");
	}

	private void MasqueDeClic(string nom)
	{
		var bouton = GetNode<TextureButton>(nom);
		var image = bouton.TextureNormal?.GetImage();
		if (image == null)
			return;

		var masque = ClassDB.Instantiate("BitMap").AsGodotObject();
		if (masque == null)
		{
			GD.Print("[MenuPause] BitMap indisponible dans cette version");
			return;
		}

		masque.Call("create_from_image_alpha", image);
		bouton.Set("texture_click_mask", masque);
	}
}
