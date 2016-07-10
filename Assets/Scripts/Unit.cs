using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Unit/Unit")]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(CSelectable))]
[RequireComponent(typeof(Visibility))]
public class Unit : MonoBehaviour
{
    //Komponenter
    [HideInInspector] public CSelectable cSelectable;
    [HideInInspector] public CMoveable cMoveable;
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
    public MountedWeapon[] mountedWeapons;
    public GameObject weaponObject;
    public Sprite icon;
    [HideInInspector]
    public float currentHealth;

    //Fiender og sånt
    [HideInInspector] public List<Unit> enemiesInRange;
    [HideInInspector] public Unit closestEnemy;

    //Unit layeret
    //int unitLayer = 8;
    //int layermask;

    void Awake()
    {
        //unit layeret
        //layermask = 1 << unitLayer;

        //Gir oss noe liv
        currentHealth = startHealth;

        //Legger oss inn i listen for riktig lag
        tag = team.ToString();
        GameManager.units[(int)team].Add(this);

        //Ser om vi har noen (ekstra) våpen
        mountedWeapons = GetComponents<MountedWeapon>();
    }

    void Start()
    {
        cSelectable = GetComponent<CSelectable>(); cSelectable.unit = this;
        cMoveable = GetComponent<CMoveable>();
        visibility = GetComponent<Visibility>();

        StartCoroutine(UpdateEnemies());

        SetWeapon(weapon);
    }

    IEnumerator UpdateEnemies()
    {
        //Finner den lengste rekkeviden på alle våpen
        float maxRange = 0;
        if (weapon != null)
            maxRange = weapon.range;
        for (int i = 0; i < mountedWeapons.Length; i++)
        {
            if (mountedWeapons[i].weapon.range > maxRange)
                maxRange = mountedWeapons[i].weapon.range;
        }

        //Så lenge vi lever
        while (true)
        {
            //venter 0.5 sek
            yield return new WaitForSeconds(0.5f);
            //Ikke mye å gjøre om man ikke kan angripe
            if (maxRange == 0)
                continue;

            //Glemmer alle fra forige gang vi sjekket
            enemiesInRange.Clear();
            closestEnemy = null;
            
            //Distansen til uniten
            float dist;
            //Distansen til den nærmeste uniten
            float minDist = Mathf.Infinity;

            //For hver collider
            foreach (Unit enemy in GameManager.visibleEnemies[(int)team])
            {
                //Finner distansen
                dist = (enemy.transform.position - this.transform.position).magnitude;

                if (dist < maxRange)
                {
                    enemiesInRange.Add(enemy);
                    //Om det er den bærmeste hittil
                    if (dist < minDist)
                    {
                        //Husker på det
                        minDist = dist;
                        closestEnemy = enemy;
                    }
                }
            }
        }
    }

    public void SetWeapon(Weapon newWeapon)
    {
        //Om det ikke er noe driter vi i det
        if (newWeapon == null)
            return;

        //Hvis det har en modell,
        if (newWeapon.loadModel)
        { 
            //Laster inn prefaben
            GameObject newWeaponObject = GameObject.Instantiate(newWeapon.gameObject);
            //Setter den på riktig sted
            newWeaponObject.transform.position = weaponObject.transform.position;
            newWeaponObject.transform.parent = weaponObject.transform.parent;
            weaponObject = newWeaponObject;
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

    public void Damage(Ammunition ammo, float distance = 1)
    {
        if (ammo.damageType == DamageType.Kinetic)
            currentHealth -= ammo.damage * Mathf.Min(ammo.armorPenetration / armor, 1);
        if (ammo.damageType == DamageType.Explosive)
            currentHealth -= ammo.damage / (armor * Mathf.Max(Mathf.Pow(distance, 2) / 10, 1));

        if (currentHealth <= 0)
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        GameManager.controlls.Deselect(this);

        GameManager.units[(int)team].Remove(this);
        for (int i = 0; i < GameManager.teams; i++)
        {
            if (GameManager.visibleEnemies[i].Contains(this))
                GameManager.visibleEnemies[i].Remove(this);
        }
        

        GameObject.Destroy(gameObject);
    }
}
