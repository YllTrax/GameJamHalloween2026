using Godot;
using System;

public partial class StageCyrielle : Node2D
{
	private CanvasLayer _gameOverUI;
	private Button _boutonRejouer;
	private bool _fini = false;

	public override void _Ready()
	{
		_gameOverUI = GetNode<CanvasLayer>("GameOverUI");
		_boutonRejouer = GetNode<Button>("GameOverUI/Button");
		_boutonRejouer.Pressed += Rejouer;
	}

	public override void _Process(double delta)
	{
		if (_fini) return;

		// Cercle de sel détruit -> défaite
		if (SaltCircle.Instance == null || !IsInstanceValid(SaltCircle.Instance))
			GameOver();
			
		if (Personnage.Instance == null || !IsInstanceValid(Personnage.Instance))
		GameOver();
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
