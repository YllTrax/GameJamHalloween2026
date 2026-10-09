using Godot;

public partial class SaltDeposit : Node2D, IInteractable
{
    [Export]
    private float baseSaltAmount = 10f;

    private float saltAmount;
    private Tween _pulseTween;
    private Vector2 _baseScale;

    public override void _Ready()
    {
        _baseScale = Scale; // indispensable pour le Pulse sinon ca disparait c'est relou
        saltAmount = 0;
    }

    public void Refill()
    {
        saltAmount = baseSaltAmount;
        Pulse();
    }

    public void Interact()
    {
        if (saltAmount <= 0)
            return;

        Personnage.Instance.carriedSalt += saltAmount;
        saltAmount = 0;
        Pulse();
    }

    private void Pulse()
    {
        _pulseTween?.Kill();
        Scale = _baseScale;

        _pulseTween = CreateTween();
        _pulseTween.SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        _pulseTween.TweenProperty(this, "scale", _baseScale * new Vector2(1.8f, 1.3f), 0.15f);
        _pulseTween.TweenProperty(this, "scale", _baseScale, 0.15f);
    }

    public void InteractRightclick() { }

    public void InteractMiddleButton() { }
}
