using Godot;
using System;

public partial class MenuPause : CanvasLayer
{
	// Scène du menu principal (laisser vide = le bouton quitte le jeu)
	[Export(PropertyHint.File, "*.tscn")] public string ScenePrincipale = "";

	// Langues proposées par le bouton "traduction" (codes de locale)
	[Export] public string[] Langues = { "fr", "en" };

	private bool _ouvert = false;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always; // le menu doit rester actif pendant la pause
		Visible = false;

		GetNode<TextureButton>("Play").Pressed += Reprendre;
		GetNode<TextureButton>("BtnQuitter").Pressed += Quitter;
		GetNode<TextureButton>("BtnReglages").Pressed += OuvrirReglages;
		GetNode<TextureButton>("BtnLangue").Pressed += ChangerLangue;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (!@event.IsActionPressed("ui_cancel")) return; // Échap par défaut

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
}
