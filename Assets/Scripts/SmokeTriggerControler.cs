using UnityEngine;
using System.Collections;

public class SmokeTriggerControler : MonoBehaviour {

    public GameObject røyk;
    private ParticleSystem røyksystem;
    private float proposjanlitetskonstantFraFørsteGranat;
    public float durrationCollider;

    public SphereCollider largeCollider;
    public SphereCollider smallCollider;

    public float largeColliderGrowth;
    public float smallColliderGrowth;

    public float largeColliderMaxRad;
    public float smallColliderMaxRad;

    public float largeColliderStartSize;
    public float smallColliderStartSize;

    private float startTime;
    // Use this for initialization

    void Start () {
        startTime = Time.time;
        røyksystem = røyk.GetComponent<ParticleSystem>();//Denne røykgranaten sit particle system
        proposjanlitetskonstantFraFørsteGranat = (røyksystem.startSpeed * røyksystem.startLifetime) / 20; //StartSpeed og startLiftetime bestemmer radiusen til røykboblen
	}
	
	// Update is called once per frame
	void Update () {
	if(Time.time > 2 + startTime)//Når granaten sprenger
        {
            if(largeCollider.radius < largeColliderMaxRad * proposjanlitetskonstantFraFørsteGranat)//Hvis den ikke er så stor røykskyen blir
            {
                largeCollider.radius = (Time.time - startTime - 2 + largeColliderStartSize) * largeColliderGrowth * proposjanlitetskonstantFraFørsteGranat;//Setter collideren lik tiden siden den begynte å ryke + størelsen den starter med * hvor mye den skal øke med * en proposjonalitet med størelsen på denne røykgranaten
            }
            if(smallCollider.radius < smallColliderMaxRad * proposjanlitetskonstantFraFørsteGranat)//Hvis den ikke er så stor røykskyen blir
            {
                smallCollider.radius = (Time.time - startTime - 2 + smallColliderStartSize) * smallColliderGrowth * proposjanlitetskonstantFraFørsteGranat;//Setter collideren lik tiden siden den begynte å ryke + størelsen den starter med * hvor mye den skal øke med * en proposjonalitet med størelsen på denne røykgranaten
            }
        }
    if(Time.time > startTime + durrationCollider)
        {
            GameObject.Destroy(smallCollider);
            GameObject.Destroy(largeCollider);
        }
	}
    
}
