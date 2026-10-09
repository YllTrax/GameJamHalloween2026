using System;
using Godot;

public partial class Bullet : Area2D
{
	private Node2D target;

	[Export]
	public float moveSpeed;

	[Export]
	public PackedScene munition;

	[Export]
	private DamageSource damageSource;
	public float Damage
	{
		get => damageSource.Damage;
		set => damageSource.Damage = value;
	}

	[Export]
	private float lifeTime = 2;
	private Timer lifeTimer;
	public Vector2 dir;

	public override void _Ready()
	{
		lifeTimer = new Timer();
		lifeTimer.WaitTime = lifeTime;
		lifeTimer.Timeout += SelfDestroy;
		AddChild(lifeTimer);
		lifeTimer.Start();
	}

	private void SelfDestroy()
	{
		if (munition != null)
		{
			Vector2 pos = GlobalPosition;
			var ammo = munition.Instantiate<Munition>();
			ammo.Position = pos;
			ammo.SetDrop(
				pos + Vector2.FromAngle(GD.Randf() * Mathf.Tau) * (40f + GD.Randf() * 60f)
			);

			GetTree().CurrentScene.CallDeferred(Node.MethodName.AddChild, ammo);
		}
		QueueFree();
	}

	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition += dir * moveSpeed * (float)delta;
	}
}
