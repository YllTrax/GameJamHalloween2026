using Godot;

public partial class Pumpkin : CharacterBody2D
{
	[Export] public float Speed = 120f;
	[Export] public float Damage = 5f;
	[Export] public float AttackCooldown = 1f;
	[Export] public float SaltAttackRange = 50f;   // distance pour taper le cercle de sel
	[Export] public float PlayerAttackRange = 50f; // distance pour taper le personnage
	[Export] public float AggroRadius = 300f;      // rayon autour du joueur qui attire les citrouilles
	[Export] public float PathUpdateInterval = 1f; // secondes entre deux recalculs du chemin

	private NavigationAgent2D _agent;
	private double _cooldown;
	private double _pathTimer;
	private Node2D _myDoor;
	private bool _doorAssigned;
	private bool _wasAggro;

	public override void _Ready()
	{
		_agent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		_pathTimer = GD.Randf() * PathUpdateInterval;
	}

	public override void _PhysicsProcess(double delta)
	{
		_cooldown -= delta;
		_pathTimer -= delta;

		// Le joueur n'est attaquable que s'il est dehors ET dans le rayon
		var joueur = Personnage.Instance;
		bool aggro = joueur != null
			&& IsInstanceValid(joueur)
			&& !joueur.iSIndoor
			&& GlobalPosition.DistanceTo(joueur.GlobalPosition) <= AggroRadius;
			
		Node2D target;
		bool targetingSalt = false;

		if (aggro)
		{
			target = joueur;
			// Elle rechoisira la porte la plus proche quand elle sortira du rayon
			_doorAssigned = false;
			_myDoor = null;
		}
		else
		{
			if (!_doorAssigned)
			{
				_myDoor = GetNearestDoor();
				_doorAssigned = true;
			}

			bool doorAlive = _myDoor != null && IsInstanceValid(_myDoor) && !_myDoor.IsQueuedForDeletion();

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

		// Si on change de mode (joueur <-> porte/sel), on recalcule tout de suite
		if (aggro != _wasAggro)
		{
			_pathTimer = 0;
			_wasAggro = aggro;
		}

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

		if (_cooldown > 0) return;

		if (aggro)
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
		if (joueur == null || !IsInstanceValid(joueur)) return;

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
		if (circle == null || circle.Health == null) return;

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
}
