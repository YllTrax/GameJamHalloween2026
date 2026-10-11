using Godot;

public partial class Trailer : Control
{
	[Export(PropertyHint.File, "*.tscn")]
	public string ScenePrincipale = "res://Main.tscn"; // ta scène avec BoucleDeJeu

	private VideoStreamPlayer _video;
	private bool _termine = false;

	public override void _Ready()
	{
		_video = GetNode<VideoStreamPlayer>("VideoStreamPlayer");
		_video.Finished += LancerJeu;
		_video.Play();
	}

	public override void _Input(InputEvent @event)
	{
		// Passer le trailer : Echap, Entrée, Espace ou clic
		if (@event.IsActionPressed("ui_cancel")
			|| @event.IsActionPressed("ui_accept")
			|| (@event is InputEventMouseButton mb && mb.Pressed))
		{
			LancerJeu();
		}
	}

	private void LancerJeu()
	{
		if (_termine)
			return; // évite un double appel (Finished + touche)

		_termine = true;
		GetTree().ChangeSceneToFile(ScenePrincipale);
	}
}
