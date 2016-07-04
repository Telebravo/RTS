using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Unit/CHealth")]
[RequireComponent(typeof(Unit))]
public class CHealth : MonoBehaviour
{
    Unit unit;
    public bool isDead;
    public float currentHealth;

    void Start ()
    {
        unit = GetComponent<Unit>();
        currentHealth = unit.startHealth;
    }

    public void Damage(Ammunition ammo, float distance = 1)
    {
        if (isDead)
            return;

        if (ammo.damageType == DamageType.Kinetic)
            currentHealth -= ammo.damage * Mathf.Min(ammo.armorPenetration / unit.armor, 1);
        if (ammo.damageType == DamageType.Explosive)
            currentHealth -= ammo.damage / (unit.armor*Mathf.Max(Mathf.Pow(distance, 2)/10, 1));

        if (currentHealth <= 0)
        {
            Death();
        }
    }
    void Death()
    {
        currentHealth = 0;
        isDead = true;
        GameManager.controlls.Deselect(unit);
        this.gameObject.SetActive(false);
    }
}
