using UnityEngine;
using System.Collections;

[AddComponentMenu("Unit/CMoveable")]
[RequireComponent(typeof(Unit))]
[RequireComponent(typeof(NavMeshAgent))]
public class CMoveable : MonoBehaviour
{
    //Uniten
    Unit unit;
    //Den magiske NavMeshAgenten
    NavMeshAgent agent;
    //Om den skal ha lov til å flytte
    public bool canMove = true;
    //Dit vi er på vei
    private Vector3 currentTarget;

    //Ved start
	void Start ()
    {
        //Henter komponenter
        unit = GetComponent<Unit>();
        agent = GetComponent<NavMeshAgent>();
        //Setter bevegelse- og rotasjonsfarten
        agent.speed = unit.movementSpeed;
        agent.angularSpeed = unit.rotationSpeed;

        currentTarget = Vector3.zero;

    }

	//Om noen gir oss en lyd om et sted å være
    /// <summary>
    /// Sets the nav mesh agents destination
    /// </summary>
    /// <param name="point">Target destination in world space</param>
	public void SetTarget (Vector3 point)
    {
        if (canMove)
        {
            //Ja, så turer vi vel dit
            currentTarget = point;
            agent.SetDestination(point);
        }
	}
    /// <summary>
    /// Gets the nav mesh agents current target position
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCurrentTarget()
    {
        return currentTarget;
    }

    //Om vi ikke vil dit likevel
    /// <summary>
    /// Clears the nav mesh agents path
    /// </summary>
    public void ClearTarget()
    {
        //Så får vi vel nøye oss med det også
        agent.ResetPath();
        currentTarget = Vector3.zero;
    }

    /// <summary>
    /// Warps to the new position.
    /// </summary>
    /// <param name="position">New position</param>
    /// <param name="force">Whether to ignore cMoveable.canMove. Default: false</param>
    public void Warp(Vector3 position, bool force = false)
    {
        if (canMove || force)
        {
            agent.Warp(position);
        }
    }

    /// <summary>
    /// Causes the transform not to be synchronized with the nav mesh agent
    /// </summary>
    public void DisableUpdate()
    {
        agent.updatePosition = false;
        agent.updateRotation = false;
    }
    /// <summary>
    /// Causes the transform to be synchronized with the nav mesh agent
    /// </summary>
    public void EnableUpdate()
    {
        agent.updatePosition = true;
        agent.updateRotation = true;
    }

    public Vector3 GetVelocity()
    {
        return agent.velocity;
    }
}
