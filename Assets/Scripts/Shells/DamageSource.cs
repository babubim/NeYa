
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    public int Damage { get; private set; } = 1;

    public void IncreaseDamage(int amount)
    {
        if (amount > 0)
            Damage += amount;
    }
}
