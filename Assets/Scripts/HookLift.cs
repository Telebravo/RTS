using UnityEngine;
using System.Collections;

public class HookLift : MonoBehaviour
{
    public Transform cargoTransform;
    public Transform hookPoint;
    Vector3 parkPos;
    Quaternion parkDir;
    float targetYDiff;

    Animator anim;
    bool selected = false;
    bool cursorControll = false;
    Unit unit;
    CHookliftable target;
    bool carrynig = false;
    Vector3 unloadPoint;
    bool unloading = false;
    bool loading = false;

    void Start ()
    {
        unit = GetComponent<Unit>();
        anim = GetComponent<Animator>();
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
                    if (!hookliftable.onTruck && hookliftable.liftable)
                    {
                        GameManager.controlls.SetCursor(Cursors.Load);
                        cursorControll = true;

                        if(Input.GetMouseButtonDown(1))
                        {
                            target = hookliftable;
                            Vector3 targetDir = target.hookPoint.position - target.transform.position;
                            targetDir.y = 0;
                            parkDir = Quaternion.LookRotation(targetDir, Vector3.up);
                            float hookPointDist = hookPoint.localPosition.z;
                            parkPos = target.hookPoint.position - targetDir.normalized * hookPointDist;
                            parkPos.y = target.hookPoint.position.y - hookPoint.position.y;
                            Debug.Log("parkPos: " + parkPos.ToString());
                            unit.cMoveable.SetTarget(parkPos);
                        }
                    }
                    else
                    {
                        GameManager.controlls.SetCursor(Cursors.LoadBlocked);
                        cursorControll = true;
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
                        target.navMeshObstacle.enabled = true;
                        target = null;
                        carrynig = false;
                        unloading = false;
                    }
                }
            }
            else if (loading)
            {
                target.transform.position = cargoTransform.position;
                Vector3 euler = cargoTransform.rotation.eulerAngles;
                euler.y += targetYDiff;
                if((targetYDiff < 185 && targetYDiff > 175) || (targetYDiff > -185 && targetYDiff < -175))
                {
                    euler.x *= -1;
                }
                target.transform.rotation = Quaternion.Euler(euler);
            }
            else if (Vector3.Distance(transform.position, parkPos) < 0.5)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, parkDir, 45f * Time.deltaTime);
                if(Quaternion.Angle(transform.rotation, parkDir) < 1)
                {
                    loading = true;
                    target.navMeshObstacle.enabled = false;
                    targetYDiff = parkDir.eulerAngles.y - target.transform.rotation.eulerAngles.y;
                    anim.SetTrigger("Hook");
                }
            }
        }
    }
	
	public void Select(bool selected)
    {
        this.selected = selected;
        Debug.Log(selected);
    }
    public void LoadingComplete()
    {
        loading = false;
        Debug.Log("Park");
        target.transform.parent = transform;
        //target.transform.localPosition = target.lockPossition;
        //target.transform.localRotation = new Quaternion();
        target.onTruck = true;
        carrynig = true;
    }
}
