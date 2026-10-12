using System;
using Godot;

public partial class SaltyIcone : Control
{
    public Vector2 arrivee;

    public override void _Ready()
    {
        // AminFaitViteTrouveUnNom();
    }

    public void AminFaitViteTrouveUnNom()
    {
        Tween tween = CreateTween();

        tween
            .TweenProperty(this, "position", arrivee, 1f)
            .SetTrans(Tween.TransitionType.Bounce)
            .SetEase(Tween.EaseType.InOut);
    }
}
