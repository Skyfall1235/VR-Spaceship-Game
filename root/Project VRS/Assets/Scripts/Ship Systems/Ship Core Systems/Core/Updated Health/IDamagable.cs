using static Health;

public interface IDamagable
{
    public void Damage(DamageData damageData, bool ignoreInvulnerabilityAfterDamage = false, bool ignoreArmor = false);
}
