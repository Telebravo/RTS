using UnityEngine;
using System.Collections;

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
    //Punktet lasten skal slippes ned på
    Vector3 unloadPoint;
    //Om vi dirver og laster av eller på noe
    bool loading = false;
    bool unloading = false;

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
                        //Ja, vi skal laste av shit
                        unloading = true;
                        //Husker puntet vi skal sette lasten på
                        unloadPoint = GameManager.controlls.mouseWorldPosition;
                        //Turer dit
                        unit.cMoveable.SetTarget(unloadPoint);
                    }
                }
                //Om vi har planer om å laste av noe
                if(unloading)
                {
                    //Ser om vi er der driten skal
                    if(Vector3.Distance(transform.position, unloadPoint) < 1)
                    {
                        //Score!
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
            //Hvis vi holder på å løfte det 
            else if (loading)
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
            else if (Vector3.Distance(transform.position, parkPos) < 0.5)
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
                }
            }
        }
    }
	//Om uniten blir selected eller deselected 
	public void Select(bool selected)
    {
        //Husker på det, greit å vite liksom
        this.selected = selected;
    }
    //Når løfte animasjonen er ferdig
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
    }
}
