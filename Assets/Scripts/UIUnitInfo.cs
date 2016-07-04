using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIUnitInfo : MonoBehaviour
{
    //Units
    public GameObject units;
    public GameObject unitPanel;
    public static List<GameObject> unitPanels;

    //Selected unit info
    public GameObject[] unitInfoPanels;
    public Image unitInfoImage;
    public List<Text> unitInfoText;
    static UIUnitInfo self;
    static float lastClick = 0f;


	void Start ()
    {
        self = this;

        unitPanels = new List<GameObject>();
        for (int i = 0; i < GameManager.controlls.selectionLimit; i++)
        {
            unitPanels.Add(Instantiate(unitPanel));
            unitPanels[i].transform.name = "Unit " + i.ToString();
            unitPanels[i].transform.SetParent(units.transform, false);
        }
	}
    void Update()
    {
        UpdateUI();
    }
    public void UpdateUI()
    {
        //Unit info
        if (GameManager.selectedUnit != null)
        {
            for (int i = 0; i < unitInfoPanels.Length; i++)
            {
                if (!unitInfoPanels[i].activeInHierarchy)
                    unitInfoPanels[i].SetActive(true);
            }

            unitInfoImage.gameObject.SetActive(true);
            unitInfoImage.sprite = GameManager.selectedUnit.icon;

            unitInfoText[0].text = GameManager.selectedUnit.displayName.ToString();
            unitInfoText[1].text = GameManager.selectedUnit.cHealth.currentHealth.ToString();

            if (GameManager.selectedUnit.weapon != null)
            {
                unitInfoText[2].text = GameManager.selectedUnit.weapon.name;
                unitInfoText[3].text = GameManager.selectedUnit.weapon.ammoSize.ToString();
            }
            else
            {
                unitInfoText[2].text = "-";
                unitInfoText[3].text = "-";
            }
            unitInfoText[4].text = GameManager.selectedUnit.armor.ToString();
            unitInfoText[5].text = GameManager.selectedUnit.movementSpeed.ToString();
        }
        else
        {
            for (int i = 0; i < unitInfoPanels.Length; i++)
            {
                if(unitInfoPanels[i].activeInHierarchy)
                    unitInfoPanels[i].SetActive(false);
            }
        }

        //Skjuler panelene som ikke er i bruk
        for (int i = GameManager.controlls.selectedUnits.Count; i < GameManager.controlls.selectionLimit; i++)
        {
            if (unitPanels[i].activeInHierarchy)
                unitPanels[i].SetActive(false);
        }

        if(GameManager.controlls.selectedUnits.Count <= 0)
        {
            return;
        }

        float unitPanelWidth = units.GetComponent<RectTransform>().rect.width-100;
        float widthPerPanel = Mathf.Min(unitPanelWidth / GameManager.controlls.selectedUnits.Count, 100);

        //Units 
        Unit unit;
        for (int i = 0; i < GameManager.controlls.selectedUnits.Count; i++)
        {
            //Aktiverer panelet
            if(!unitPanels[i].activeInHierarchy)
                unitPanels[i].SetActive(true);

            //Setter posisjonen
            unitPanels[i].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, i * widthPerPanel, 100);

            //Henter unit komponenten
            unit = GameManager.controlls.selectedUnits[i].GetComponent<Unit>();

            //Setter navnet
            unitPanels[i].transform.GetComponentInChildren<Text>().text = unit.displayName;

            //Forholet mellom maks-størrelsen på helathbaren og hvor mye liv uniten kan ha
            float n = 96f / unit.startHealth;

            //Setter størrelsen på helath baren
            unitPanels[i].transform.FindChild("HealthBar").transform.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 2, unit.cHealth.currentHealth * n);
            unitPanels[i].transform.FindChild("Icon").transform.GetComponent<Image>().sprite = unit.icon;
        }
    }
    public void SelectUnit(RectTransform rTransform)
    {
        GameManager.selectedUnit = GameManager.controlls.selectedUnits[unitPanels.IndexOf(rTransform.gameObject)].GetComponent<Unit>();

        if(Time.time - lastClick < 0.2f && GameManager.selectedUnit)
        {
            GameManager.controlls.DeselectAll();
            GameManager.controlls.Select(GameManager.selectedUnit);
        }
        lastClick = Time.time;
    }
    public static void SelectionChanged()
    {
        if(GameManager.controlls.selectedUnits.Count == 0)
        {
            GameManager.selectedUnit = null;
        }
        else
        {
            if(GameManager.selectedUnit == null || !GameManager.controlls.selectedUnits.Contains(GameManager.selectedUnit))
                GameManager.selectedUnit = GameManager.controlls.selectedUnits[0].GetComponent<Unit>();
        }
       self.UpdateUI();
    }
}