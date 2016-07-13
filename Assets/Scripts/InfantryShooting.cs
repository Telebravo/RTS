using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Unit/InfantryShooting")]
[RequireComponent(typeof(Unit))]
public class InfantryShooting : MonoBehaviour
{
    Unit unit;

    public Unit target;
    public LineRenderer lineRenderer;

    float lastShootTime;
    Vector3[] noline = new Vector3[2] { Vector3.zero, Vector3.zero };

    void Start ()
    {
        unit = GetComponent<Unit>();
        lineRenderer = GetComponent<LineRenderer>();
        lastShootTime = Random.value * 60 / unit.weapon.firerate;
        lineRenderer.SetWidth(unit.weapon.ammo.lineWidth, unit.weapon.ammo.lineWidth);
    }
	
	void Update ()
    {
        if (Time.time > lastShootTime + 0.1f)
            lineRenderer.SetPositions(noline);

        target = unit.closestEnemy;
        Unit hitUnit;
        if (target != null)
        {
            Vector3 targetDirection = target.transform.position - unit.weapon.barrelEnd.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            if (transform.rotation.eulerAngles.y < targetRotation.eulerAngles.y + 3 && transform.rotation.eulerAngles.y > targetRotation.eulerAngles.y - 3)
            {
                if (Time.time >= lastShootTime + (60f / unit.weapon.firerate))
                {
                    RaycastHit hit;

                    if (Physics.Raycast(unit.weapon.barrelEnd.position, targetDirection, out hit, unit.weapon.range))
                    {
                        hitUnit = hit.transform.GetComponent<Unit>();
                        if (hitUnit != null && hitUnit.team != unit.team)
                        {
                            lastShootTime = Time.time;

                            Vector3 shootDirection = targetDirection.normalized + (new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f)*2 / unit.weapon.accuracy);

                            if (Physics.Raycast(unit.weapon.barrelEnd.position, shootDirection, out hit, unit.weapon.range))
                            {
                                //Debug.DrawLine(unit.weapon.barrelEnd.position, hit.point, Color.yellow, 0.1f);
                                lineRenderer.SetPositions(new Vector3[2] { unit.weapon.barrelEnd.position, hit.point });

                                hitUnit = hit.transform.GetComponent<Unit>();

                                if (hitUnit != null)
                                {
                                    hitUnit.Damage(unit.weapon.ammo);
                                }
                            }
                            else
                            {
                                //Debug.DrawRay(unit.weapon.barrelEnd.position, shootDirection.normalized*1000, Color.yellow, 0.2f);
                                lineRenderer.SetPositions(new Vector3[2] { unit.weapon.barrelEnd.position, unit.weapon.barrelEnd.position + shootDirection.normalized * unit.weapon.range });
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
    public void Disable()
    {
        lineRenderer.SetPositions(noline);
        enabled = false;
    }
}
