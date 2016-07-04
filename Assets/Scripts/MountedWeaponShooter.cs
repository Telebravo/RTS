using UnityEngine;
using System.Collections;

public class MountedWeaponShooter : MonoBehaviour
{
    public Weapon weapon;
    public Transform mount;
    public Transform weaponPosition;

    public float mountRotationSpeed = 50;
    public float weaponElevationSpeed = 50;

    Unit unit;
    Unit target;
    Unit hitUnit;

    float lastShootTime;

    void Start ()
    {
        unit = GetComponent<Unit>();
        SetWeapon(weapon);
        lastShootTime = 0;
	}
	
	void Update ()
    {
        target = unit.closestEnemy;

        if (target != null)
        {
            //Retningen mot målet
            Vector3 targetDirection = target.transform.position - weapon.barrelEnd.position;
            //Fetningen mot målet om man ignorerer høyde
            Vector3 flatDirection = targetDirection - Vector3.up * targetDirection.y;

            //Roterer mounten
            Quaternion mountRotation = Quaternion.LookRotation(flatDirection, Vector3.up);
            mount.rotation = Quaternion.RotateTowards(mount.rotation, mountRotation, mountRotationSpeed * Time.deltaTime);

            //Roterer våpenet
            float angle = Mathf.Atan2(targetDirection.y, new Vector2(flatDirection.x, flatDirection.z).magnitude) * Mathf.Rad2Deg;
            weapon.transform.localRotation = Quaternion.RotateTowards(weapon.transform.localRotation, Quaternion.Euler(new Vector3(-angle, 0, 0)), weaponElevationSpeed * Time.deltaTime);

            //Om mounten er riktig rotert
            if (Quaternion.Angle(mount.rotation, mountRotation) < 1)
            {
                //Om våoenet er riktig rotert
                if (Quaternion.Angle(weapon.transform.localRotation, Quaternion.Euler(new Vector3(-angle, 0, 0))) < 1)
                {
                    //Oppdaterer retningen til fra tuppen av våpenet
                    targetDirection = target.transform.position - weapon.barrelEnd.position;
                    if (Time.time >= lastShootTime + (60f / weapon.firerate))
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(weapon.barrelEnd.position, targetDirection, out hit, weapon.range))
                        {
                            hitUnit = hit.transform.GetComponent<Unit>();
                            if (hitUnit != null && hitUnit.team != unit.team)
                            {
                                lastShootTime = Time.time;

                                Vector3 shootDirection = targetDirection.normalized + (new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f) * 2 / weapon.accuracy);

                                if (Physics.Raycast(weapon.barrelEnd.position, shootDirection, out hit, weapon.range))
                                {
                                    Debug.DrawLine(weapon.barrelEnd.position, hit.point, Color.yellow, 0.1f);

                                    hitUnit = hit.transform.GetComponent<Unit>();

                                    if (hitUnit != null)
                                    {
                                        hitUnit.Damage(weapon.ammo);
                                    }
                                }
                                else
                                {
                                    Debug.DrawRay(weapon.barrelEnd.position, shootDirection.normalized * 1000, Color.yellow, 0.2f);
                                }
                            }
                        }
                    }
                }
            }
        }
	}

    public void SetWeapon(Weapon newWeapon)
    {
        if (newWeapon == null)
            return;

        if (newWeapon.loadModel)
        {
            GameObject newWeaponObject = GameObject.Instantiate(newWeapon.gameObject);
            newWeaponObject.transform.position = weaponPosition.position;
            newWeaponObject.transform.parent = weaponPosition.transform.parent;
            weapon = newWeaponObject.GetComponent<Weapon>();
        }
        else
        {
            weapon = newWeapon;
        }
    }
}
