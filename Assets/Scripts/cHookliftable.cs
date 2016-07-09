using UnityEngine;
using System.Collections;

[AddComponentMenu("Unit/CHookliftable")]
[RequireComponent(typeof(Unit))]
public class CHookliftable : MonoBehaviour
{
    Unit unit;

    //Possjonen obektet skal ha når det er låst fast på et lasteplan elns
    public Vector3 lockPossition;
    //Krokfestet
    public Transform hookPoint;
    //Høyden fra bakken til senteret av objektet
    public float onGroundY;
    //Om det er på en lastebil ect
    public bool onTruck = false;
    //Om det for øyeblikket kan bli kroket
    public bool liftable = true;
    //NavMesh kollideren
    public NavMeshObstacle navMeshObstacle;

    //Ved start
    void Awake()
    {
        //Heter komponenter
        unit = GetComponent<Unit>();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
    }
    //Når objektet går inn på skjermen
    void OnBecameVisible()
    {
        //Sier ifra til folkz at det er synlig
        GameManager.controlls.SetVisible(unit, true);
    }
    //Når objektet går ut av skjermen
    void OnBecameInvisible()
    {
        //Ikke synlig lengere
        GameManager.controlls.SetVisible(unit, false);
    }
}
