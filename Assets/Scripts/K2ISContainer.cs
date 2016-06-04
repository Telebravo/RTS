using UnityEngine;
using System.Collections;

public class K2ISContainer : MonoBehaviour
{
    public GameObject box1, box2;

    CHookliftable hookliftable;

    public bool selected = false;
    public bool deployed = false;
    bool cursorControll = false;
    
	void Start ()
    {
        hookliftable = GetComponent<CHookliftable>();

        if (deployed)
            Unpack();
        else
            Pack();
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
    void Pack()
    {
        deployed = false;

        box1.SetActive(false);
        box2.SetActive(false);
    }
    void Unpack()
    {
        deployed = true;

        box1.SetActive(true);
        box2.SetActive(true);
    }
}
