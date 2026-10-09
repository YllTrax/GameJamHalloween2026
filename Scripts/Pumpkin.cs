using System.ComponentModel;
using Godot;

public partial class Pumpkin : CharacterBody2D
{
	[Export]
	public float Speed = 120f;

	[Export]
	public float Damage = 5f;

	[Export]
	public float AttackCooldown = 1f;

	[Export]
	public float SaltAttackRange = 50f; // distance pour taper le cercle de sel

	[Export]
	public float PlayerAttackRange = 50f; // distance pour taper le personnage

	[Export]
	public float AggroRadius = 300f; // rayon autour du joueur qui attire les citrouilles

	[Export]
	public float PathUpdateInterval = 1f; // secondes entre deux recalculs du chemin

	// Pour les Germs Drop
	[Export]
	public float GermsDrop = 2;

	[Export]
	public PackedScene GermsScene;

	//-------Animation-----
	[Export]
	private AnimatedSprite2D anim;

	//----------navigation

	private NavigationAgent2D _agent;
	private double _cooldown;
	private double _pathTimer;
	private Node2D _myDoor;
	private bool _doorAssigned;

	// Germs
	private Health _health;
	private bool _dead;
	private Timer _timerFire;

	public override void _Ready()
	{
		_agent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		_pathTimer = GD.Randf() * PathUpdateInterval; // décale les recalculs entre citrouilles

		// Germs
		_health = GetNodeOrNull<Health>("Health");
		if (_health != null)
			_health.Died += Die;

		_timerFire = new Timer { WaitTime = 1.1f, OneShot = false };
		_timerFire.Timeout += ToUpAndDown;
		AddChild(_timerFire);
		_timerFire.Start();
		if (IsInstanceValid(anim))
			anim.Play("IdleSouth");
	}

	public override void _PhysicsProcess(double delta)
	{
		// Mort
		if (_dead)
			return;
		if (_health != null && _health.currentHealth <= 0)
		{
			//  Die();
			return;
		}

		if (Personnage.Instance == null)
			return;

		_cooldown -= delta;
		_pathTimer -= delta;

		bool indoor = Personnage.Instance.iSIndoor;
		Node2D target = Personnage.Instance;
		bool targetingSalt = false;

		if (indoor)
		{
			if (!_doorAssigned)
			{
				_myDoor = GetNearestDoor();
				_doorAssigned = true;
			}

			bool doorAlive =
				_myDoor != null && IsInstanceValid(_myDoor) && !_myDoor.IsQueuedForDeletion();

			if (doorAlive)
			{
				target = _myDoor;
			}
			else if (SaltCircle.Instance != null && IsInstanceValid(SaltCircle.Instance))
			{
				target = SaltCircle.Instance;
				targetingSalt = true;
			}
			else
			{
				Velocity = Vector2.Zero;
				return;
			}
		}
		else
		{
			_doorAssigned = false;
			_myDoor = null;
		}

		// Recalcul du chemin seulement toutes les PathUpdateInterval secondes
		if (_pathTimer <= 0)
		{
			_agent.TargetPosition = target.GlobalPosition;
			_pathTimer = PathUpdateInterval;
		}

		if (_agent.IsNavigationFinished())
		{
			Velocity = Vector2.Zero;
		}
		else
		{
			Vector2 nextPoint = _agent.GetNextPathPosition();
			Velocity = (nextPoint - GlobalPosition).Normalized() * Speed;
		}

		MoveAndSlide();

		if (_cooldown > 0)
			return;

		if (!indoor)
		{
			AttackPlayer();
		}
		else if (targetingSalt)
		{
			AttackSaltCircle();
		}
		else
		{
			AttackDoor();
		}
	}

	private void AttackPlayer()
	{
		var joueur = Personnage.Instance;
		if (joueur == null || !IsInstanceValid(joueur))
			return;

		if (GlobalPosition.DistanceTo(joueur.GlobalPosition) <= PlayerAttackRange)
		{
			var health = joueur.GetNodeOrNull<Health>("Health");
			if (health == null)
			{
				GD.Print("Le personnage n'a pas de noeud Health !");
				return;
			}

			health.TakeDamage(Damage);
			GD.Print($"Personnage : {health.currentHealth} PV");
			_cooldown = AttackCooldown;
		}
	}

	private void AttackDoor()
	{
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			if (GetSlideCollision(i).GetCollider() is Node2D hit && hit.IsInGroup("Doors"))
			{
				var health = hit.GetNode<Health>("Health");
				health.TakeDamage(Damage);
				GD.Print($"{hit.Name} : {health.currentHealth} PV");
				_cooldown = AttackCooldown;
				break;
			}
		}
	}

	private void AttackSaltCircle()
	{
		var circle = SaltCircle.Instance;
		if (circle == null || circle.Health == null)
			return;

		if (GlobalPosition.DistanceTo(circle.GlobalPosition) <= SaltAttackRange)
		{
			circle.Health.TakeDamage(Damage);
			GD.Print($"Cercle de sel : {circle.Health.currentHealth} PV");
			_cooldown = AttackCooldown;
		}
	}

	private Node2D GetNearestDoor()
	{
		Node2D nearest = null;
		float best = float.MaxValue;

		foreach (Node n in GetTree().GetNodesInGroup("Doors"))
		{
			if (n is not Node2D door || !IsInstanceValid(door) || door.IsQueuedForDeletion())
				continue;

			float d = GlobalPosition.DistanceSquaredTo(door.GlobalPosition);
			if (d < best)
			{
				best = d;
				nearest = door;
			}
		}
		return nearest;
	}

	// Germs
	public void Die()
	{
		GD.Print($"[Pumpkin] Die | scene={(GermsScene != null)} | drop={GermsDrop}");
		if (_dead)
			return;
		_dead = true;

		if (GermsScene == null)
			return;

		Node parent = GetParent();
		for (int i = 0; i < GermsDrop; i++)
		{
			var germ = GermsScene.Instantiate<Germs>();
			germ.Position = Position; // part de la citrouille
			germ.SetDrop(
				Position + Vector2.FromAngle(GD.Randf() * Mathf.Tau) * (40f + GD.Randf() * 60f)
			);
			parent.CallDeferred(Node.MethodName.AddChild, germ);
		}
	}

	private void ToUpAndDown()
	{
		Vector2 from = GlobalPosition;
		Vector2 to = new Vector2(GlobalPosition.X, GlobalPosition.Y - 5);

		float duration = from.DistanceTo(to) / Speed;

		var tween = CreateTween();
		tween.SetProcessMode(Tween.TweenProcessMode.Physics);

		tween.TweenMethod(
			Callable.From<float>(t =>
			{
				Vector2 pos = from.Lerp(to, t);
				pos.Y -= Mathf.Sin(t * Mathf.Pi); //* arcHeight;
				GlobalPosition = pos;
			}),
			0f,
			1f,
			duration
		);
	}
}
