using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TanksShoter : MonoBehaviour
{
    public GameObject Shot;
    Unit unit;
    public Transform target;
    public Transform turret;
    public Transform barrelEnd;
    float lastShootTime;

    private float range;

    void Start()
    {
        unit = GetComponent<Unit>();
        lastShootTime = 1;
    }

    void Update()
    {
        target = unit.closestEnemy;
        if (target != null)
        {
            Vector3 targetDirection = target.position - barrelEnd.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            if (turret.rotation.eulerAngles.y < targetRotation.eulerAngles.y + 3 && turret.rotation.eulerAngles.y > targetRotation.eulerAngles.y - 3)
            {
                if (Time.time >= lastShootTime + (60 / unit.weapon.firerate))
                {
                    lastShootTime = Time.time;
                    GameObject Shell = Instantiate<GameObject>(Shot);
                    Shell.layer = 0;
                    ShotHandler shotHandlerShell = Shell.GetComponent<ShotHandler>();
                    Shell.GetComponent<Rigidbody>().velocity = ((target.position - barrelEnd.position).normalized) * shotHandlerShell.Speed;
                    Shell.transform.position =  barrelEnd.position;
                    Shell.transform.rotation = barrelEnd.rotation;

                    if ((target.position - barrelEnd.position).magnitude < unit.weapon.range)
                    {
                        range = (target.position - barrelEnd.position).magnitude;
                    }
                    else
                    {
                        range = unit.weapon.range;
                    }
                    shotHandlerShell.AirTime = range / shotHandlerShell.Speed;
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
