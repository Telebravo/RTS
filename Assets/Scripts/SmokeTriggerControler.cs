using UnityEngine;
using System.Collections;

public class SmokeTriggerControler : MonoBehaviour
{
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
    void Start()
    {
        startTime = Time.time;
        røyksystem = røyk.GetComponent<ParticleSystem>();//Denne røykgranaten sit particle system
        proposjanlitetskonstantFraFørsteGranat = (røyksystem.startSpeed * røyksystem.startLifetime) / 20; //StartSpeed og startLiftetime bestemmer radiusen til røykboblen
    }

    // Update is called once per frame
    void Update()
    {
        //Om colliderene skal fjernes
        if (Time.time > startTime + durrationCollider)
        {
            if (smallCollider != null)
            {
                GameObject.Destroy(smallCollider);
                GameObject.Destroy(largeCollider);
            }
        }
        //Om ikke
        else
        {
            //Når granaten sprenger
            if (Time.time > 2 + startTime)
            {
                //Hvis den ikke er så stor røykskyen blir
                if (largeCollider.radius < largeColliderMaxRad * proposjanlitetskonstantFraFørsteGranat)
                {
                    //Setter collideren lik tiden siden den begynte å ryke + størelsen den starter med * hvor mye den skal øke med * en proposjonalitet med størelsen på denne røykgranaten
                    largeCollider.radius = (Time.time - startTime - 2 + largeColliderStartSize) * largeColliderGrowth * proposjanlitetskonstantFraFørsteGranat;
                }

                //Hvis den ikke er så stor røykskyen blir
                if (smallCollider.radius < smallColliderMaxRad * proposjanlitetskonstantFraFørsteGranat)
                {
                    //Setter collideren lik tiden siden den begynte å ryke + størelsen den starter med * hvor mye den skal øke med * en proposjonalitet med størelsen på denne røykgranaten
                    smallCollider.radius = (Time.time - startTime - 2 + smallColliderStartSize) * smallColliderGrowth * proposjanlitetskonstantFraFørsteGranat;
                }
            }   
        }
    }
}
