using Godot;

public partial class Pumpkin : CharacterBody2D
{
	[Export] public float Speed = 120f;
	[Export] public float Damage = 5f;
	[Export] public float AttackCooldown = 1f;

	private NavigationAgent2D _agent;
	private double _cooldown;

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
		if (indoor)
		{
			Node2D door = GetNearestDoor();
			if (door == null)
			{
				Velocity = Vector2.Zero; // plus de porte : elle ne bouge plus
				return;
			}
			target = door;
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

		// Attaque : seulement la porte qu'elle touche, si le joueur est dedans
		if (!indoor || _cooldown > 0) return;

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
