using UnityEngine;
using System.Collections;

public class CMoveable : MonoBehaviour
{
    Unit unit;
    NavMeshAgent agent;
    public WheelCollider[] wheels;
    public bool veichle;

    LineRenderer line;

    void Start ()
    {
        unit = GetComponent<Unit>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = unit.movementSpeed;
        agent.angularSpeed = unit.rotationSpeed;
        if (veichle)
        {
            agent.updatePosition = false;
            agent.updateRotation = false;
        }
        line = GetComponent<LineRenderer>();
    }
	void Update()
    {
        if (!veichle)
            return;

        float maxRotation = 45f;

        Vector3 target = agent.nextPosition;
        target.y = transform.position.y;

        Vector3 vDir = target - transform.position;

        float angle = Mathf.Atan2(vDir.z, vDir.x)*-Mathf.Rad2Deg -90;
        angle = Vector3.Angle(transform.position, target);
        angle -= transform.rotation.y;
        
        Debug.Log("Angle n: " + angle.ToString());
        angle = Mathf.Clamp(angle, -maxRotation, maxRotation);
        Debug.Log("Angle c: " + angle.ToString());
        
        Debug.DrawRay(wheels[0].transform.position, vDir.normalized * 5);
        Debug.DrawRay(wheels[1].transform.position, vDir.normalized * 5);
        Debug.DrawLine(transform.position, target);

        wheels[0].steerAngle = angle;
        wheels[1].steerAngle = angle;

        wheels[2].motorTorque = 0;
        wheels[3].motorTorque = 0;

        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].brakeTorque = 0;
        }
        if (vDir.magnitude > 10)
        {
            wheels[2].motorTorque = 200;
            wheels[3].motorTorque = 200;
        }
        else if (vDir.magnitude > 2)
        {
            wheels[2].motorTorque = 50;
            wheels[3].motorTorque = 50;
        }
        else
        {
            wheels[0].steerAngle = 0;
            wheels[1].steerAngle = 0;
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].brakeTorque = 200;
            }
        }

        Vector3 pos;
        Quaternion rot;
        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].GetWorldPose(out pos, out rot);
            wheels[i].transform.GetChild(0).position = pos;
            wheels[i].transform.GetChild(0).rotation = rot;
        }
    }
	public void SetTarget (Vector3 point)
    {
        agent.SetDestination(point);
    }
}
