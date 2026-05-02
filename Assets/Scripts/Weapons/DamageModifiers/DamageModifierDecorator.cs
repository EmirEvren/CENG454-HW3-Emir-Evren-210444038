public abstract class DamageModifierDecorator : IWeaponDamageModifier
{
    protected readonly IWeaponDamageModifier wrappedModifier;

    protected DamageModifierDecorator(IWeaponDamageModifier wrappedModifier)
    {
        this.wrappedModifier = wrappedModifier;
    }

    public abstract int ModifyDamage(int baseDamage, AmmoColor bulletColor, AmmoColor targetColor);

    protected int GetWrappedDamage(int baseDamage, AmmoColor bulletColor, AmmoColor targetColor)
    {
        if (wrappedModifier == null)
            return baseDamage;

        return wrappedModifier.ModifyDamage(baseDamage, bulletColor, targetColor);
    }
}