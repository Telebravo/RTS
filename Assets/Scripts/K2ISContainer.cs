using UnityEngine;
using System.Collections;

[AddComponentMenu("Unit/K2ISContainer")]
[RequireComponent(typeof(CHookliftable))]
public class K2ISContainer : MonoBehaviour
{
    CHookliftable hookliftable;
    Animator anim;

    public bool selected = false;
    public bool deployed = false;
    bool cursorControll = false;
    
	void Start ()
    {
        anim = GetComponent<Animator>();
        hookliftable = GetComponent<CHookliftable>();
    }
	void Update ()
    {
	    if(selected && Input.GetKey(KeyCode.LeftControl) && GameManager.controlls.holdOverObject == transform && !hookliftable.onTruck)
        {
            cursorControll = true;
            if (deployed)
                GameManager.controlls.SetCursor(Cursors.Pack);
            else
                GameManager.controlls.SetCursor(Cursors.Unpack);

            if(Input.GetMouseButtonDown(1))
            {
                if (deployed)
                    Pack();
                else
                    Unpack();
            }
        }
        else if(cursorControll)
        {
            cursorControll = false;
            GameManager.controlls.SetCursor(Cursors.Arrow);
        }
	}
    void Select(bool selected)
    {
        this.selected = selected;
    }
    void Unpack()
    {
        hookliftable.liftable = false;
        anim.SetTrigger("Unpack");
    }
    void UnpackComplete()
    {
        deployed = true;
        ResetTriggers();
    }
    void Pack()
    {
        anim.SetTrigger("Pack");
    }
    void PackComplete()
    {
        deployed = false;
        hookliftable.liftable = true;
        ResetTriggers();
    }
    void ResetTriggers()
    {
        anim.ResetTrigger("Pack");
        anim.ResetTrigger("Unpack");
    }
}
