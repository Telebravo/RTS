using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(Unit))]
public class Transport : MonoBehaviour
{
    [HideInInspector]
    public Unit unit;
    public List<Transform> slots;
    public List<CTransportable> items;
    public Transform embarkPosition;

    private bool stopping = false;

	void Awake ()
    {
        unit = GetComponent<Unit>();
        items = new List<CTransportable>();
	}
    void Update()
    {
        if(unit.cSelectable.selected)
        {
            if(Input.GetKeyDown(KeyCode.U))
            {
                unit.cMoveable.ClearTarget();
                stopping = true;
            }
        }
        if(stopping)
        {
            if(unit.cMoveable.GetCurrentTarget() != Vector3.zero)
            {
                stopping = false;
            }
            if(unit.cMoveable.GetVelocity().magnitude < 1)
            {
                stopping = false;

                foreach(CTransportable item in items.ToArray())
                {
                    Remove(item);
                }
            }
        }
    }
    public bool CanAdd(CTransportable unit)
    {
        return items.Count < slots.Count;
    }
    public bool Add(CTransportable item)
    {
        if (items.Count == slots.Count || items.Contains(item))
            return false;

        item.OnLoad();
        items.Add(item);
        item.transform.SetParent(this.transform);
        item.transform.position = slots[items.Count - 1].position + item.lockPosition;

        return true;
    }
    public void Remove(CTransportable item)
    {
        if (items.Contains(item))
        {
            item.transform.position = embarkPosition.position;
            item.transform.SetParent(this.transform.parent);
            items.Remove(item);
            item.OnUnload();
        }
    }
}
