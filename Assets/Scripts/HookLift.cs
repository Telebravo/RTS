using UnityEngine;
using System.Collections;

public class HookLift : MonoBehaviour
{
    bool selected = false;
    bool cursorControll = false;
    Unit unit;
    cHookliftable target;
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
            GameManager.cameraControlls.SetCursor(Cursors.Select);
            cursorControll = false;
        }
        if (selected && !carrynig)
        {
            if (CameraControlls.holdOverObject != null && Input.GetKey(KeyCode.LeftControl))
            {
                cHookliftable hookliftable = CameraControlls.holdOverObject.GetComponent<cHookliftable>();
                if (hookliftable != null)
                {
                    if (!hookliftable.onTruck)
                    {
                        GameManager.cameraControlls.SetCursor(Cursors.Load);
                        cursorControll = true;

                        if(Input.GetMouseButtonDown(1))
                        {
                            target = hookliftable;
                            unit.cMoveable.SendMessage("SetTarget", target.transform.position);
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
                    GameManager.cameraControlls.SetCursor(Cursors.Unload);
                    cursorControll = true;
                    if (Input.GetMouseButtonDown(1))
                    {
                        unloading = true;
                        unloadPoint = CameraControlls.mouseWorldPosition;
                        unit.cMoveable.SendMessage("SetTarget", unloadPoint);
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
