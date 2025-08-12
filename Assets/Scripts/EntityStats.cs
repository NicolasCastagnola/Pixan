using System;
[Serializable]
public class EntityStats
{
    public HealthComponent HealthComponent;
    public int rawDamage, defaultDamage;
    public float rawWalkSpeed, defaultSpeed;
    public int rawArmor, defaultArmor;

    public int defaultHealth = 100;

    public void AddArmor(int amount) => rawArmor += amount;
    public int AddDamage(int amount) => rawDamage += amount;
    public void AddWalkSpeed(float amount) => rawWalkSpeed += amount;
    public void AddToMaxHealth(int amount, bool shouldFullHeal = false) => HealthComponent.SetMaxHealth(amount, shouldFullHeal);

    public void ResetValues()
    {
        rawDamage = defaultDamage;
        rawWalkSpeed = defaultSpeed;
        rawArmor = defaultArmor;
        HealthComponent.ResetHealth(defaultHealth);
    }
}