using UnityEngine;
using System.Collections;

public class MountedWeaponShooter : MonoBehaviour
{
    public Weapon weapon;
    public GameObject position;

    Unit unit;
    Unit target;
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

        if(target != null)
        {
            Vector3 targetDirection = target.transform.position - weapon.barrelEnd.position;
        }
	}

    public void SetWeapon(Weapon newWeapon)
    {
        if (newWeapon == null)
            return;
        if (newWeapon.loadModel)
        {
            GameObject newWeaponObject = GameObject.Instantiate(newWeapon.gameObject);
            newWeaponObject.transform.position = position.transform.position;
            newWeaponObject.transform.parent = position.transform.parent;
            position = newWeaponObject;
            weapon = newWeaponObject.GetComponent<Weapon>();
        }
        else
        {
            weapon = newWeapon;
        }
    }
}
