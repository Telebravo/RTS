using UnityEngine;
using System.Collections;

[AddComponentMenu("Unit/ArtileriShoter")]
[RequireComponent(typeof(Unit))]
public class ArtileriShoter : MonoBehaviour
{
    public float range = 1;
    public GameObject Shell;
    public Transform BarrelRotation;
    public float barrelRotationSpeed;

    private float Utgangshatighet;
    private float AirTime;
    private Unit unit;
    private float Dist;
    private float Dist1;
    private float SkuddVinkel;
    Ray ray;
    RaycastHit hit;
    Vector3 retning;
    Vector3 retningS;

    // Use this for initialization
    void Start()
    {
        unit = GetComponent<Unit>();
        /*
         * print(unit.weapon.range);
         * print((float)(unit.weapon.range * Physics.gravity.y) / (Mathf.Sqrt(2) / 2));
         * print((Mathf.Sqrt(2) / 2));
         * print((float)(unit.weapon.range * Physics.gravity.y));
        */
        Utgangshatighet = Mathf.Sqrt((float)(unit.weapon.range * -Physics.gravity.y)); // Sin(2 * 45) = 1
        //Debug.Log("Utgangshastighet: " + Utgangshatighet);

        retning = Vector3.down;
        
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0) && unit.cSelectable.selected)
        {
            //print("Fire1");
            //Finner posisjon i verden utifra musepeker
            ray = CameraControlls.camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 10000, 1, QueryTriggerInteraction.Ignore))
            {
                //print("Fire2");
                Dist = (hit.point - transform.position).magnitude;
                //Debug.Log("Distanse: " + Dist);

                if (Dist <= unit.weapon.range && Dist >= unit.weapon.minRange)
                {
                    /*
                     * print("Fire3");
                     * Debug.Log("Gravitasjon: " + Physics.gravity.y);
                     * Debug.Log("stuff: " + (float)(Dist * -Physics.gravity.y) / Mathf.Pow(Utgangshatighet, 2));
                    */

                    SkuddVinkel = (Mathf.Asin((float)(Dist * -Physics.gravity.y) / Mathf.Pow(Utgangshatighet, 2)));//Ikke hele formelen

                    if (SkuddVinkel < Mathf.PI / 2)//For å få riktig sinusverdi
                    {
                        SkuddVinkel = (Mathf.PI - SkuddVinkel);
                    }
                    SkuddVinkel = SkuddVinkel / 2;//Resten av formelen
                    retning = hit.point - unit.weapon.barrelEnd.position;
                    retning.y = 0;
                    retningS = retning.normalized * Mathf.Cos(SkuddVinkel);
                    retningS.y = Mathf.Sin(SkuddVinkel);
                    retningS = new Vector3(retningS.x - transform.rotation.x, 0, 0);
                }
            }
        }
        if (retning != Vector3.down)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(retning, Vector3.up), 45f * Time.deltaTime);

            //Om det er innafor
            if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(retning, Vector3.up)) < 1)
            {
                //Debug.Log("retningS og BarrelRotation: " + retningS + " og " + BarrelRotation.rotation.eulerAngles);

                BarrelRotation.localRotation = Quaternion.RotateTowards(BarrelRotation.localRotation, Quaternion.Euler( new Vector3(- SkuddVinkel * Mathf.Rad2Deg, 0, 0)),barrelRotationSpeed * Time.deltaTime);

                if (Quaternion.Angle(BarrelRotation.localRotation, Quaternion.Euler(new Vector3(-SkuddVinkel * Mathf.Rad2Deg, 0, 0))) < 5)
                {
                    retning = retning.normalized * Mathf.Cos(SkuddVinkel);
                    retning.y = Mathf.Sin(SkuddVinkel);

                    //Debug.Log("Skuddvinkel: " + SkuddVinkel * Mathf.Rad2Deg);

                    GameObject Kjell = Instantiate(Shell);
                    Kjell.GetComponent<ShotHandler>().AirTime = 99999;
                    Kjell.GetComponent<ShotHandler>().ammo = unit.weapon.ammo;
                    Kjell.transform.position = unit.weapon.barrelEnd.position;

                    Rigidbody rgbd = Kjell.GetComponent<Rigidbody>();
                    rgbd.useGravity = true;

                    rgbd.velocity = retning.normalized * Utgangshatighet;
                    /*
                     * Debug.Log("Fart: " + rgbd.velocity.ToString());
                     * Debug.Log("Fartu: " + Utgangshatighet);
                     * Debug.Log("Fartr: " + rgbd.velocity.magnitude);
                     * Debug.Log("Farty" + rgbd.velocity.y + ",  Fartxz" + Mathf.Sqrt(Mathf.Pow(rgbd.velocity.x, 2) + Mathf.Pow(rgbd.velocity.z, 2)));
                    */
                    //Kjell.GetComponent<ShotHandler>().AirTime  = Dist / new Vector2(rgbd.velocity.x, rgbd.velocity.z).magnitude;

                    retning = Vector3.down;
                }
            }
        }
    }
}