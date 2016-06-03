using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PengePasser : MonoBehaviour {

    public float Penger;
    public float PengeØkningPerSek = 1;
    public float StartPenger = 0;
    public float PengerPerØkning = 1;
    public Text AntallPenger;

    private float LastPenge;
    private float SekPerPenge;
	// Use this for initialization
	void Start () {
        Penger = StartPenger;
        SekPerPenge = 1 / PengeØkningPerSek;
	}
	
	// Update is called once per frame
	void Update () {
        if(Time.time >= LastPenge + SekPerPenge){
            Penger += PengerPerØkning;
            LastPenge = Time.time;
            AntallPenger.text  = Penger.ToString() + " $";
        }
	
	}
}
