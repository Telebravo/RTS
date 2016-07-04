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
        //Skyter på den nærmeste duden
        target = unit.closestEnemy;

        //om vi har et mål
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
                        //Ser hva vi treffer
                        RaycastHit hit;
                        if (Physics.Raycast(weapon.barrelEnd.position, targetDirection, out hit, weapon.range))
                        {
                            //Om det er en fientlig unit
                            hitUnit = hit.transform.GetComponent<Unit>();
                            if (hitUnit != null && hitUnit.team != unit.team)
                            {
                                //Husker når vi sist skjøt
                                lastShootTime = Time.time;

                                //Om det ikke skyter noe prosjektil
                                if (weapon.ammo.projectilePrefab == null)
                                {
                                    //Accuracy
                                    Vector3 shootDirection = targetDirection.normalized + (new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f) * 2 / weapon.accuracy);

                                    //Ser om vi treffer noe
                                    if (Physics.Raycast(weapon.barrelEnd.position, shootDirection, out hit, weapon.range))
                                    {
                                        //Tegner linje
                                        Debug.DrawLine(weapon.barrelEnd.position, hit.point, Color.yellow, 0.1f);

                                        //Ser om vi treff en unit
                                        hitUnit = hit.transform.GetComponent<Unit>();
                                        if (hitUnit != null)
                                        {
                                            //Påfører skade
                                            hitUnit.Damage(weapon.ammo);
                                        }
                                    }
                                    //Om vi ikke traff noe
                                    else
                                    {
                                        //Tegner en linje med lengden til våpenets rekkevidde
                                        Debug.DrawRay(weapon.barrelEnd.position, shootDirection.normalized * 1000, Color.yellow, 0.2f);
                                    }
                                }
                                //Om vi skal skyte et prosjektil
                                else
                                {
                                    //Lager et nytt prosjektil
                                    GameObject newShell = Instantiate<GameObject>(weapon.ammo.projectilePrefab);

                                    //Setter layeret til Default
                                    newShell.layer = 0;
                                    //Posisjonerer det
                                    newShell.transform.position = weapon.barrelEnd.position;
                                    newShell.transform.rotation = weapon.barrelEnd.rotation;

                                    //Henter ShotHandler komponenten
                                    ShotHandler shotHandlerShell = newShell.GetComponent<ShotHandler>();
                                    
                                    //Setter farten
                                    newShell.GetComponent<Rigidbody>().velocity = ((target.transform.position - weapon.barrelEnd.position).normalized) * shotHandlerShell.Speed;
                                    
                                    //Setter airtime
                                    float range = Mathf.Min((target.transform.position - weapon.barrelEnd.position).magnitude, weapon.range);
                                    shotHandlerShell.AirTime = unit.weapon.ammo.damageType == DamageType.Explosive ? range / shotHandlerShell.Speed : 99999;

                                    //Setter hvilken ammo prosjektilet kommer fra
                                    shotHandlerShell.ammo = unit.weapon.ammo;
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
        //om det ikke er noe driter vi i det
        if (newWeapon == null)
            return;

        //Hvis det har en modell,
        if (newWeapon.loadModel)
        {
            //Laster inn prefaben
            GameObject newWeaponObject = GameObject.Instantiate(newWeapon.gameObject);
            //Setter den på riktig sted
            newWeaponObject.transform.position = weaponPosition.position;
            newWeaponObject.transform.parent = weaponPosition.transform.parent;
            //Henter Weapon komponenten
            weapon = newWeaponObject.GetComponent<Weapon>();
        }
        //Om ikke
        else
        {
            //Setter våpenet som det nye
            weapon = newWeapon;
        }
    }
}
