using UnityEngine;
using System.Collections;

public class ShotHandler : MonoBehaviour
{
    public float Speed;
    public GameObject Explosion;
    public float Range;
    public float Damage;
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
            Explode();
        }
    }
    void Explode()
    {
        Instantiate(Explosion, transform.position, transform.rotation);
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, ExplosionRadius);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            CHealth health = hitColliders[i].GetComponent<CHealth>();
            if(health != null)
            {
                health.Damage(Damage, DamageType.Explosive, (hitColliders[i].transform.position - transform.position).magnitude);
                print((hitColliders[i].transform.position - transform.position).magnitude);
            }
        }
        Destroy(gameObject);
    }
}