using System;
using Godot;

public partial class MenuSetting : CanvasLayer
{
	[Export]
	public string NomBus = "Volume";

	[Export]
	private BaseButton btnRetour; // le bouton "Return"

	// Prévenu quand on ferme les réglages (MenuPause s'y abonne)
	public event Action Ferme;

	private const string Fichier = "user://reglages.cfg";
	private HSlider _slider;
	private int _bus;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always; // reste actif pendant la pause
		Visible = false; // caché au départ

		if (btnRetour != null)
			btnRetour.Pressed += Fermer;
		else
			GD.PushWarning("[MenuSetting] btnRetour non assigné dans l'inspecteur");

		_slider = GetNode<HSlider>("VBoxContainer/HSlider");
		_bus = AudioServer.GetBusIndex(NomBus);
		if (_bus < 0)
		{
			GD.PushError($"[MenuSetting] Bus introuvable : {NomBus}");
			return;
		}

		// Le slider va de 0 (silence) à 1 (volume maximum)
		_slider.MinValue = 0;
		_slider.MaxValue = 1;
		_slider.Step = 0.01;

		// Valeur sauvegardée, sinon volume actuel du bus
		double depart = Charger(Mathf.DbToLinear(AudioServer.GetBusVolumeDb(_bus)));
		_slider.Value = depart;
		AppliquerVolume((float)depart);

		_slider.ValueChanged += v => AppliquerVolume((float)v);
		_slider.DragEnded += _ => Sauvegarder();
	}

	public override void _ExitTree()
	{
		if (_bus >= 0 && _slider != null)
			Sauvegarder();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (Visible && @event.IsActionPressed("ui_cancel"))
		{
			Fermer();
			GetViewport().SetInputAsHandled();
		}
	}

	public void Ouvrir()
	{
		Visible = true;
	}

	private void Fermer()
	{
		if (_bus >= 0 && _slider != null)
			Sauvegarder();
		Visible = false;
		Ferme?.Invoke(); // MenuPause réapparaît
	}

	private void AppliquerVolume(float lineaire)
	{
		// Linéaire (0 à 1) -> décibels. À 0 on coupe le bus.
		bool muet = lineaire <= 0.0001f;
		AudioServer.SetBusMute(_bus, muet);
		if (!muet)
			AudioServer.SetBusVolumeDb(_bus, Mathf.LinearToDb(lineaire));
	}

	private void Sauvegarder()
	{
		var cfg = new ConfigFile();
		cfg.Load(Fichier); // garde les autres réglages déjà présents
		cfg.SetValue("audio", NomBus, (float)_slider.Value);
		cfg.Save(Fichier);
	}

	private double Charger(double parDefaut)
	{
		var cfg = new ConfigFile();
		if (cfg.Load(Fichier) != Error.Ok)
			return parDefaut;
		return cfg.GetValue("audio", NomBus, (float)parDefaut).AsDouble();
	}

	public override void _Input(InputEvent @event)
	{
		if (Visible && @event is InputEventMouseButton mb && mb.Pressed)
			GD.Print(
				$"[MenuSetting] Clic reçu par : {GetViewport().GuiGetHoveredControl()?.GetPath()}"
			);
	}
}
