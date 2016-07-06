using UnityEngine;
using System.Collections;

public class Ting : MonoBehaviour {
    NavMeshAgent agent;
	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(new Vector3(200, 0.5f, 150));
	
	}
	
	// Update is called once per frame
	void Update () {
        
	}
    void OnMouseDown()
    {
        Vector3 MousePositionInPixles = Input.mousePosition;
        Vector3 ClickedPoint = Camera.main.ScreenToWorldPoint(MousePositionInPixles);
        agent.SetDestination(ClickedPoint);
        Debug.Log(MousePositionInPixles);
        Debug.Log(ClickedPoint);
    }
    void FixedUpdate()
    {

    }
}
