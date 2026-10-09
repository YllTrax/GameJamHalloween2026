using System;
using Godot;

public partial class BoucleDeJeu : Node2D
{
	// --- Spawn des citrouilles ---
	[Export]
	public PackedScene PumpkinScene;

	[Export]
	public PackedScene PumpkinLv2Scene; // Lv2

	[Export]
	public PackedScene PumpkinLv3Scene; // Lv3

	[Export]
	public float IntervalleSpawn = 5f; // intervalle de base (vague 1)

	[Export]
	public Rect2 ZoneJeu = new Rect2(0, 0, 1152, 648); // la zone visible

	[Export]
	public float Marge = 100f; // distance en dehors de la zone

	// Toutes les X citrouilles Lv1, on crée une Lv2 ; toutes les X Lv2, une Lv3
	[Export]
	public int SeuilLv2 = 10;

	[Export]
	public int SeuilLv3 = 10;

	// --- Vagues ---
	[Export]
	public float DureeVague = 180f; // secondes par vague

	[Export]
	public float FacteurIntervalle = 0.9f; // chaque vague : intervalle x 0.9

	[Export]
	public float IntervalleMin = 0.8f; // plancher

	[Export]
	public float BonusVitesseParVague = 0.05f; // +5% de vitesse par vague

	[Export]
	public Label WaveLabel; // Label d'affichage (assigné dans l'inspecteur)

	//Nombre de Gems
	[Export]
	public Label GemsLabel;

	[Export]
	public Label SaltLabel;

	// --- Upgrade ---
	[Export]
	public float DistanceInteraction = 60f; // distance max au cercle de sel pour ouvrir le menu

	//GameOver
	[Export]
	public Label ScoreLabel;

	public int VagueActuelle { get; private set; } = 1;

	private int _compteurLv1 = 0;
	private int _compteurLv2 = 0;
	private float _timerVague = 0f;
	private float _timerSpawn = 0f;
	private RandomNumberGenerator _rng = new RandomNumberGenerator();

	// --- Game over ---
	private CanvasLayer _gameOverUI;
	private Button _boutonRejouer;
	private bool _fini = false;

	// --- Gems ---
	private int _germsAffiches = -1;
	private float _saltAffiches = -1;

	// --- Upgrade ---
	private CanvasLayer _upgradeUI;
	private CanvasLayer _hudWaves;
	private CanvasLayer _hudGems;

	//--heeeu je sais pas il est tard  mais voila ---------
	[Export]
	Godot.Collections.Array<SaltDeposit> depots;

	[Export]
	private Label refillLabelAnounce;

	public override void _Ready()
	{
		//  depots = FindChildren("SaltDeposits", "", true, false);

		_timerFill = new Timer { WaitTime = refillCD, OneShot = false };
		_timerFill.Timeout += DepositRandomHandler;
		AddChild(_timerFill);
		_timerFill.Start();

		_rng.Randomize();
		_timerVague = DureeVague;
		_gameOverUI = GetNode<CanvasLayer>("GameOver");
		_boutonRejouer = GetNode<Button>("GameOver/Button");
		_boutonRejouer.Pressed += Rejouer;
		_upgradeUI = GetNodeOrNull<CanvasLayer>("Updgrape");
		_hudWaves = GetNodeOrNull<CanvasLayer>("Waves");
		_hudGems = GetNodeOrNull<CanvasLayer>("Gems");
		MettreAJourLabel();
	}

	public override void _Process(double delta)
	{
		if (_fini)
			return;

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

		MettreAJourGerms();
		MettreAJourSalt();
		GererUpgrade();

		GererVagues((float)delta);
		GererSpawn((float)delta);
	}

	private void GererVagues(float delta)
	{
		_timerVague -= delta;
		if (_timerVague <= 0f)
		{
			VagueActuelle++;
			_timerVague = DureeVague;
			GD.Print($"Waves {VagueActuelle} ! Intervalle : {IntervalleActuel():0.00}s");
		}
		MettreAJourLabel();
	}

	// Intervalle qui diminue à chaque vague, jamais sous IntervalleMin
	private float IntervalleActuel()
	{
		float intervalle = IntervalleSpawn * Mathf.Pow(FacteurIntervalle, VagueActuelle - 1);
		return Mathf.Max(intervalle, IntervalleMin);
	}

	private void MettreAJourLabel()
	{
		if (WaveLabel == null)
			return;

		WaveLabel.Text = $"Waves  {VagueActuelle}";
	}

	private void GererSpawn(float delta)
	{
		if (PumpkinScene == null)
			return;

		_timerSpawn -= delta;
		if (_timerSpawn <= 0f)
		{
			SpawnPumpkin();
			_timerSpawn = IntervalleActuel();
		}
	}

	private void SpawnPumpkin()
	{
		PackedScene scene;

		if (_compteurLv2 >= SeuilLv3)
		{
			scene = PumpkinLv3Scene;
			_compteurLv2 = 0; // on vide
		}
		else if (_compteurLv1 >= SeuilLv2)
		{
			scene = PumpkinLv2Scene;
			_compteurLv1 = 0; // on vide
			_compteurLv2++; // une Lv2 de plus vers la Lv3
		}
		else
		{
			scene = PumpkinScene;
			_compteurLv1++;
		}

		if (scene == null)
			return;

		Node2D pumpkin = scene.Instantiate<Node2D>();

		// Plus rapide à chaque vague
		if (pumpkin is Pumpkin p)
			p.Speed *= 1f + BonusVitesseParVague * (VagueActuelle - 1);

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
			case 0:
				return new Vector2(gauche, _rng.RandfRange(haut, bas)); // gauche
			case 1:
				return new Vector2(droite, _rng.RandfRange(haut, bas)); // droite
			case 2:
				return new Vector2(_rng.RandfRange(gauche, droite), haut); // haut
			default:
				return new Vector2(_rng.RandfRange(gauche, droite), bas); // bas
		}
	}

	//Mettre Le gems a jour
	private void MettreAJourGerms()
	{
		if (GemsLabel == null)
			return;

		int g = Personnage.Instance.germs;
		if (g == _germsAffiches)
			return;

		_germsAffiches = g;
		GemsLabel.Text = $"Gems : {g}";
	}

	private void MettreAJourSalt()
	{
		if (SaltLabel == null)
			return;

		float g = Personnage.Instance.carriedSalt;
		if (g == _saltAffiches)
			return;

		_saltAffiches = g;
		SaltLabel.Text = $"Salt : {g}";
	}

	//Fenetre update
	private void GererUpgrade()
	{
		if (_upgradeUI == null)
			return;

		float distance = Personnage.Instance.GlobalPosition.DistanceTo(
			SaltCircle.Instance.GlobalPosition
		);
		bool proche = distance <= DistanceInteraction;

		// Le joueur s'éloigne du cercle : on ferme le shop
		if (_upgradeUI.Visible && !proche)
		{
			AfficherUpgrade(false);
			return;
		}

		// E près du cercle : ouvre / ferme
		if (proche && Input.IsActionJustPressed("Upgrade"))
			AfficherUpgrade(!_upgradeUI.Visible);
	}

	private void AfficherUpgrade(bool afficher)
	{
		if (afficher)
			UpgradeStats.PrintAll();
		_upgradeUI.Visible = afficher;
		if (_hudWaves != null)
			_hudWaves.Visible = !afficher;
		if (_hudGems != null)
			_hudGems.Visible = !afficher;
	}

	// Boucle de jeu
	private void GameOver()
	{
		ScoreLabel.Text = $"Score : {VagueActuelle}";
		_fini = true;
		_gameOverUI.Visible = true;
		GetTree().Paused = true;
		UpgradeStats.Reset();
	}

	private void Rejouer()
	{
		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();
		
	}

	private void DepositRandomHandler()
	{
		depots[(int)GD.RandRange(0, depots.Count - 1)].Refill();
		TweenTraversee(refillLabelAnounce);
	}

	//---------------------------------------------------la partie tween ----------------------------------------------------------
	private Tween _bannerTween; // je met en champs ici je susi sous anxio je relfechis doucement XD

	[Export]
	private double refillCD = 20.0;
	private Timer _timerFill;

	/// <summary>
	/// Fait traverser l'écran à un Label de gauche à droite :
	/// rapide à l'entrée, lent au centre, rapide à la sortie.
	/// </summary>
	private void TweenTraversee(
		Label label,
		float dureeEntree = 0.6f,
		float pauseCentre = 0.8f,
		float dureeSortie = 0.6f
	)
	{
		_bannerTween?.Kill();

		// Calcule la taille réelle du texte (sinon Size peut être faux juste après un changement de texte)
		label.Size = label.GetCombinedMinimumSize();

		Vector2 ecran = GetViewportRect().Size;
		float y = (ecran.Y - label.Size.Y) / 2f; // centré verticalement

		Vector2 depart = new Vector2(-label.Size.X, y); // hors écran à gauche
		Vector2 centre = new Vector2((ecran.X - label.Size.X) / 2f, y); // milieu
		Vector2 arrivee = new Vector2(ecran.X, y); // hors écran à droite

		label.Position = depart;
		label.Visible = true;

		_bannerTween = CreateTween();

		// Entrée : arrive vite, ralentit en approchant du centre
		_bannerTween
			.TweenProperty(label, "position", centre, dureeEntree)
			.SetTrans(Tween.TransitionType.Expo)
			.SetEase(Tween.EaseType.Out);

		// Pause au centre pour laisser lire
		_bannerTween.TweenInterval(pauseCentre);

		// Sortie : démarre doucement puis accélère
		_bannerTween
			.TweenProperty(label, "position", arrivee, dureeSortie)
			.SetTrans(Tween.TransitionType.Expo)
			.SetEase(Tween.EaseType.In);

		_bannerTween.TweenCallback(Callable.From(() => label.Visible = false));
	}
}
