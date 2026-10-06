using Godot;

public partial class Pumpkin : CharacterBody2D
{
	[Export] public float Speed = 120f;
	[Export] public float Damage = 5f;
	[Export] public float AttackCooldown = 1f;
	[Export] public float SaltAttackRange = 60f; // distance à partir de laquelle elle tape le cercle

	private NavigationAgent2D _agent;
	private double _cooldown;
	private Node2D _myDoor;
	private bool _doorAssigned;

	public override void _Ready()
	{
		_agent = GetNode<NavigationAgent2D>("NavigationAgent2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Personnage.Instance == null) return;

		_cooldown -= delta;
		bool indoor = Personnage.Instance.iSIndoor;
		Node2D target = Personnage.Instance;
		bool targetingSalt = false;

		if (indoor)
		{
			// On choisit la porte une seule fois
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
				// Sa porte est cassée : direction le cercle de sel
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
			// Le joueur est ressorti : on réinitialise pour la prochaine fois
			_doorAssigned = false;
			_myDoor = null;
		}

		_agent.TargetPosition = target.GlobalPosition;

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

		if (!indoor || _cooldown > 0) return;

		if (targetingSalt)
		{
			AttackSaltCircle();
		}
		else
		{
			AttackDoor();
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
