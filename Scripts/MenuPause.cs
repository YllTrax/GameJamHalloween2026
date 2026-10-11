using System;
using Godot;

public partial class MenuPause : CanvasLayer
{
	// Scène du menu principal (laisser vide = le bouton quitte le jeu)
	[Export(PropertyHint.File, "*.tscn")]
	public string ScenePrincipale = "";

	// Langues proposées par le bouton "traduction" (codes de locale)
	[Export]
	public string[] Langues = { "fr", "en" };

	[Export]
	public CanvasLayer languesMenu;

	[Export]
	private AudioStreamPlayer2D clic;

	private bool _ouvert = false;
	private MenuSetting _menuSetting;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		Visible = false;

		// MenuSetting est le nœud voisin de MenuPause dans la scène
		_menuSetting = GetNodeOrNull<MenuSetting>("../MenuSetting");
		if (_menuSetting == null)
			GD.PushError("[MenuPause] MenuSetting introuvable (../MenuSetting)");
		else
			_menuSetting.Ferme += OnReglagesFermes;

		Connecter("Play", Reprendre);
		Connecter("BtnQuitter", Quitter);
		Connecter("BtnReglages", OuvrirReglages);
		Connecter("BtnLangue", OuvrirLangues);
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
		if (!@event.IsActionPressed("ui_cancel"))
			return;

		// Les réglages sont ouverts : c'est eux qui gèrent Échap
		if (_menuSetting != null && _menuSetting.Visible)
			return;

		// Le menu langues est ouvert : c'est lui qui gère Échap
		if (languesMenu != null && languesMenu.Visible)
			return;

		if (_ouvert)
			Reprendre();
		else if (!GetTree().Paused)
			MettreEnPause();
		else
			return;

		GetViewport().SetInputAsHandled();
	}

	private void MettreEnPause()
	{
		clic.Play();
		_ouvert = true;
		Visible = true;
		GetTree().Paused = true;
	}

	private void Reprendre()
	{
		clic.Play();
		_ouvert = false;
		Visible = false;
		GetTree().Paused = false;
	}

	private void Quitter()
	{
		clic.Play();
		GetTree().Paused = false; // toujours dépauser avant de changer de scène
		if (!string.IsNullOrEmpty(ScenePrincipale))
			GetTree().ChangeSceneToFile(ScenePrincipale);
		else
			GetTree().Quit();
	}

	private void OuvrirReglages()
	{
		clic.Play();
		if (_menuSetting == null)
			return;

		Visible = false; // on cache le menu pause (le jeu reste en pause)
		_menuSetting.Ouvrir();
	}

	private void OnReglagesFermes()
	{
		clic.Play();
		if (_ouvert)
			Visible = true; // retour au menu pause
	}


	private void OuvrirLangues()
	{
		if (languesMenu == null)
		{
			GD.PushError("[MenuPause] languesMenu non assigné dans l'inspecteur");
			return;
		}
		clic?.Play();
		Visible = false; // on cache le menu pause (le jeu reste en pause)
		languesMenu.Visible = true;
	}
}
