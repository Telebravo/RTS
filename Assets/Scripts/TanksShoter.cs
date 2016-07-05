using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Unit/TanksShoter")]
[RequireComponent(typeof(Unit))]
public class TanksShoter : MonoBehaviour
{
    Unit unit;

    public Transform turret;
    public Transform barrelEnd;

    private Unit target;
    private float lastShootTime;
    private float range;

    void Start()
    {
        unit = GetComponent<Unit>();
        lastShootTime = Random.value * 60 / unit.weapon.firerate;
    }

    void Update()
    {
        target = unit.closestEnemy;

        if (target != null)
        {
            Vector3 targetDirection = target.transform.position - barrelEnd.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            if (turret.rotation.eulerAngles.y < targetRotation.eulerAngles.y + 1 && turret.rotation.eulerAngles.y > targetRotation.eulerAngles.y - 1)
            {
                if (Time.time >= lastShootTime + (60 / unit.weapon.firerate))
                {
                    RaycastHit hit;

                    if (Physics.Raycast(barrelEnd.position, targetDirection, out hit, unit.weapon.range))
                    {
                        if (hit.transform == target.transform)
                        {
                            lastShootTime = Time.time;

                            GameObject newShell = Instantiate<GameObject>(unit.weapon.ammo.projectilePrefab);
                            newShell.layer = 0;
                            newShell.transform.position = barrelEnd.position;
                            newShell.transform.rotation = barrelEnd.rotation;

                            ShotHandler shotHandlerShell = newShell.GetComponent<ShotHandler>();
                            newShell.GetComponent<Rigidbody>().velocity = ((target.transform.position - barrelEnd.position).normalized) * shotHandlerShell.Speed;

                            if ((target.transform.position - barrelEnd.position).magnitude < unit.weapon.range)
                            {
                                range = (target.transform.position - barrelEnd.position).magnitude;
                            }
                            else
                            {
                                range = unit.weapon.range;
                            }

                            shotHandlerShell.AirTime = unit.weapon.ammo.damageType == DamageType.Explosive ? range / shotHandlerShell.Speed : 99999;
                            shotHandlerShell.ammo = unit.weapon.ammo;
                        }
                    }
                }
            }
            else
            {
                turret.rotation = Quaternion.Lerp(turret.rotation, targetRotation, 0.3f);
                turret.Rotate(Vector3.right, turret.rotation.x, Space.World);
            }
        }
    }
}
