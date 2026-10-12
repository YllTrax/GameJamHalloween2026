using System.Collections.Generic;
using System.Runtime.InteropServices;
using Godot;

public partial class SaltDeposit : Node2D, IInteractable
{
    [Export]
    private float baseSaltAmount = 10f;

    private float saltAmount;
    private Tween _pulseTween;
    private Vector2 _baseScale;

    [Export]
    private Polygon2D outLine;

    [Export]
    public Area2D[] areas;

    public override void _Ready()
    {
        _baseScale = Scale; // indispensable pour le Pulse sinon ca disparait c'est relou
        saltAmount = 0;

        GetRandomPos(areas[GD.RandRange(0, areas.Length - 1)]);
    }

    public void Refill()
    {
        saltAmount = baseSaltAmount * UpgradeStats.SaltMult;
        GD.Print(
            $"[Sel] Dépôt rechargé : base {baseSaltAmount} x{UpgradeStats.SaltMult:0.00} = {saltAmount}"
        );
        outLine.Visible = true;
        Pulse();
    }

    public void Interact()
    {
        if (saltAmount <= 0)
        {
            return;
        }

        Personnage.Instance.carriedSalt += saltAmount;
        saltAmount = 0;
        PulseAndGetRandomPos();
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

    private void PulseAndGetRandomPos()
    {
        _pulseTween?.Kill();
        Scale = _baseScale;

        _pulseTween = CreateTween();
        _pulseTween.SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        _pulseTween.TweenProperty(this, "scale", _baseScale * new Vector2(1.8f, 1.3f), 0.15f);
        _pulseTween.TweenProperty(this, "scale", _baseScale, 0.15f);
        GetRandomPos(areas[GD.RandRange(0, areas.Length - 1)]);
    }

    public void InteractRightclick()
    {
        return;
        //  PulseAndGetRandomPos();
    }

    public void InteractMiddleButton()
    {
        return;
    }

    private void GetRandomPos(Area2D newArea)
    {
        var area = newArea.GetNode<CollisionShape2D>("CollisionShape2D");

        // Rectangle local de la forme (marche pour n'importe quelle Shape2D)
        Rect2 local = area.Shape.GetRect();

        // Passage en coordonnées globales (prend en compte position, scale, rotation)
        Rect2 global = area.GlobalTransform * local;

        Vector2 min = global.Position;
        Vector2 max = global.End;

        GlobalPosition = new Vector2(
            (float)GD.RandRange(min.X, max.X),
            (float)GD.RandRange(min.Y, max.Y)
        );
        outLine.Visible = false;
    }
}
