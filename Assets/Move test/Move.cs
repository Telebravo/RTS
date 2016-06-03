using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour
{
    NavMeshAgent agent;

	void Start ()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(new Vector3(0, 3.5f, -15));
	}
	
	void Update ()
    {
        
	}
    void OnMouseDown()
    {
        Vector3 MousePositionInPixels = Input.mousePosition;
        Vector3 ClickedPoint = Camera.main.ScreenToWorldPoint(MousePositionInPixels);

        agent.SetDestination(ClickedPoint);

    }

    

    void FixedUpdate()
    {

    }
}
