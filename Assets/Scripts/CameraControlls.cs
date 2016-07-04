using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CameraControlls : MonoBehaviour
{
    public Texture2D[] cursorTextures;

    //Liste over synlige objekter
    public List<Unit> visibleUnits;
    //Maks antall valgte units
    public int selectionLimit;

    //Objektet musen for øyeblikket holder over, men ikke nødvendigvis har valgt
    public Transform holdOverObject;
    public Vector3 mouseWorldPositionUnitLayer;
    public Vector3 mouseWorldPosition;

    //Objektet som er valgt
    public List<Unit> selectedUnits;
    List<Unit> previouslySelectedUnits;
    public RectTransform selectionAreaPanel;
    //Det forrige objektet
    Transform lastSelected;

    public RectTransform directionArrow;

    //Selectable komponenten til objektet (ser om det er noe som skal kunne bli valgt)
    Unit selectedUnit;
    //Moveable komponenten til objektet (ser om et er noe som kan beordres til å flytte)
    CMoveable cMoveable;

    //Kameraet
    new public static Camera camera;
    Vector3 mp1, mp2;

    //Raycast stuff
    Ray ray;
    RaycastHit hit;
    //Unit layeret
    int unitLayer = 8;
    int unitLayermask;
    //UI layer
    int UILayer = 5;
    int unitAndUILayermask;

    bool onUI = false;

    //Ved start, liksom litt før den andre start greien
    void Awake()
    {
        //Layermask for unit-layeret
        unitLayermask = 1 << unitLayer;
        unitAndUILayermask = 1 << unitLayer | 1 << UILayer;

        //Initsierer stuff
        visibleUnits = new List<Unit>();
        selectedUnits = new List<Unit>();
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

        Selection();
        Movement();
    }
    void Selection()
    {
        //Finner objektet man paker på
        holdOverObject = null;
        //Lager er ray fra kamera til posisjonen i verdenen man peker på med musen
        ray = camera.ScreenPointToRay(Input.mousePosition);
        //Ser om vi treffer noen units
        if (Physics.Raycast(ray, out hit, 10000, unitLayermask, QueryTriggerInteraction.Ignore))
        {
            //Lagrer treffpunket
            mouseWorldPositionUnitLayer = hit.point;
            //Lagrer hvilken unit vi peker på
            holdOverObject = hit.transform;
            //Ser om det kan velges
            selectedUnit = holdOverObject.GetComponent<Unit>();
            if (selectedUnit != null)
            {

            }

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
            //Gjør ingen ting om musen er over UIet
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1))
            {
                onUI = true;
                return;
            }
            onUI = false;

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

            if(onUI)
            {
                return;
            }
            onUI = false;
            //Om man bare har trykket
            if (Vector2.Distance(mp1, mp2) < 10)
            {
                //Ignorerer det om musen er over UIet
                if(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1))
                {
                    return;
                }
            }
            //Husker de forige valgte objektene
            previouslySelectedUnits = new List<Unit>(selectedUnits);
            selectedUnits.Clear();

            //Det merkede området
            Rect area = selectionAreaPanel.rect;
            area.x = selectionAreaPanel.position.x;
            area.y = selectionAreaPanel.position.y;

            //Om man bare har trykket
            if (Vector2.Distance(mp1, mp2) < 10)
            {
                //Finner posisjon i verden utifra musepeker
                ray = camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, 10000, unitAndUILayermask, QueryTriggerInteraction.Ignore))
                {
                    //Ser om objektet kan valges
                    selectedUnit = hit.transform.GetComponent<Unit>();
                    if (selectedUnit != null)
                    {
                        //Velger objektet
                        selectedUnits.Add(selectedUnit);
                        selectedUnit.SendMessage("Selected");
                    }
                }
            }
            else //Om man har markert et område
            {
                //Går igjennom alle objektene på skjermen
                foreach (Unit unit in visibleUnits)
                {
                    //Ser om de er i området
                    Vector2 point = Camera.main.WorldToScreenPoint(unit.transform.position);
                    if (area.Contains(point))
                    {
                        //Ser om det allerede var valgt
                        if (previouslySelectedUnits.Contains(unit))
                        {
                            selectedUnits.Add(unit);
                            //Om vi har nådd selectionlimiten
                            if (selectedUnits.Count >= selectionLimit)
                                break;
                            else
                                continue;
                        }

                        //Legger det til i listen over valgte objekter
                        selectedUnits.Add(unit);

                        //Melder fra til objektet at det er valgt
                        unit.cSelectable.SendMessage("Selected");

                        //Ser om vi har nådd selectionlimiten
                        if (selectedUnits.Count >= selectionLimit)
                            break;
                    }
                }
            }
            //For hvert objekt av de vi hadde velgt forrige frame
            foreach (Unit unit in previouslySelectedUnits)
            {
                //Om det ikke er valgt lenger
                if (!selectedUnits.Contains(unit))
                    //Sier ifra til objektet at det ikke er valgt lenger
                    unit.cSelectable.SendMessage("Deselected");
            }

            if(!selectedUnits.Equals(previouslySelectedUnits))
                UIUnitInfo.SelectionChanged();
        }
    }

    void Movement()
    {
        //FLYTTE UNIT
        if (Input.GetMouseButtonUp(1))
        {
            //Om et object er vagt
            if (selectedUnits.Count > 0)
            {
                //Finner posisjon i verden utifra musepeker
                ray = camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 10000, 1, QueryTriggerInteraction.Ignore))
                {
                    //For hvert av de valgte unitsene
                    foreach (Unit unit in selectedUnits)
                    {
                        //Får tak i move-komponenten
                        cMoveable = unit.GetComponent<CMoveable>();
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

    public void Select(Unit unit)
    {
        if (!selectedUnits.Contains(unit))
        {
            selectedUnits.Add(unit);
            unit.cSelectable.SendMessage("Selected");
        }
    }
    public void Deselect(Unit unit)
    {
        if (selectedUnits.Contains(unit))
        {
            selectedUnits.Remove(unit);
            unit.cSelectable.SendMessage("Deselected");
        }
    }
    public void DeselectAll()
    {
        previouslySelectedUnits = new List<Unit>(selectedUnits);
        selectedUnits.Clear();
        for (int i = 0; i < previouslySelectedUnits.Count; i++)
        {
            previouslySelectedUnits[i].cSelectable.SendMessage("Deselected");
        }
    }

    public void SetVisible(Unit unit, bool visible)
    {
        if (visible)
        {
            if (!GameManager.controlls.visibleUnits.Contains(unit))
                GameManager.controlls.visibleUnits.Add(unit);
        }
        else
        {
            if (GameManager.controlls.visibleUnits.Contains(unit))
                GameManager.controlls.visibleUnits.Remove(unit);
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