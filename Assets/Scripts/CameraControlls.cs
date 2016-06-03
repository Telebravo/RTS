using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class CameraControlls : MonoBehaviour
{
    //Liste over synlige objekter
    public static List<Transform> visibleObjects;
    public List<Transform> _visible;
    //Maks antall valgte units
    public int selectionLimit;

    //Objektet som er valgt
    public static List<Transform> selectedObjects;
    List<Transform> previouslySelectedObjects;
    public RectTransform selectionAreaPanel;
    //Det forrige objektet
    Transform lastSelected;

    //Selectable komponenten til objektet (ser om det er noe som skal kunne bli valgt)
    CSelectable cSelectable;
    //Moveable komponenten til objektet (ser om et er noe som kan beordres til å flytte)
    CMoveable cMoveable;
    
    //Kameraet
    new public static Camera camera;
    Vector3 mp1, mp2;

    //Raycast stuff
    Ray ray;
    RaycastHit hit;
    //Unit layeret
    int layer = 8; 
    int layermask;

    void Awake()
    {
        layermask = 1 << layer;
        visibleObjects = new List<Transform>();
        selectedObjects = new List<Transform>();
    }
    void Start ()
    {
        //Henter kamera koponenten
        camera = GetComponent<Camera>();
	}
    public static Rect GUIRectWithObject(GameObject go)
    {
        Vector3 cen = go.GetComponent<Renderer>().bounds.center;
        Vector3 ext = go.GetComponent<Renderer>().bounds.extents;
        Vector2[] extentPoints = new Vector2[8]
         {
               WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z-ext.z)),
               WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z-ext.z)),
               WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z+ext.z)),
               WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z+ext.z)),
               WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z-ext.z)),
               WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z-ext.z)),
               WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z+ext.z)),
               WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z+ext.z))
         };
        Vector2 min = extentPoints[0];
        Vector2 max = extentPoints[0];
        foreach (Vector2 v in extentPoints)
        {
            min = Vector2.Min(min, v);
            max = Vector2.Max(max, v);
        }
        return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    }
    public static Vector2 WorldToGUIPoint(Vector3 world)
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(world);
        //screenPoint.y = (float)Screen.height - screenPoint.y;
        return screenPoint;
    }

    void Update ()
    {
        if (Input.GetKey(KeyCode.LeftControl))
            return;

        _visible = visibleObjects;
       if (Input.GetMouseButtonDown(0))
       {
            //Posisjonen når museknapen ble trykket ned
            mp1 = Input.mousePosition;
            selectionAreaPanel.gameObject.SetActive(true);
        }
       if(Input.GetMouseButton(0))
       {
            //Den nåværende posisjonen
            mp2 = Input.mousePosition;

            //Tegner rektangelet med magi
            selectionAreaPanel.position = new Vector3(mp1.x <= mp2.x ? mp1.x : mp2.x, mp1.y <= mp2.y ? mp1.y : mp2.y);
            selectionAreaPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mp1.x <= mp2.x ? mp2.x - mp1.x : mp1.x - mp2.x);
            selectionAreaPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   mp1.y <= mp2.y ? mp2.y - mp1.y : mp1.y - mp2.y);
        }
        if (Input.GetMouseButtonUp(0))
        {
            //Gjemmer grafikken
            selectionAreaPanel.gameObject.SetActive(false);

            //
            previouslySelectedObjects = new List<Transform>(selectedObjects);
            selectedObjects.Clear();

            //Det merkede området
            Rect area = selectionAreaPanel.rect;
            area.x = selectionAreaPanel.position.x;
            area.y = selectionAreaPanel.position.y;

            //Om man bare har trykket
            if (Vector2.Distance(mp1, mp2) < 10)
            {
                //Finner posisjon i verden utifra musepeker
                ray = camera.ScreenPointToRay(Input.mousePosition);
                
                if (Physics.Raycast(ray, out hit, 10000, layermask, QueryTriggerInteraction.Ignore))
                {
                    //Ser om objektet kan valges
                    cSelectable = hit.transform.GetComponent<CSelectable>();
                    if (cSelectable != null)
                    {
                        //Velger objektet
                        selectedObjects.Add(hit.transform);
                        cSelectable.SendMessage("Selected");
                    }
                }
            }
            else //Om man har markert et område
            {
                //Går igjennom alle objektene på skjermen
                foreach (Transform obj in visibleObjects)
                {
                    //Ser om de er i området
                    Vector2 point = Camera.main.WorldToScreenPoint(obj.position);
                    if (area.Contains(point))
                    {
                        //Ser om det allerede var valgt
                        if (previouslySelectedObjects.Contains(obj))
                        {
                            selectedObjects.Add(obj);
                            if (selectedObjects.Count >= selectionLimit)
                                break;
                            else
                                continue;
                        }

                        //Legger det til i listen over valgte objekter
                        selectedObjects.Add(obj);

                        //Melder fra til objektet at det er valgt
                        cSelectable = obj.GetComponent<CSelectable>();
                        cSelectable.SendMessage("Selected");

                        if (selectedObjects.Count >= selectionLimit)
                            break;
                    }
                }
            }
            foreach (Transform obj in previouslySelectedObjects)
            {
                if(!selectedObjects.Contains(obj))
                    obj.GetComponent<CSelectable>().SendMessage("Deselected");
            }
            Debug.Log(string.Format("current: {0}, prev: {1}", selectedObjects.Count, previouslySelectedObjects.Count));
        }

        //FLYTTE UNIT
        if (Input.GetMouseButtonUp(1))
        {
            //Om et object er vagt
            if (selectedObjects.Count > 0)
            {
                //Finner posisjon i verden utifra musepeker
                ray = camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 10000, 1, QueryTriggerInteraction.Ignore))
                {
                    foreach (Transform obj in selectedObjects)
                    {
                        cMoveable = obj.GetComponent<CMoveable>();
                        if (cMoveable != null)
                            cMoveable.SendMessage("SetTarget", hit.point);
                    }
                }
            }
        }
    }
    /*void Select ()
    {
        Debug.Log("Camera: Selecting");
        ray = camera.ScreenPointToRay(Input.mousePosition);

        //Ser om vi treffer noe
        if (Physics.Raycast(ray, out hit, 100))
        {
            //Husker det potensielt forrige objektet som var valgt
            lastSelected = selected;
            selected = hit.transform;

            //Om vi har valgt en ny unit
            if (selected == lastSelected)
            {
                return;
            }
            else if (cSelectable != null)
            {
                //Melder fra tld en forrige at den ikke er valgt lenger
                cSelectable.SendMessage("Deselected");
            }

            //Henter komponenten til det nye objektet
            cSelectable = selected.GetComponent<CSelectable>();

            //Om det er noe som kan velges
            if (cSelectable != null)
            {
                //Melder fra til objektet at det er valgt
                cSelectable.SendMessage("Selected");
                cMoveable = selected.GetComponent<CMoveable>();
            }
            //Om det ikke kan bli valgt
            else
            {
                //Resetter
                selected = null;
                lastSelected = null;
                cSelectable = null;
                cMoveable = null;
            }
        }
        //Om vi ikke trykket på noe
        else
        {
            //Melder fra til det potensielt forrige valgte objektet at det ikke er valgt lenger
            if (cSelectable != null)
                cSelectable.SendMessage("Deselected");

            //Resetter
            selected = null;
            lastSelected = null;
            cSelectable = null;
            cMoveable = null;
        }
    }*/
    void Move ()
    {
        //Finner posisjon i verden utifra musepeker
        ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100))
        {
            //Sender ny posisjon til objektet
            cMoveable.SendMessage("SetTarget", hit.point);
        }
    }

    /*void DrawSelectionRect()
    {
        Rect r = GUIRectWithObject(selected.gameObject);
        Debug.Log(string.Format("X:{0} - {1} Y: {2} - {3}", r.xMin, r.xMax, r.yMin, r.yMax));
        panel.position = new Vector3(r.xMin - 5, r.yMin - 5);
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, r.width + 10);
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, r.height + 10);

        Debug.Log(panel.position.y);
    }*/

    public static void Delect(Transform transfom)
    {
        if (!selectedObjects.Contains(transfom))
        {
            selectedObjects.Add(transfom);
            transfom.GetComponent<CSelectable>().SendMessage("Selected");
        }
    }
    public static void Deselect(Transform transfom)
    {
        if (selectedObjects.Contains(transfom))
        {
            selectedObjects.Remove(transfom);
            transfom.GetComponent<CSelectable>().SendMessage("Deselected");
        }
    }
}