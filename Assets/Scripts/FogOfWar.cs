using UnityEngine;
using System.Collections;

public class FogOfWar : MonoBehaviour
{
    private Collider[] unitsInRange;
    private float visibleDistance;

    //Unit layeret
    int unitLayer = 8;
    int layermask;

    void Awake()
    {
        layermask = 1 << unitLayer;
    }

    void Start()
    {
        StartCoroutine(UpdateEnemies());
    }

    IEnumerator UpdateEnemies()
    {
        Unit unit;
        Unit enemy;

        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            GameManager.visibleEnemies.Clear();

            for (int i = 0; i < GameManager.friendlyUnits.Count; i++)
            {
                if(i % 10 == 0)
                yield return new WaitForFixedUpdate();

                unit = GameManager.friendlyUnits[i];
                unitsInRange = Physics.OverlapSphere(unit.transform.position, unit.optics * 2, layermask);
                
                for (int n = 0; n < unitsInRange.Length; n++)
                {
                    enemy = unitsInRange[n].GetComponent<Unit>();

                    if (enemy.team != GameManager.team)
                    {
                        visibleDistance = unit.optics * enemy.visibility.GetVisibility();

                        if (Vector3.Distance(enemy.transform.position, unit.transform.position) < visibleDistance)
                        {
                            if (!GameManager.visibleEnemies.Contains(enemy))
                                GameManager.visibleEnemies.Add(enemy);
                        }
                    }
                }
            }
            for (int i = 0; i < GameManager.enemyUnits.Count; i++)
            {
                if (i % 10 == 0)
                    yield return new WaitForFixedUpdate();

                enemy = GameManager.enemyUnits[i];
                if (GameManager.visibleEnemies.Contains(enemy))
                {   
                    enemy.GetComponent<MeshRenderer>().enabled = true;
                }
                else
                {
                    enemy.GetComponent<MeshRenderer>().enabled = false;
                }

            }
        }
    }
}