using UnityEngine;
using System.Collections;

public class ShotHandler : MonoBehaviour
{
    public float Speed;
    public GameObject Explosion;
    public float Range;
    public Ammunition ammo;
    public float ExplosionRadius;
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
                CHealth health = other.GetComponent<CHealth>();
                if (health != null)
                {
                    health.Damage(ammo);
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
            CHealth health = hitColliders[i].GetComponent<CHealth>();
            if(health != null)
            {
                health.Damage(ammo, (hitColliders[i].bounds.ClosestPoint(transform.position) - transform.position).magnitude);
            }
        }
        Destroy(gameObject);
    }
}