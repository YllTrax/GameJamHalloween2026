using Godot;
using System;


public partial class DamageSource : Area2D
{
	[Export] private float damage = 100;
	public override void _Ready()
	{
		BodyEntered += ToDamagePlayer;
	}

	private void ToDamagePlayer(Node2D body)
	{
		if (body is Personnage && body.GetNode<Health>("Health")is Health health)
		{
			health.TakeDamage(damage);
		}

	}
}
