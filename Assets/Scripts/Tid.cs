using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tid : MonoBehaviour {

    public Text tekstTid;
    public int tid;
    private int minute;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        tid = (int)Time.time;
        tekstTid.text = Mathf.Floor(Time.time/60) + ":" + tid%60;
	}
}
