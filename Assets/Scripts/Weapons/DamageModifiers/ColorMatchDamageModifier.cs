public class ColorMatchDamageModifier : DamageModifierDecorator
{
    public ColorMatchDamageModifier(IWeaponDamageModifier wrappedModifier) : base(wrappedModifier)
    {
    }

    public override int ModifyDamage(int baseDamage, AmmoColor bulletColor, AmmoColor targetColor)
    {
        if (bulletColor != targetColor)
            return 0;

        return GetWrappedDamage(baseDamage, bulletColor, targetColor);
    }
}