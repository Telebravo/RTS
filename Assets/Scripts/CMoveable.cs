using UnityEngine;
using System.Collections;

public class CMoveable : MonoBehaviour
{
    //Uniten
    Unit unit;
    //Den magiske NavMeshAgenten
    NavMeshAgent agent;

    //Ved start
	void Start ()
    {
        //Henter komponenter
        unit = GetComponent<Unit>();
        agent = GetComponent<NavMeshAgent>();
        //Setter bevegelse- og rotasjonsfarten
        agent.speed = unit.movementSpeed;
        agent.angularSpeed = unit.rotationSpeed;
	}
	//Om noen gir oss en lyd om et sted å være
	public void SetTarget (Vector3 point)
    {
        //Ja, så turer vi vel dit
        agent.SetDestination(point);
	}
    //Om vi ikke vil dit likevel
    public void ClearTarget()
    {
        //Så får vi vel nøye oss med det også
        agent.ResetPath();
    }
    public void Warp(Vector3 position)
    {
        agent.Warp(position);
    }
    public void DisableUpdate()
    {
        agent.updatePosition = false;
        agent.updateRotation = false;
    }
    public void EnableUpdate()
    {
        agent.updatePosition = true;
        agent.updateRotation = true;
    }
}
