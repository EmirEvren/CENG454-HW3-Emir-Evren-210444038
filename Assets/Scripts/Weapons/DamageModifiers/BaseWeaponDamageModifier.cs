public class BaseWeaponDamageModifier : IWeaponDamageModifier
{
    public int ModifyDamage(int baseDamage, AmmoColor bulletColor, AmmoColor targetColor)
    {
        return baseDamage;
    }
}