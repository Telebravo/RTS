using UnityEngine;
using System.Collections;

[AddComponentMenu("Unit/CTransportable")]
[RequireComponent(typeof(Unit))]
public class CTransportable : MonoBehaviour
{
    [HideInInspector]
    public Unit unit;
    private Transport transport;

    private bool cursorCotroll;
    private Vector3 targetPosition;

    private Vector3 targetEmbarkPosition;

    public Vector3 lockPosition;

	void Start ()
    {
        unit = GetComponent<Unit>();
	}

	void Update ()
    {
        if(cursorCotroll)
        {
            GameManager.controlls.SetCursor(Cursors.Arrow);
            cursorCotroll = false;
        }

	    if(unit.cSelectable.selected && Input.GetKey(KeyCode.LeftControl) && GameManager.controlls.holdOverObject != null)
        {
            transport = GameManager.controlls.holdOverObject.GetComponent<Transport>();
            if (transport != null)
            {
                if(transport.CanAdd(this) && transport.unit.team == this.unit.team)
                {
                    GameManager.controlls.SetCursor(Cursors.Load);
                    cursorCotroll = true;

                    if(Input.GetMouseButtonDown(1))
                    {
                        targetPosition = transport.embarkPosition.position;
                        unit.cMoveable.SetTarget(targetPosition);
                    }
                }
                else
                {
                    GameManager.controlls.SetCursor(Cursors.LoadBlocked);
                    cursorCotroll = true;
                }
            }
        }
        if (targetPosition != Vector3.zero && transport != null)
        {
            if (targetPosition == unit.cMoveable.GetCurrentTarget())
            {
                targetEmbarkPosition.Set(transform.position.x, transport.embarkPosition.position.y, transform.position.z);
                if (Vector3.Distance(targetEmbarkPosition, transport.embarkPosition.position) < 1)
                {
                    transport.Add(this);
                    targetPosition = Vector3.zero;
                    transport = null;
                }
                else if(!transport.CanAdd(this))
                {
                    targetPosition = Vector3.zero;
                    transport = null;
                    unit.cMoveable.ClearTarget();
                }
            }
            else
            {
                targetPosition = Vector3.zero;
                transport = null;
            }
        }
	}

    public void OnLoad()
    {
        unit.GetComponent<InfantryShooting>().enabled = false;
        unit.cMoveable.canMove = false;
        unit.cMoveable.ClearTarget();
        unit.cMoveable.DisableUpdate();
        GameManager.controlls.Deselect(unit);
        unit.cSelectable.enabled = false;
    }

    public void OnUnload()
    {
        unit.GetComponent<InfantryShooting>().Disable();
        unit.cMoveable.canMove = true;
        unit.cMoveable.Warp(transform.position);
        unit.cMoveable.EnableUpdate();
        unit.cSelectable.enabled = true;
    }
}