using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {
    public bool select = false;
	// Use this for initialization
    void Selected()
    {
        Debug.Log("Shits Selected yo");
        select = true;
    }
    void Deselected()
    {
        Debug.Log("Shits Deselected yo");
        select = false; 
    }
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void Onclick()
    {
        if (select)
        {
            Deselected();
        }else
        {
            Selected();
        }
    }
}
