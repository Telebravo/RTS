using UnityEngine;
using System.Collections;

public class SpawnScript : MonoBehaviour {

    public GameObject preFab;
    public GameObject building;
    

    public void Onclick()
    {
        
        GameObject unit = Instantiate<GameObject>(preFab);
        unit.transform.position = building.transform.position + new Vector3(6, 0, 0);
        Debug.Log("Funker");
    }
}
