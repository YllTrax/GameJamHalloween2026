using Godot;

public partial class SaltDeposit : Node2D, IInteractable
{
    [Export]
    private float baseSaltAmount = 10f;

    [Export]
    private PackedScene IconScene;

    [Export]
    private Polygon2D outLine;

    [Export]
    public Area2D[] areas;

    private float saltAmount;
    private Vector2 _baseScale;
    private Tween _pulseTween;

    private Label _target;
    private Tween _labelPulseTween;

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
            return;

        Personnage.Instance.carriedSalt += saltAmount;
        saltAmount = 0;
        DragonFlyX(6, GlobalPosition); // position AVANT le déplacement
        PulseAndGetRandomPos();
    }

    public void InteractRightclick()
    {
        return;
    }

    public void InteractMiddleButton()
    {
        return;
    }

    // ───────────── Feedback du dépôt ─────────────

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
        Pulse();
        GetRandomPos(areas[GD.RandRange(0, areas.Length - 1)]);
    }

    private void GetRandomPos(Area2D newArea)
    {
        var area = newArea.GetNode<CollisionShape2D>("CollisionShape2D");

        Rect2 local = area.Shape.GetRect();

        Rect2 global = area.GlobalTransform * local;

        Vector2 min = global.Position;
        Vector2 max = global.End;

        GlobalPosition = new Vector2(
            (float)GD.RandRange(min.X, max.X),
            (float)GD.RandRange(min.Y, max.Y)
        );
        outLine.Visible = false;
    }

    // ───────────── Icônes de sel vers l'UI ─────────────

    public void DragonFlyX(int x, Vector2 fromPos)
    {
        for (int i = 0; i < x; i++)
        {
            GetTree().CreateTimer(0.08 * i).Timeout += () => Fly(fromPos);
        }
    }

    public void Fly(Vector2 worldPos)
    {
        var ui = GetTree().GetFirstNodeInGroup("ui") as CanvasLayer;
        _target ??= GetTree().GetFirstNodeInGroup("salt_label") as Label;

        if (ui == null || _target == null || IconScene == null)
        {
            GD.PrintErr("[Sel] Fly : groupe 'ui', groupe 'salt_label' ou IconScene manquant");
            return;
        }

        // Monde -> écran, avec un petit décalage aléatoire
        Vector2 start =
            GetViewport().GetCanvasTransform() * worldPos
            + new Vector2((float)GD.RandRange(-15, 15), (float)GD.RandRange(-15, 15));
        Vector2 end = _target.GlobalPosition + _target.Size / 2;

        var icon = IconScene.Instantiate<Control>();
        ui.AddChild(icon);

        // Le Control se positionne par son coin, on le centre
        Vector2 half = icon.Size / 2;
        icon.PivotOffset = half;
        icon.Position = start - half;
        icon.Scale = Vector2.Zero;

        Tween tween = icon.CreateTween();
        // Petit pop d'apparition
        tween
            .TweenProperty(icon, "scale", Vector2.One, 0.15f)
            .SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);
        // Vol vers le label
        tween
            .TweenProperty(icon, "position", end - half, 0.5f)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.In);
        // Arrivée
        tween.TweenCallback(
            Callable.From(() =>
            {
                icon.QueueFree();
                PulseLabel();
            })
        );
    }

    private void PulseLabel()
    {
        _labelPulseTween?.Kill();
        _target.Scale = Vector2.One;
        _target.PivotOffset = _target.Size / 2;

        _labelPulseTween = _target.CreateTween();
        _labelPulseTween.TweenProperty(_target, "scale", Vector2.One * 1.2f, 0.08f);
        _labelPulseTween.TweenProperty(_target, "scale", Vector2.One, 0.12f);
    }
}
