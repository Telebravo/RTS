using UnityEngine;
using System.Collections;

public class FogOfWar : MonoBehaviour
{
    //Unit layeret
    int unitLayer = 8;
    int layermask;

    void Awake()
    {
        layermask = 1 << unitLayer;
    }

    void Start()
    {
        StartCoroutine(UpdateUnits());
    }

    IEnumerator UpdateUnits()
    {
        Collider[] unitsInRange;
        float visibleDistance;
        Unit friendlyUnit;
        Unit enemyUnit;
        MeshRenderer[] meshRenderers;
        SkinnedMeshRenderer[] skinnedMeshRenderers;

        //Så lenge spillet går
        while (true)
        {
            //Vent 0.5 sek
            yield return new WaitForSeconds(0.1f);

            //For hvert lag
            for (int team = 0; team < GameManager.teams; team++)
            {
                //Venter på neste fixed update, tar bare et lag om gangen
                yield return new WaitForFixedUpdate();

                //Glemmer hvem som var synlig forrige gang
                GameManager.visibleEnemies[team].Clear();

                //For hver unit på laget
                for (int i = 0; i < GameManager.units[team].Count; i++)
                {
                    //Tar bare 10 om gangen
                    if (i % 10 == 0)
                        yield return new WaitForFixedUpdate();
                    
                    friendlyUnit = GameManager.units[team][i];

                    //Finner alle units innen for en radius av optics * 2, den lengste distansen man kan se noen på
                    unitsInRange = Physics.OverlapSphere(friendlyUnit.transform.position, friendlyUnit.optics * 2, layermask);

                    //For hver unit
                    for (int n = 0; n < unitsInRange.Length; n++)
                    {
                        enemyUnit = unitsInRange[n].GetComponent<Unit>();

                        //Om det er en fiende
                        if (enemyUnit.team != friendlyUnit.team)
                        {
                            //Finner distansem man kan se den på
                            visibleDistance = friendlyUnit.optics * enemyUnit.visibility.GetVisibility();

                            //Om den er innafor
                            if (Vector3.Distance(enemyUnit.transform.position, friendlyUnit.transform.position) < visibleDistance)
                            {
                                //Ser om vi kan se den
                                RaycastHit hit;
                                if (Physics.Raycast(friendlyUnit.transform.position, enemyUnit.transform.position - friendlyUnit.transform.position, out hit))
                                {
                                    //Om vi ser den
                                    if (hit.transform == enemyUnit.transform)
                                    {
                                        //Legger den til i listen over synlige units for dette laget
                                        if (!GameManager.visibleEnemies[team].Contains(enemyUnit))
                                            GameManager.visibleEnemies[team].Add(enemyUnit);
                                    }
                                    //Om den er bak røyk
                                    else if (hit.collider.tag == "Smoke")
                                    {
                                        //Den er vanskeligere å se
                                        visibleDistance /= 2;
                                        RaycastHit hit2;

                                        //Ser om vi kan se den
                                        if (Physics.Raycast(hit.transform.position, hit.transform.position - friendlyUnit.transform.position, out hit2))
                                        {
                                            //Om vi kan se den
                                            if (hit2.transform == enemyUnit.transform)
                                            {
                                                //Om den er nærme nok
                                                if (Vector3.Distance(friendlyUnit.transform.position, enemyUnit.transform.position) < visibleDistance)
                                                {
                                                    //Legger den til i listen over synlige units for dette laget
                                                    if (!GameManager.visibleEnemies[team].Contains(enemyUnit))
                                                        GameManager.visibleEnemies[team].Add(enemyUnit);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //Om dette er laget vi styrer
                if (team == (int)GameManager.team)
                {
                    //For hvert lag
                    for (int t = 0; t < GameManager.teams; t++)
                    {
                        //Uten om vårt eget
                        if (t == team)
                            continue;

                        //For hver unit
                        for (int i = 0; i < GameManager.units[t].Count; i++)
                        {
                            if (i % 10 == 0)
                                yield return new WaitForFixedUpdate();
                            
                            //Henter litt komponenter
                            enemyUnit = GameManager.units[t][i];
                            meshRenderers = enemyUnit.GetComponentsInChildren<MeshRenderer>();
                            skinnedMeshRenderers = enemyUnit.GetComponentsInChildren<SkinnedMeshRenderer>();

                            //Om laget vårt kan se den
                            if (GameManager.visibleEnemies[team].Contains(enemyUnit))
                            {
                                //Gjør den synlig
                                for (int n = 0; n < meshRenderers.Length; n++)
                                {
                                    meshRenderers[n].enabled = true;
                                }
                                for (int n = 0; n < skinnedMeshRenderers.Length; n++)
                                {
                                    skinnedMeshRenderers[n].enabled = true;
                                }
                            }
                            //Om laget vårt ikke kan se den
                            else
                            {
                                //Gjør den usynlig
                                for (int n = 0; n < meshRenderers.Length; n++)
                                {
                                    meshRenderers[n].enabled = false;
                                }
                                for (int n = 0; n < skinnedMeshRenderers.Length; n++)
                                {
                                    skinnedMeshRenderers[n].enabled = false;
                                }
                            }
                        }
                    }
                    
                }
            }
        }
    }
}