using Godot;

public static class UpgradeStats
{
	// Bonus en fraction : 0.10 = +10 %
	public static float SaltBonus;
	public static float DoorBonus;
	public static float TurretDamageBonus;
	public static float TurretAmmoBonus;

	public static float SaltMult => 1f + SaltBonus;
	public static float DoorMult => 1f + DoorBonus;
	public static float TurretDamageMult => 1f + TurretDamageBonus;
	public static float TurretAmmoMult => 1f + TurretAmmoBonus;

    // Applique le bonus de vie à UNE porte. Peut être rappelée : seul l'écart est appliqué.
    public static void ApplyDoorBonus(Node door)
    {
        var health = door.GetNodeOrNull<Health>("Health");
        if (health == null) return;

        float deja = door.HasMeta("door_mult") ? door.GetMeta("door_mult").AsSingle() : 1f;
        float ratio = DoorMult / deja;

        health.baseHealth *= ratio;
        health.currentHealth *= ratio;   // garde le même pourcentage de vie
        door.SetMeta("door_mult", DoorMult);
    }
	public static void Reset()
	{
		SaltBonus = DoorBonus = TurretDamageBonus = TurretAmmoBonus = 0f;
	}
	public static void PrintAll()
	{
		GD.Print($"[Upgrades] Sel x{SaltMult:0.00} | Portes x{DoorMult:0.00} | Dégâts tourelles x{TurretDamageMult:0.00}");
	}
}