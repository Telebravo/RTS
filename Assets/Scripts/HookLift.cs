using UnityEngine;
using System.Collections;

public class HookLift : MonoBehaviour
{
    bool selected = false;
    bool cursorControll = false;
    Unit unit;
    CHookliftable target;
    bool carrynig = false;
    Vector3 unloadPoint;
    bool unloading = false;

    void Start ()
    {
        unit = GetComponent<Unit>();
    }
	void Update ()
    {
        if (cursorControll)
        {
            GameManager.controlls.SetCursor(Cursors.Select);
            cursorControll = false;
        }
        if (selected && !carrynig)
        {
            if (GameManager.controlls.holdOverObject != null && Input.GetKey(KeyCode.LeftControl))
            {
                CHookliftable hookliftable = GameManager.controlls.holdOverObject.GetComponent<CHookliftable>();
                if (hookliftable != null)
                {
                    if (!hookliftable.onTruck)
                    {
                        GameManager.controlls.SetCursor(Cursors.Load);
                        cursorControll = true;

                        if(Input.GetMouseButtonDown(1))
                        {
                            target = hookliftable;
                            unit.cMoveable.SetTarget(target.transform.position);
                        }
                    }
                }
            }
        }
        if(target != null)
        {
            if(target.onTruck)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    GameManager.controlls.SetCursor(Cursors.Unload);
                    cursorControll = true;
                    if (Input.GetMouseButtonDown(1))
                    {
                        unloading = true;
                        unloadPoint = GameManager.controlls.mouseWorldPosition;
                        unit.cMoveable.SetTarget(unloadPoint);
                    }
                }
                if(unloading)
                {
                    if(Vector3.Distance(transform.position, unloadPoint) < 1)
                    {
                        target.transform.parent = transform.parent;
                        target.transform.Translate(Vector3.down * (target.lockPossition.y - target.onGroundY));
                        target.onTruck = false;
                        unloading = false;
                        target = null;
                        carrynig = false;
                    }
                }
            }
            else if (Vector3.Distance(transform.position, target.transform.position) < 1)
            {
                target.transform.parent = transform;
                target.transform.localPosition = target.lockPossition;
                target.transform.localRotation = new Quaternion();
                target.onTruck = true;
                carrynig = true;
            }
        }
    }
	
	public void Select(bool selected)
    {
        this.selected = selected;
        Debug.Log(selected);
    }
}
