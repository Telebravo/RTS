using UnityEngine;
using System.Collections;

public class Se : MonoBehaviour {


    private SphereCollider maxLos;
    private Collider[] EnheterInnenforMaksLOS;
    private Unit unit;
    private string Lag;
    private float synligDist;

    //Unit layeret
    int unitLayer = 8;
    int layermask;

    void Awake()
    {
        layermask = 1 << unitLayer;
    }

    // Use this for initialization
    void Start () {
        unit = GetComponent<Unit>();
        Lag = GameManager.team;
        StartCoroutine(UpdateEnemies());
    }
	
	// Update is called once per frame
	void Update () {
    }
    IEnumerator UpdateEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            EnheterInnenforMaksLOS = Physics.OverlapSphere(transform.position, unit.optics * 2 , layermask);

            for (int i = 0; i < EnheterInnenforMaksLOS.Length; i++)
            {
                Debug.Log(EnheterInnenforMaksLOS[i].name);
                if (Lag != EnheterInnenforMaksLOS[i].tag)
                {
                    synligDist = unit.optics * EnheterInnenforMaksLOS[i].GetComponent<Visibility>().getVisibility();
                    Debug.Log("synligDist" + synligDist);
                    Debug.Log("dist: " + Vector3.Distance(EnheterInnenforMaksLOS[i].GetComponent<Transform>().position, transform.position));
                    if (Vector3.Distance( EnheterInnenforMaksLOS[i].GetComponent<Transform>().position , transform.position) > synligDist)
                    {
                        EnheterInnenforMaksLOS[i].GetComponentInParent<MeshRenderer>().enabled = false;
                    }
                    else
                    {
                        EnheterInnenforMaksLOS[i].GetComponentInParent<MeshRenderer>().enabled = true;
                    }
                }
            }
            
        }
    }
}
