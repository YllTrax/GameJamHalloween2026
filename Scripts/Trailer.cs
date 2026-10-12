using Godot;

public partial class Trailer : Control
{
	[Export(PropertyHint.File, "*.tscn")]
	public string ScenePrincipale = "res://Main.tscn";

	private VideoStreamPlayer _video;
	private Control _tutoFr;
	private Control _tutoEng;
	private Button _boutonLangue;
	private Button _boutonJouer;

	private bool _enTuto = false;
	private bool _termine = false;
	private bool _francais = true;

	public override void _Ready()
	{
		_video = GetNode<VideoStreamPlayer>("VideoStreamPlayer");
		_tutoFr = GetNode<Control>("tuto_fr");
		_tutoEng = GetNode<Control>("tuto_eng");
		_boutonLangue = GetNode<Button>("BoutonLangue");
		_boutonJouer = GetNode<Button>("BoutonJouer");

		// Au départ : seulement la vidéo
		_tutoFr.Visible = false;
		_tutoEng.Visible = false;
		_boutonLangue.Visible = false;
		_boutonJouer.Visible = false;

		_video.Finished += AfficherTuto;
		_boutonLangue.Pressed += ChangerLangue;
		_boutonJouer.Pressed += LancerJeu;

		_video.Play();
	}

	public override void _Input(InputEvent @event)
	{
		if (!_enTuto)
		{
			// Pendant le trailer : passer avec Echap, Entrée, Espace ou clic
			if (
				@event.IsActionPressed("ui_cancel")
				|| @event.IsActionPressed("ui_accept")
				|| (@event is InputEventMouseButton mb && mb.Pressed)
			)
			{
				AfficherTuto();
			}
		}
		else
		{
			// Pendant le tuto : Entrée / Espace lance le jeu
			// (les clics souris sont gérés par les boutons)
			if (@event.IsActionPressed("ui_accept"))
			{
				LancerJeu();
			}
		}
	}

	private void AfficherTuto()
	{
		if (_enTuto)
			return;

		_enTuto = true;
		_video.Stop();
		_video.Visible = false;

		_boutonLangue.Visible = true;
		_boutonJouer.Visible = true;
		MettreAJourLangue();
	}

	private void ChangerLangue()
	{
		_francais = !_francais;
		MettreAJourLangue();
	}

	private void MettreAJourLangue()
	{
		_tutoFr.Visible = _francais;
		_tutoEng.Visible = !_francais;

		// Le bouton propose l'autre langue
		_boutonLangue.Text = _francais ? "English" : "Français";
		_boutonJouer.Text = _francais ? "Jouer" : "Play";
	}

	private void LancerJeu()
	{
		if (_termine)
			return;

		_termine = true;
		GetTree().ChangeSceneToFile(ScenePrincipale);
	}
}
