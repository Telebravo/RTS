using UnityEngine;
using System.Collections;

public class ShotHandler : MonoBehaviour
{
    public float Speed;
    public GameObject Explosion;

    [HideInInspector]
    public Ammunition ammo;
    [HideInInspector]
    public float Range;
    [HideInInspector]
    public float AirTime;
    
    private float StartTime;
    private Rigidbody rigbd;

	void Start ()
    {
        StartTime = Time.time;
	}
	
	void Update ()
    {
        if (Time.time >= StartTime + AirTime)
        {
            Explode();
        }
	
	}
    void OnTriggerEnter(Collider other)
    {
        if(!other.isTrigger)
        {
            if (ammo.damageType == DamageType.Kinetic)
            {
                Unit unit = other.GetComponent<Unit>();
                if (unit != null)
                {
                    unit.Damage(ammo);
                    GameObject explotionObject = (GameObject)Instantiate(Explosion, transform.position, transform.rotation);
                    explotionObject.transform.localScale = Vector3.one * ammo.explosionRadius * 2;
                    Destroy(gameObject);
                }
            }
            else if (ammo.damageType == DamageType.Explosive)
            {
                Explode();
            }
        }
    }
    void Explode()
    {
        GameObject explotionObject = (GameObject)Instantiate(Explosion, transform.position, transform.rotation);
        explotionObject.transform.localScale = Vector3.one * ammo.explosionRadius * 2;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, ammo.explosionRadius);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            Unit unit = hitColliders[i].GetComponent<Unit>();
            if(unit != null)
            {
                unit.Damage(ammo, (hitColliders[i].bounds.ClosestPoint(transform.position) - transform.position).magnitude);
            }
        }
        Destroy(gameObject);
    }
}