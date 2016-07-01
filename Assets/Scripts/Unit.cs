using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    [HideInInspector] public CSelectable cSelectable;
    [HideInInspector] public CMoveable cMoveable;
    [HideInInspector] public CHealth cHealth;

    public string displayName = "Unit";
    public int startHealth = 100;
    public int size = 1;
    public int optics = 1;
    public int armor = 1;
    public int movementSpeed = 5;
    public int rotationSpeed = 120;
    public Weapons _weapon = Weapons.HK416;
    [HideInInspector]
    public Weapon weapon;

    public Collider[] collidersInRange;
    public List<Transform> enemiesInRange;
    public Transform closestEnemy;

    //Unit layeret
    int layer = 8;
    int layermask;

    void Awake()
    {
        weapon = Weapon.Get(_weapon);
        layermask = 1 << layer;
    }
    void Start()
    {
        cSelectable = GetComponent<CSelectable>();
        cMoveable = GetComponent<CMoveable>();
        cHealth = GetComponent<CHealth>();

        StartCoroutine(UpdateEnemies());
    }
    IEnumerator UpdateEnemies()
    {
        while (true)
        {
            collidersInRange = Physics.OverlapSphere(transform.position, weapon.range, layermask);
            enemiesInRange.Clear();

            closestEnemy = null;
            float dist;
            float minDist = Mathf.Infinity;

            for (int i = 0; i < collidersInRange.Length; i++)
            {
                if (collidersInRange[i].tag != this.tag && collidersInRange[i].tag.Contains("Team"))
                {
                    enemiesInRange.Add(collidersInRange[i].transform);

                    dist = (collidersInRange[i].transform.position - this.transform.position).magnitude;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closestEnemy = collidersInRange[i].transform;
                    }
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
