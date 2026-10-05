using System;
using Godot;

public partial class SaltDeposit : Node2D, IInteractable
{
    [Export]
    private float saltAmount;

    public override void _Ready() { }

    public override void _Process(double delta) { }

    private void Pulse()
    {
        Tween tween = CreateTween();
        tween.SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(this, "scale", new Vector2(1.3f, 1.3f), 0.15f);
        tween.TweenProperty(this, "scale", Vector2.One, 0.15f);
    }

    public void Interact()
    {
        Personnage.Instance.carriedSalt += saltAmount;
        Pulse();
        saltAmount = 0;
    }
}
