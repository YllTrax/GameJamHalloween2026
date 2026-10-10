using Godot;

public partial class MenuLangues : CanvasLayer
{
    [Export]
    private BaseButton btnRetour;

    [Export]
    private CanvasLayer menuPause;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        Visible = false;

        if (btnRetour != null)
            btnRetour.Pressed += Retour;
    }

    public void ChoisirLangue(string locale)
    {
        TranslationServer.SetLocale(locale);
    }

    private void Retour()
    {
        Visible = false;
        if (menuPause != null)
            menuPause.Visible = true;
    }
}
