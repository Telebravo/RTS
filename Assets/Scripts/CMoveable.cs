using UnityEngine;
using System.Collections;

public class CMoveable : MonoBehaviour
{
    Unit unit;
    NavMeshAgent agent;

	void Start ()
    {
        unit = GetComponent<Unit>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = unit.movementSpeed;
        agent.angularSpeed = unit.rotationSpeed;
	}
	
	void SetTarget (Vector3 point)
    {
        agent.SetDestination(point);
	}
}
