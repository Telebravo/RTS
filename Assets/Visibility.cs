using UnityEngine;
using System.Collections;

public class Visibility : MonoBehaviour {


    public bool iStilling;
    public bool iBevegelse;

    private float synlighet;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public float getVisibility()
    {
        synlighet = 100 - GetComponent<Unit>().camo;
        synlighet = synlighet / 100;
        if (iStilling) { synlighet = synlighet / 2; }
        if (iBevegelse) { synlighet = synlighet * 2; }


        return synlighet;
    }

}
