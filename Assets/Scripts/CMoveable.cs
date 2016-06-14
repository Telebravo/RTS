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
        GetPath();
        if (!veichle)
            return;

        float maxRotation = 45f;

        Vector3 target = agent.steeringTarget;
        target.y = transform.position.y;

        Vector3 vDir = target - transform.position;

        float angle = Mathf.Atan2(vDir.z, vDir.x)*-Mathf.Rad2Deg+90;
        angle -= transform.rotation.y;
        angle = Mathf.Clamp(angle, -maxRotation, maxRotation);

        Debug.Log("Angle: " + angle.ToString());

        wheels[0].steerAngle = angle;
        wheels[1].steerAngle = angle;

        wheels[0].motorTorque = 0;
        wheels[1].motorTorque = 0;

        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].brakeTorque = 0;
        }
        if (vDir.magnitude > 10)
        {
            wheels[0].motorTorque = 200;
            wheels[1].motorTorque = 200;
        }
        else if (vDir.magnitude > 2)
        {
            wheels[0].motorTorque = 50;
            wheels[1].motorTorque = 50;
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
