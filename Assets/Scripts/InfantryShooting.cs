using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InfantryShooting : MonoBehaviour
{
    Unit unit;
    CHealth hitHealth;

    public Transform target;

    float lastShootTime;

	void Start ()
    {
        unit = GetComponent<Unit>();
        lastShootTime = 1;
    }
	
	void Update ()
    {
        target = unit.closestEnemy;

        if (target != null)
        {
            Vector3 targetDirection = target.position - unit.weapon.barrelEnd.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            if (transform.rotation.eulerAngles.y < targetRotation.eulerAngles.y + 3 && transform.rotation.eulerAngles.y > targetRotation.eulerAngles.y - 3)
            {
                if (Time.time >= lastShootTime + (60f / unit.weapon.firerate))
                {
                    RaycastHit hit;

                    if (Physics.Raycast(unit.weapon.barrelEnd.position, targetDirection, out hit, unit.weapon.range))
                    {
                        if (hit.transform == target)
                        {
                            lastShootTime = Time.time;

                            Vector3 shootDirection = targetDirection.normalized + (new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f)*2 / unit.weapon.accuracy);

                            if (Physics.Raycast(unit.weapon.barrelEnd.position, shootDirection, out hit, unit.weapon.range))
                            {
                                Debug.DrawLine(unit.weapon.barrelEnd.position, hit.point, Color.yellow, 0.1f);

                                hitHealth = hit.transform.GetComponent<CHealth>();

                                if (hitHealth != null)
                                {
                                    hitHealth.Damage(unit.weapon.ammo);
                                }
                            }
                            else
                            {
                                Debug.DrawRay(unit.weapon.barrelEnd.position, shootDirection.normalized*1000, Color.yellow, 0.2f);
                            }
                        }
                    }
                }
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.3f);
            }
        }
    }
}
