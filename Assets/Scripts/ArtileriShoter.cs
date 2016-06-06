﻿using UnityEngine;
using System.Collections;


public class ArtileriShoter : MonoBehaviour {
    public float range = 1;
    public GameObject Shell;
    public Transform BarrelEnd;

    private float Utgangshatighet;
    private float AirTime;
    private Unit unit;
    private float Dist;
    private float SkuddVinkel;
    Ray ray;
    RaycastHit hit;
    new Camera camera;
    // Use this for initialization
    void Start () {
        unit = GetComponent<Unit>();
        print(unit.weapon.range);
        print((float)(unit.weapon.range * Physics.gravity.y) / (Mathf.Sqrt(2) / 2));
        print((Mathf.Sqrt(2) / 2));
        print((float)(unit.weapon.range * Physics.gravity.y));
        Utgangshatighet = Mathf.Sqrt( (float) (unit.weapon.range * -Physics.gravity.y) /( Mathf.Sqrt(2)/2));
        Debug.Log("Utgangshastighet: " + Utgangshatighet);

        camera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0) && unit.cSelectable.selected)
        {
            print("Fire1");
            //Finner posisjon i verden utifra musepeker
            ray = CameraControlls.camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 10000, 1, QueryTriggerInteraction.Ignore))
            {
                print("Fire2");
                Dist = (hit.point - transform.position).magnitude;
                Debug.Log("Distanse: " + Dist);
                if(Dist <= unit.weapon.range)
                {
                    print("Fire3");
                    Debug.Log("Gravitasjon: " + Physics.gravity.y);
                    Debug.Log("stuff: " + (float)(Dist * -Physics.gravity.y) / Mathf.Pow(Utgangshatighet, 2));

                    SkuddVinkel = Mathf.Asin((float)(Dist * -Physics.gravity.y) / Mathf.Pow(Utgangshatighet, 2));

                    Vector3 retning = hit.point - transform.position;
                    retning.y = 0;
                    transform.rotation = Quaternion.LookRotation(retning, Vector3.up);
                    retning.y = Mathf.Sin(SkuddVinkel) * retning.magnitude;


                    Debug.Log("Skudd vektor: " + retning);
                    Debug.Log("Skuddvinkel: "+SkuddVinkel);

                    GameObject Kjell = Instantiate(Shell);
                    Kjell.GetComponent<ShotHandler>().AirTime = 99999;
                    Kjell.transform.position = BarrelEnd.position;

                    Rigidbody rgbd = Kjell.GetComponent<Rigidbody>();
                    rgbd.useGravity = true;

                    rgbd.velocity = retning.normalized * Utgangshatighet;

                    Debug.Log("Fart: " + rgbd.velocity.ToString());
                    Debug.Log("Fartu: " + Utgangshatighet);
                    Debug.Log("Fartr: " + rgbd.velocity.magnitude);
                    //Kjell.GetComponent<ShotHandler>().AirTime  = Dist / new Vector2(rgbd.velocity.x, rgbd.velocity.z).magnitude;

                }    
            }
        }
	
	}
}
