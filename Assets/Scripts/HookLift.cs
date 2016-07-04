using UnityEngine;
using System.Collections;

[AddComponentMenu("Unit/HookLift")]
[RequireComponent(typeof(Unit))]
public class HookLift : MonoBehaviour
{
    //Transformen til dummyen brukt i animasjonen
    public Transform cargoTransform;
    //Tuppen av kroken
    public Transform hookPoint;
    //Punktet vi skal stoppe på for å laste på targetet
    Vector3 parkPos;
    //Veien vi skal peke
    Quaternion parkDir;
    //Høydeforskjelden på sentrene til løfteren og det vi skal løfte når kroken og krokfestet er sammen
    float targetYDiff;

    //Animasjonskomponenten
    Animator anim;
    //Om uniten er valgt
    bool selected = false;
    //Om vi forøyeblikket sier det skal brukess en annen cursor en normalt
    bool cursorControll = false;
    //Unit-komponenten
    Unit unit;
    //Objektet vi skal løfte
    CHookliftable target;
    //Om vi for øyeblikket har noen last
    bool carrynig = false;
    //Om vi dirver og laster av eller på noe
    bool loading = false;
    bool travelToUnload = false;
    bool unloading = false;
    bool unloadMove = false;

    //
    Vector2 mouse1, mouse2;

    //Ved start
    void Start ()
    {
        //Henter komponentenen
        unit = GetComponent<Unit>();
        anim = GetComponent<Animator>();
    }
    //Hver frame
	void Update ()
    {
        //Om vi har satt en spesiell cursor
        if (cursorControll)
        {
            //Forandrer den tilbake
            GameManager.controlls.SetCursor(Cursors.Select);
            //Husker at vi gjorde det
            cursorControll = false;
        }
        //Om uniten er valgt og ikke allerede har en last
        if (selected && !carrynig)
        {
            //Om vi holder musen over et objekt og holder nede control
            if (GameManager.controlls.holdOverObject != null && Input.GetKey(KeyCode.LeftControl))
            {
                //Ser om det kan løftes
                CHookliftable hookliftable = GameManager.controlls.holdOverObject.GetComponent<CHookliftable>();
                if (hookliftable != null)
                {
                    //Ser om det ikke allerede er på en lastebil eller av en annen grunn ikke kan løftes
                    if (!hookliftable.onTruck && hookliftable.liftable)
                    {
                        //Om alt er good skifter vi cursor til load ikonet
                        GameManager.controlls.SetCursor(Cursors.Load);
                        cursorControll = true;

                        //Om man så trykker på venstre museknapp
                        if(Input.GetMouseButtonDown(1))
                        {
                            //Ser om ikke vi allerede driver å løfter noe
                            if (loading)
                            {

                            }
                            //Om vi ikke holder på å løfte noe
                            else
                            {
                                //Setter targetet til det objektet vi holdt musen over
                                target = hookliftable;
                                //Ser hvilken vei det står
                                Vector3 targetDir = target.hookPoint.position - target.transform.position;
                                //Driter fint i høyde
                                targetDir.y = 0;
                                //Lagrer en Quaternion for den veien lastebilen må stå
                                parkDir = Quaternion.LookRotation(targetDir, Vector3.up);
                                //Ser hvor langtbak fra senteret posisjonen til kroken er når den skal løfte shit
                                float hookPointDist = hookPoint.localPosition.z;
                                //Der vi skal stå blir posisjonen til targetets krokfeste minus litt lengre bak langs retningen til det vi skal løfte 
                                parkPos = target.hookPoint.position - targetDir.normalized * hookPointDist;
                                //Høyden vi må stå i er lik høydeforskjelden til krokfeset og kroken (forhåpentlig vi vil dette tilsvare at vi står på bakken)
                                parkPos.y = target.hookPoint.position.y - hookPoint.position.y;
                                //Sier til move-komponenten at vi gjerne vil kjøre til målet
                                unit.cMoveable.SetTarget(parkPos);
                            }
                        }
                    }
                    else //Om objektet ikke kan løftes akkurat nå
                    {
                        //Bytter vi cursor til load blocked
                        GameManager.controlls.SetCursor(Cursors.LoadBlocked);
                        cursorControll = true;
                    }
                }
            }
        }
        //Om vi har settoss ut noe vi skal kroke
        if(target != null)
        {
            //Om det allerede er på lasteplanet
            if(target.onTruck)
            {
                //Om vi holder ikke kontroll
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    //Setter musen til unload-ikonet
                    GameManager.controlls.SetCursor(Cursors.Unload);
                    cursorControll = true;
                    //Om vi så tryker på venstre museknapp
                    if (Input.GetMouseButtonDown(1))
                    {
                        //Setter posisjonen til pilen lik musens posisjon i verden+ litt opp
                        GameManager.controlls.directionArrow.position = GameManager.controlls.mouseWorldPosition + Vector3.up/10;
                        //Setter den like lang som collideren til lasten
                        GameManager.controlls.directionArrow.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, target.GetComponent<Collider>().bounds.size.z * 100);
                        //Gjør pilen synelig
                        GameManager.controlls.directionArrow.gameObject.SetActive(true);
                    }
                    //Så lenge vi holder den inne
                    else if (Input.GetMouseButton(1))
                    {
                        //Henter posisjonen til musen da museknappen ble trykket inn 
                        Vector3 pos1 = GameManager.controlls.directionArrow.position;
                        //Posisjonen nå
                        Vector3 pos2 = GameManager.controlls.mouseWorldPosition;
                        //Lager en vektor fra den første posisjonen til den andre
                        Vector3 dir = pos2 - pos1;
                        //Driter i høyde
                        dir.y = 0;

                        //Vinkelen i fohold til rett frem på z-aksen (blå pil i editoren)
                        float angle = Vector3.Angle(Vector3.back, dir);
                        if (dir.x > 0)
                            angle *= -1;

                        //Setter rotasjonen til objektet
                        GameManager.controlls.directionArrow.rotation = Quaternion.Euler(90, angle, 0);
                    }
                    //Når den slippes
                    else if (Input.GetMouseButtonUp(1))
                    {
                        //Skjuler pilen
                        GameManager.controlls.directionArrow.gameObject.SetActive(false);

                        //Henter rorasjonen til pilen
                        parkDir = GameManager.controlls.directionArrow.rotation;
                        //Trenger bare rotasjonen rundt y-aksen
                        Vector3 targetDir = new Vector3(0, parkDir.eulerAngles.y, 0);
                        parkDir = Quaternion.Euler(targetDir);

                        // Parkpos = (posisjonen til pilen) + retning * ((9.2 - 4.2 = 4.5) - (distansen fra senteret av kontaineren til krokfestet))
                        // 9.2 = (distansen fra krokløfteren til kontaineren når den er satt ned)
                        // 4.2 = (distensen den kjører frem når den setter ned kontaineren)
                        parkPos = GameManager.controlls.directionArrow.position + (parkDir * Vector3.forward).normalized * (4.5f - target.GetComponent<Collider>().bounds.extents.z);
                        //Trekker fra distansen pilen står over bakken
                        parkPos.y -= 0.1f;
                        //Husker at vi er på vei
                        travelToUnload = true;
                        //Turer dit
                        unit.cMoveable.SetTarget(parkPos);

                        Debug.Log("Parkpos: " + parkPos.ToString());
                        Debug.Log("Parkdir: " + parkDir.eulerAngles.ToString());
                    }
                }
                //Om vi har planer om å laste av noe
                if(travelToUnload)
                {
                    //Ser om vi er der driten skal
                    if(Vector3.Distance(transform.position, parkPos) < 0.2)
                    {
                        //Roterer til den veien vi skal stå
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, parkDir, 45f * Time.deltaTime);
                        //Om det er innafor
                        if (Quaternion.Angle(transform.rotation, parkDir) < 1)
                        {
                            //Vi er fremme
                            travelToUnload = false;
                            //Gjør at lasten ikke lengre er et child av krokløfteren
                            target.transform.parent = transform.parent;
                            //Vi bærer ikke noe lengre
                            carrynig = false;
                            //Driver å laster av
                            unloading = true;
                            //Skal ikke flytte oss men vi laster av
                            unit.cMoveable.ClearTarget();
                            unit.cMoveable.canMove = false;
                            //Sier til lasten at den ikke lenger sitter på en bil
                            target.onTruck = false;
                            //Starter animajonen
                            anim.SetTrigger("Unhook");
                        }
                    }
                }
            }
            //Hvis vi holder på å løfte det 
            else if (loading || unloading)
            {
                //Setter posisjonen til det vi løfter i henhold til animasjonen
                target.transform.position = cargoTransform.position;
                //Henter vinkelene rotasjonen skal ha fra animasjonen
                Vector3 euler = cargoTransform.rotation.eulerAngles;
                //Justerer for rotasjonen rundt yaksen objektet kan ha hatt da vi begynte å løfte det
                euler.y += targetYDiff;
                //Passerpå at det roterer riktig vei når det løftes
                // (Fungerer bare ved ish 0 og 180 grader
                if((targetYDiff < 185 && targetYDiff > 175) || (targetYDiff > -185 && targetYDiff < -175))
                {
                    //Om driten står ~180 grader feil vei, så snur vi på x-rotasjonen
                    euler.x *= -1;
                }
                //Setter rotasjonen til objektet
                target.transform.rotation = Quaternion.Euler(euler);
            }
            //Hvis vi er i posisjon til å kroke
            else if (Vector3.Distance(transform.position, parkPos) < 0.2)
            {
                //Roterer til den veien vi skal stå
                transform.rotation = Quaternion.RotateTowards(transform.rotation, parkDir, 45f * Time.deltaTime);
                //Om det er innafor
                if(Quaternion.Angle(transform.rotation, parkDir) < 1)
                {
                    //Husker at vi driver å kroker
                    loading = true;
                    //Slår av kollideren så stuff ikke fucker seg
                    target.navMeshObstacle.enabled = false;
                    //Husker hvilken vei objektet er rotert så vi kan justere det ift animasjonen
                    targetYDiff = parkDir.eulerAngles.y - target.transform.rotation.eulerAngles.y;
                    //Starter løfte-animasjonen
                    anim.SetTrigger("Hook");
                    //Står her vi
                    unit.cMoveable.ClearTarget();
                    unit.cMoveable.canMove = false;
                    //Sier at ingen andre kan løfte den nå;
                    target.liftable = false;
                }
            }
        }
    }
    //Hvert physics step
    void FixedUpdate()
    {
        if (unloadMove)
        {
            // Time: 4,8333 (Animation duration)
            // Dist: 5,3 (Distance)
            // Dist / Time = 1,09655 m/s
            transform.Translate(0, 0, 1.09655f * Time.fixedDeltaTime, Space.Self);
        }
    }
	//Om uniten blir selected eller deselected 
	public void Select(bool selected)
    {
        //Husker på det, greit å vite liksom
        this.selected = selected;
    }
    //Når løfte-animasjonen er ferdig
    public void LoadingComplete()
    {
        //Nei, vi driver utrolig nok ikke å løfter noe lenger
        loading = false;
        //Legger lasten inn som et child til dette objektet så det følger med når vi kjører og sånt
        target.transform.parent = transform;
        //Sier til lasten: Nigga, u on a boat!
        target.onTruck = true;
        //Vurderer å huske på at vi faktisk bærer på noe
        carrynig = true;
        //Får lov til å kjøre igjen
        unit.cMoveable.canMove = true;
    }
    //når senke-animasjonen er ferdig
    public void UnloadComplete()
    {
        //Laster ikke av lengre
        unloading = false;
        //Skrur på collideren til lasten
        target.navMeshObstacle.enabled = true;
        //Sier at andre an løfte den igjen
        target.liftable = true;
        //Har ikke noe last/target lenger
        target = null;
        //Vi kan ture rundt igjen
        unit.cMoveable.canMove = true;
    }
    //Når vi skal begynne å kjøre litt fremmover under senke-animasjonen
    public void UnloadStartMove()
    {
        //Debug.Log("Pos1:" + transform.position.ToString());
        //Vi skal kjøre fremover
        unloadMove = true;
        //Skrur av navmeshagenten
        unit.cMoveable.DisableUpdate();
    }
    //Når vi skal stoppe å kjøre fremmover under senke-animasjonen
    public void UnloadStopMove()
    {
        //Skal ikke flytte oss lenger
        unloadMove = false;
        //Skrur på navmeshagenten igjen
        unit.cMoveable.EnableUpdate();
        //Flytter navmeshagenten til den nye posisjonen
        unit.cMoveable.Warp(transform.position, true);
        //Debug.Log("Pos12" + transform.position.ToString());
    }
}
