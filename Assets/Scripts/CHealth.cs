using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    public void Damage(float amount, DamageType type, float distance = 1)
    {
        if (isDead)
            return;

        if (type == DamageType.Kinetic)
            currentHealth -= amount / unit.armor;
        if (type == DamageType.Explosive)
            currentHealth -= amount / (unit.armor*Mathf.Pow(distance, 1.5f));

        if (currentHealth <= 0)
        {
            Death();
        }
    }
    void Death()
    {
        currentHealth = 0;
        isDead = true;
        CameraControlls.Deselect(transform);
        this.gameObject.SetActive(false);
    }
}
