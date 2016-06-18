using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class CameraControlls : MonoBehaviour
{
    public Texture2D[] cursorTextures;

    //Liste over synlige objekter
    public List<Transform> visibleObjects;
    //Maks antall valgte units
    public int selectionLimit;

    //Objektet musen for øyeblikket holder over, men ikke nødvendigvis har valgt
    public Transform holdOverObject;
    public Vector3 mouseWorldPositionUnitLayer;
    public Vector3 mouseWorldPosition;

    //Objektet som er valgt
    public List<Transform> selectedObjects;
    List<Transform> previouslySelectedObjects;
    public RectTransform selectionAreaPanel;
    //Det forrige objektet
    Transform lastSelected;

    public RectTransform directionArrow;

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

    //Ved start, liksom litt før den andre start greien
    void Awake()
    {
        //Layermask for unit-layeret
        layermask = 1 << layer;

        //Initsierer stuff
        visibleObjects = new List<Transform>();
        selectedObjects = new List<Transform>();
        GameManager.controlls = this;
    }
    //Ved start
    void Start()
    {
        //Henter kamera koponenten
        camera = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Confined;
        SetCursor(Cursors.Arrow);
    }
    void Update()
    {
        //Om vi trykker på escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Frigjør musepekeren
            Cursor.lockState = CursorLockMode.None;
        }

        //Finner objektet man paker på
        holdOverObject = null;
        //Lager er ray fra kamera til posisjonen i verdenen man peker på med musen
        ray = camera.ScreenPointToRay(Input.mousePosition);
        //Ser om vi treffer noen units
        if (Physics.Raycast(ray, out hit, 10000, layermask, QueryTriggerInteraction.Ignore))
        {
            //Lagrer treffpunket
            mouseWorldPositionUnitLayer = hit.point;
            //Lagrer hvilken unit vi peker på
            holdOverObject = hit.transform;
            //Ser om det kan velges
            cSelectable = holdOverObject.GetComponent<CSelectable>();
            if (cSelectable != null) ;

        }

        //Musepekeren sin posison i verden
        ray = camera.ScreenPointToRay(Input.mousePosition);
        //Ser om vi treffer noe, gjelder også andre ting enn units
        if (Physics.Raycast(ray, out hit, 10000, 1, QueryTriggerInteraction.Ignore))
        {
            //Lagrer treffpunktet
            mouseWorldPosition = hit.point;
        }

        //Kan ikke velge og flytte ting om man holder inne control, da den brukes for å utføre handlinger med units
        if (Input.GetKey(KeyCode.LeftControl))
            return;

        //Når man trykker ned venste museknapp
        if (Input.GetMouseButtonDown(0))
        {
            //Posisjonen når museknapen ble trykket ned
            mp1 = Input.mousePosition;
            selectionAreaPanel.gameObject.SetActive(true);
        }
        //Så lenge venstre musenkapp er nede
        if (Input.GetMouseButton(0))
        {
            //Den nåværende posisjonen
            mp2 = Input.mousePosition;

            //Tegner rektangelet med magi
            selectionAreaPanel.position = new Vector3(mp1.x <= mp2.x ? mp1.x : mp2.x, mp1.y <= mp2.y ? mp1.y : mp2.y);
            selectionAreaPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mp1.x <= mp2.x ? mp2.x - mp1.x : mp1.x - mp2.x);
            selectionAreaPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mp1.y <= mp2.y ? mp2.y - mp1.y : mp1.y - mp2.y);
        }
        //Når man slipper venstre museknapp
        if (Input.GetMouseButtonUp(0))
        {
            //Gjemmer grafikken
            selectionAreaPanel.gameObject.SetActive(false);

            //Husker de forige valgte objektene
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
                            //Om vi har nådd selectionlimiten
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

                        //Ser om vi har nådd selectionlimiten
                        if (selectedObjects.Count >= selectionLimit)
                            break;
                    }
                }
            }
            //For hvert objekt av de vi hadde velgt forrige frame
            foreach (Transform obj in previouslySelectedObjects)
            {
                //Om det ikke er valgt lenger
                if (!selectedObjects.Contains(obj))
                    //Sier ifra til objektet at det ikke er valgt lenger
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
                    //For hvert av de valgte unitsene
                    foreach (Transform obj in selectedObjects)
                    {
                        //Får tak i move-komponenten
                        cMoveable = obj.GetComponent<CMoveable>();
                        //Om uniten kan flytte på seg
                        if (cMoveable != null)
                        {
                            //Sier at den skal ture 
                            cMoveable.SendMessage("SetTarget", hit.point);
                        }
                    }
                }
            }
        }
    }
    void Move()
    {
        //Finner posisjon i verden utifra musepeker
        ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100))
        {
            //Sender ny posisjon til objektet
            cMoveable.SendMessage("SetTarget", hit.point);
        }
    }

    public void Select(Transform transfom)
    {
        if (!selectedObjects.Contains(transfom))
        {
            selectedObjects.Add(transfom);
            transfom.GetComponent<CSelectable>().SendMessage("Selected");
        }
    }
    public void Deselect(Transform transfom)
    {
        if (selectedObjects.Contains(transfom))
        {
            selectedObjects.Remove(transfom);
            transfom.GetComponent<CSelectable>().SendMessage("Deselected");
        }
    }
    public void SetVisible(Transform transform, bool visible)
    {
        if (visible)
        {
            if (!GameManager.controlls.visibleObjects.Contains(transform))
                GameManager.controlls.visibleObjects.Add(transform);
        }
        else
        {
        if (GameManager.controlls.visibleObjects.Contains(transform))
            GameManager.controlls.visibleObjects.Remove(transform);
        }
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
        return screenPoint;
    }

    public void SetCursor(Cursors cursor)
    {
        switch (cursor)
        {
            case Cursors.Arrow:
                Cursor.SetCursor(cursorTextures[(int)cursor], new Vector2(0, 0), CursorMode.Auto);
                break;
            case Cursors.Select:
                Cursor.SetCursor(cursorTextures[(int)cursor], new Vector2(0, 0), CursorMode.Auto);
                break;
            case Cursors.Attack:
                Cursor.SetCursor(cursorTextures[(int)cursor], new Vector2(0, 0), CursorMode.Auto);
                break;
            case Cursors.Load:
                Cursor.SetCursor(cursorTextures[(int)cursor], new Vector2(16, 0), CursorMode.Auto);
                break;
            case Cursors.LoadBlocked:
                Cursor.SetCursor(cursorTextures[(int)cursor], new Vector2(16, 0), CursorMode.Auto);
                break;
            case Cursors.Unload:
                Cursor.SetCursor(cursorTextures[(int)cursor], new Vector2(16, 32), CursorMode.Auto);
                break;
            case Cursors.UnloadBlocked:
                Cursor.SetCursor(cursorTextures[(int)cursor], new Vector2(16, 32), CursorMode.Auto);
                break;
            case Cursors.Pack:
                Cursor.SetCursor(cursorTextures[(int)cursor], new Vector2(16, 16), CursorMode.Auto);
                break;
            case Cursors.Unpack:
                Cursor.SetCursor(cursorTextures[(int)cursor], new Vector2(16, 16), CursorMode.Auto);
                break;
        }
    }
}
public enum Cursors { Arrow, Select, Attack, Load, LoadBlocked, Unload, UnloadBlocked, Pack, Unpack}