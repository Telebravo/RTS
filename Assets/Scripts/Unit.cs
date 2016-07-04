using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Unit/Unit")]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(CSelectable))]
[RequireComponent(typeof(CHealth))]
[RequireComponent(typeof(Visibility))]
public class Unit : MonoBehaviour
{
    //Komponenter
    [HideInInspector] public CSelectable cSelectable;
    [HideInInspector] public CMoveable cMoveable;
    [HideInInspector] public CHealth cHealth;
    [HideInInspector] public Visibility visibility;

    //Stats?
    public Team team = Team.Team1;
    public string displayName = "Unit";
    public int startHealth = 100;
    public int size = 1;
    public int optics = 0;
    public int camo = 0;
    public int armor = 1;
    public int movementSpeed = 5;
    public int rotationSpeed = 120;
    public Weapon weapon;
    public GameObject weaponObject;
    public Texture2D icon;

    //Fiender og sånt
    Collider[] collidersInRange;
    [HideInInspector] public List<Unit> enemiesInRange;
    [HideInInspector] public Unit closestEnemy;

    //Unit layeret
    int unitLayer = 8;
    int layermask;

    void Awake()
    {
        tag = team.ToString();

        layermask = 1 << unitLayer;
        if(team == GameManager.team)
        {
            GameManager.friendlyUnits.Add(this);
        }
        else
        {
            GameManager.enemyUnits.Add(this);
        }
    }
    void Start()
    {
        cSelectable = GetComponent<CSelectable>(); cSelectable.unit = this;
        cMoveable = GetComponent<CMoveable>();
        cHealth = GetComponent<CHealth>();
        visibility = GetComponent<Visibility>();

        StartCoroutine(UpdateEnemies());

        SetWeapon(weapon);
    }
    IEnumerator UpdateEnemies()
    {
        Unit enemyUnit;
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (weapon == null)
                continue;

            collidersInRange = Physics.OverlapSphere(transform.position, weapon.range, layermask);
            enemiesInRange.Clear();

            closestEnemy = null;
            float dist;
            float minDist = Mathf.Infinity;

            for (int i = 0; i < collidersInRange.Length; i++)
            {
                enemyUnit = collidersInRange[i].GetComponent<Unit>();
                if (enemyUnit.team != this.team)
                {
                    enemiesInRange.Add(enemyUnit);

                    dist = (enemyUnit.transform.position - this.transform.position).magnitude;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closestEnemy = enemyUnit;
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
            newWeaponObject.transform.position = weaponObject.transform.position;
            newWeaponObject.transform.parent = weaponObject.transform.parent;
            weaponObject = newWeaponObject;
            weapon = newWeaponObject.GetComponent<Weapon>();
        }
        else
        {
            weapon = newWeapon;
        }
    }
}
