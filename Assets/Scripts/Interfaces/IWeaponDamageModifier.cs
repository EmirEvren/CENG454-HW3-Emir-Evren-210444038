public interface IWeaponDamageModifier
{
    int ModifyDamage(int baseDamage, AmmoColor bulletColor, AmmoColor targetColor);
}