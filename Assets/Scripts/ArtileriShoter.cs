using UnityEngine;
using System.Collections;


public class ArtileriShoter : MonoBehaviour
{
    public float range = 1;
    public GameObject Shell;
    public Transform BarrelEnd;
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
    new Camera camera;
    Vector3 retning;
    Vector3 retningS;
    bool aligned = false;
    // Use this for initialization
    void Start()
    {
        unit = GetComponent<Unit>();
        print(unit.weapon.range);
        print((float)(unit.weapon.range * Physics.gravity.y) / (Mathf.Sqrt(2) / 2));
        print((Mathf.Sqrt(2) / 2));
        print((float)(unit.weapon.range * Physics.gravity.y));
        Utgangshatighet = Mathf.Sqrt((float)(unit.weapon.range * -Physics.gravity.y)); // Sin(2 * 45) = 1
        Debug.Log("Utgangshastighet: " + Utgangshatighet);

        camera = Camera.main;
        retning = Vector3.down;
        
    }
    // Update is called once per frame
    void Update()
    {


        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0) && unit.cSelectable.selected)
        {
            print("Fire1");
            //Finner posisjon i verden utifra musepeker
            ray = CameraControlls.camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 10000, 1, QueryTriggerInteraction.Ignore))
            {
                print("Fire2");
                Dist = (hit.point - transform.position).magnitude;
                //Dist1 = Mathf.Pow( Dist , 2 ) + Mathf.Pow( BarrelEnd.position.y , 2);
                Debug.Log("Distanse: " + Dist);
                if (Dist <= unit.weapon.range && Dist >= unit.weapon.minRange)
                {
                    print("Fire3");
                    Debug.Log("Gravitasjon: " + Physics.gravity.y);
                    Debug.Log("stuff: " + (float)(Dist * -Physics.gravity.y) / Mathf.Pow(Utgangshatighet, 2));

                    SkuddVinkel = (Mathf.Asin((float)(Dist * -Physics.gravity.y) / Mathf.Pow(Utgangshatighet, 2)));//Ikke hele formelen

                    if (SkuddVinkel < Mathf.PI / 2)//For å få riktig sinusverdi
                    {
                        SkuddVinkel = (Mathf.PI - SkuddVinkel);
                    }
                    SkuddVinkel = SkuddVinkel / 2;//Resten av formelen
                    //SkuddVinkel = Mathf.Deg2Rad * 45;
                    retning = hit.point - BarrelEnd.position;
                    //float retningLengde = retning.magnitude;
                    retning.y = 0;
                    //transform.rotation = Quaternion.LookRotation(retning, Vector3.up);
                    aligned = false;
                    retningS = retning.normalized * Mathf.Cos(SkuddVinkel);
                    retningS.y = Mathf.Sin(SkuddVinkel);
                    retningS = new Vector3(retningS.x - transform.rotation.x,/* retningS.y - transform.rotation.y*/0,/* retningS.z - transform.rotation.z*/0);
                }
            }
        }
        if (retning != Vector3.down)
        {

            if (!aligned)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(retning, Vector3.up), 45f * Time.deltaTime);
                Quaternion towerRotation = Quaternion.Euler(-180 + SkuddVinkel * Mathf.Rad2Deg, 90, 0);
            }
            //Om det er innafor
            if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(retning, Vector3.up)) < 1 || aligned)
            {
                aligned = true;
                Debug.Log("retningS og BarrelRotation: " + retningS + " og " + BarrelRotation.rotation.eulerAngles);
                //BarrelRotation.rotation = Quaternion.LookRotation(retningS, Vector3.up) * Quaternion.Inverse(transform.rotation);
                BarrelRotation.localRotation = Quaternion.RotateTowards(BarrelRotation.localRotation, Quaternion.Euler( new Vector3(- SkuddVinkel * Mathf.Rad2Deg, 0, 0)),barrelRotationSpeed * Time.deltaTime);//It works, eller i hvertfall nærme nok tror jeg

                //BarrelRotation.rotation = Quaternion.RotateTowards(BarrelRotation.rotation, Quaternion.LookRotation(retningS,Vector3.up) * Quaternion.Inverse(transform.rotation), 45f * Time.deltaTime);
                //Quaternion towerRotation = Quaternion.Euler(-180 + SkuddVinkel * Mathf.Rad2Deg, 90, 0);
                /*
                Debug.Log("Barrel Rotation før:" + BarrelRotation.rotation);
                BarrelRotation.Rotate(Vector3.right, 45f, Space.Self);
                BarrelRotation.rotation.lo
                //BarrelRotation.rotation = Quaternion.RotateTowards(BarrelRotation.rotation, towerRotation, barrelRotationSpeed * Time.deltaTime);
                Debug.Log("Barrel Rotation etter:" + BarrelRotation.rotation);*/

                if (Quaternion.Angle(BarrelRotation.localRotation, Quaternion.Euler(new Vector3(-SkuddVinkel * Mathf.Rad2Deg, 0, 0))) < 5)
                {
                    retning = retning.normalized * Mathf.Cos(SkuddVinkel);
                    retning.y = Mathf.Sin(SkuddVinkel);

                    //Debug.Log("Skudd vektor: " + retning);
                    //Debug.Log("SkuddVektorlengde: " + retning.magnitude);
                    Debug.Log("Skuddvinkel: " + SkuddVinkel * Mathf.Rad2Deg);

                    GameObject Kjell = Instantiate(Shell);
                    Kjell.GetComponent<ShotHandler>().AirTime = 99999;
                    Kjell.transform.position = BarrelEnd.position;

                    Rigidbody rgbd = Kjell.GetComponent<Rigidbody>();
                    rgbd.useGravity = true;

                    rgbd.velocity = retning.normalized * Utgangshatighet;

                    Debug.Log("Fart: " + rgbd.velocity.ToString());
                    Debug.Log("Fartu: " + Utgangshatighet);
                    Debug.Log("Fartr: " + rgbd.velocity.magnitude);
                    Debug.Log("Farty" + rgbd.velocity.y + ",  Fartxz" + Mathf.Sqrt(Mathf.Pow(rgbd.velocity.x, 2) + Mathf.Pow(rgbd.velocity.z, 2)));
                    //Kjell.GetComponent<ShotHandler>().AirTime  = Dist / new Vector2(rgbd.velocity.x, rgbd.velocity.z).magnitude;

                    retning = Vector3.down;
                    //aligned = false;
                }
            }
        }
    }
}