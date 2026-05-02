using UnityEngine;

public class BonusDamageModifier : DamageModifierDecorator
{
    private readonly int bonusDamage;

    public BonusDamageModifier(IWeaponDamageModifier wrappedModifier, int bonusDamage) : base(wrappedModifier)
    {
        this.bonusDamage = Mathf.Max(0, bonusDamage);
    }

    public override int ModifyDamage(int baseDamage, AmmoColor bulletColor, AmmoColor targetColor)
    {
        int currentDamage = GetWrappedDamage(baseDamage, bulletColor, targetColor);

        if (currentDamage <= 0)
            return 0;

        return currentDamage + bonusDamage;
    }
}