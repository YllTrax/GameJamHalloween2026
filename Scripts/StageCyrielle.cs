using Godot;
using System;

public partial class StageCyrielle : Node2D
{
	// --- Spawn des citrouilles ---
	[Export] public PackedScene PumpkinScene;
	[Export] public PackedScene PumpkinLv2Scene;  // Lv2
	[Export] public PackedScene PumpkinLv3Scene;  // Lv3
	[Export] public float DureeTotale = 180f;        // durée du spawn en secondes
	[Export] public float IntervalleSpawn = 5f;    // une citrouille toutes les 5 secondes
	[Export] public Rect2 ZoneJeu = new Rect2(0, 0, 1152, 648); // la zone visible
	[Export] public float Marge = 100f;              // distance en dehors de la zone
	// Toutes les X citrouilles Lv1, on crée une Lv2 ; toutes les X Lv2, une Lv3
	[Export] public int SeuilLv2 = 10;
	[Export] public int SeuilLv3 = 10;

	private int _compteurLv1 = 0;
	private int _compteurLv2 = 0;
	private float _tempsEcoule = 0f;
	private float _timerSpawn = 0f;
	private RandomNumberGenerator _rng = new RandomNumberGenerator();

	// --- Game over ---
	private CanvasLayer _gameOverUI;
	private Button _boutonRejouer;
	private bool _fini = false;

	public override void _Ready()
	{
		_rng.Randomize();
		_gameOverUI = GetNode<CanvasLayer>("GameOverUI");
		_boutonRejouer = GetNode<Button>("GameOverUI/Button");
		_boutonRejouer.Pressed += Rejouer;
	}

	public override void _Process(double delta)
	{
		if (_fini) return;

		// Cercle de sel détruit -> défaite
		if (SaltCircle.Instance == null || !IsInstanceValid(SaltCircle.Instance))
		{
			GameOver();
			return;
		}
		// Personnage Mort -> défaite
		if (Personnage.Instance == null || !IsInstanceValid(Personnage.Instance))
		{
			GameOver();
			return;
		}

		GererSpawn((float)delta);
	}

	private void GererSpawn(float delta)
	{
		if (PumpkinScene == null || _tempsEcoule >= DureeTotale) return;

		_tempsEcoule += delta;
		_timerSpawn -= delta;

		if (_timerSpawn <= 0f)
		{
			SpawnPumpkin();
			_timerSpawn = IntervalleSpawn;
		}
	}

	private void SpawnPumpkin()
	{
		PackedScene scene;

		if (_compteurLv2 >= SeuilLv3)
		{
			scene = PumpkinLv3Scene;
			_compteurLv2 = 0;          // on vide
		}
		else if (_compteurLv1 >= SeuilLv2)
		{
			scene = PumpkinLv2Scene;
			_compteurLv1 = 0;          // on vide
			_compteurLv2++;            // une Lv2 de plus vers la Lv3
		}
		else
		{
			scene = PumpkinScene;
			_compteurLv1++;
		}

		if (scene == null) return;

		Node2D pumpkin = scene.Instantiate<Node2D>();
		AddChild(pumpkin);
		pumpkin.GlobalPosition = PositionHorsZone();
	}


	// Choisit un point aléatoire juste à l'extérieur de la zone, sur un des 4 côtés
	private Vector2 PositionHorsZone()
	{
		float gauche = ZoneJeu.Position.X - Marge;
		float droite = ZoneJeu.End.X + Marge;
		float haut = ZoneJeu.Position.Y - Marge;
		float bas = ZoneJeu.End.Y + Marge;

		switch (_rng.RandiRange(0, 3))
		{
			case 0: return new Vector2(gauche, _rng.RandfRange(haut, bas));  // gauche
			case 1: return new Vector2(droite, _rng.RandfRange(haut, bas));  // droite
			case 2: return new Vector2(_rng.RandfRange(gauche, droite), haut); // haut
			default: return new Vector2(_rng.RandfRange(gauche, droite), bas); // bas
		}
	}

	private void GameOver()
	{
		_fini = true;
		_gameOverUI.Visible = true;
		GetTree().Paused = true;
	}

	private void Rejouer()
	{
		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();
	}
}
