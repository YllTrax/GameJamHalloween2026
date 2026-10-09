using Godot;

public partial class Munition : Area2D
{
    [ExportGroup("Apparition")]
    [Export]
    private float dropDuration = 0.4f;

    [ExportGroup("Ramassage")]
    [Export]
    private float duration = 0.35f;

    [Export]
    private int value = 1;

    [ExportGroup("")]
    private bool _collected;
    private bool _hasDrop;
    private Vector2 _dropTarget;

    /// <summary>
    /// À appeler AVANT d'ajouter le germe à la scène
    /// </summary>
    public void SetDrop(Vector2 target)
    {
        _dropTarget = target;
        _hasDrop = true;
    }

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;

        if (_hasDrop)
            PlayDrop();
    }

    // ---------- Apparition ----------
    private void PlayDrop()
    {
        Monitoring = false; // pas ramassable pendant qu'il vole

        Vector2 baseScale = Scale;
        Scale = Vector2.Zero; // part de rien

        var tween = CreateTween().SetParallel();

        // Jaillit vers sa position finale
        tween
            .TweenProperty(this, "position", _dropTarget, dropDuration)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);

        // Grossit en même temps avec un petit rebond
        tween
            .TweenProperty(this, "scale", baseScale, dropDuration)
            .SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);

        // Quand il est posé, il devient ramassable
        tween.Chain().TweenCallback(Callable.From(() => Monitoring = true));
    }

    // ---------- Ramassage ----------
    private void OnBodyEntered(Node2D body)
    {
        if (_collected || body is not Personnage personnage)
            return;

        _collected = true;
        SetDeferred(Area2D.PropertyName.Monitoring, false);

        Vector2 start = GlobalPosition;
        Vector2 baseScale = Scale;

        var tween = CreateTween(); // séquentiel par défaut

        // 1. Pop
        tween
            .TweenProperty(this, "scale", baseScale * 1.3f, 0.08f)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);

        // 2. Aspiration vers le joueur...
        tween
            .TweenMethod(
                Callable.From<float>(t =>
                {
                    if (IsInstanceValid(personnage))
                        GlobalPosition = start.Lerp(personnage.GlobalPosition, t);
                }),
                0f,
                1f,
                duration
            )
            .SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.In);

        // ...pendant qu'il rétrécit
        tween
            .Parallel()
            .TweenProperty(this, "scale", Vector2.Zero, duration)
            .SetEase(Tween.EaseType.In);

        // 3. Fin
        tween.TweenCallback(
            Callable.From(() =>
            {
                if (IsInstanceValid(personnage))
                    personnage.OnCollectMunition(value);
                QueueFree();
            })
        );
    }
}
