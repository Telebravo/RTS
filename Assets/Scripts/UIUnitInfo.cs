using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIUnitInfo : MonoBehaviour
{
    public GameObject units;
    public GameObject unitPanel;
    public List<GameObject> unitPanels;
    public Image unitInfoImage;
    public List<Text> unitInfo;

	void Start ()
    {
        unitPanels = new List<GameObject>();
        for (int i = 0; i < GameManager.controlls.selectionLimit; i++)
        {
            unitPanels.Add(Instantiate(unitPanel));
            unitPanels[i].transform.SetParent(units.transform, false);
        }
	}
    void Update()
    {
        if (GameManager.controlls.selectedObjects.Count > 0)
        {
            GameManager.selectedUnit = GameManager.controlls.selectedObjects[0].GetComponent<Unit>();
        }
        else
        {
            GameManager.selectedUnit = null;
        }

        UpdateUI();
    }
    public void UpdateUI()
    {
        //Unit info
        if (GameManager.selectedUnit != null)
        {
            unitInfoImage.gameObject.SetActive(true);
            unitInfoImage.sprite = Sprite.Create(GameManager.selectedUnit.icon, new Rect(Vector2.zero, new Vector2(194, 259)), Vector2.zero);

            unitInfo[0].text = GameManager.selectedUnit.displayName.ToString();
            unitInfo[1].text = GameManager.selectedUnit.cHealth.currentHealth.ToString();

            if (GameManager.selectedUnit.weapon != null)
            {
                unitInfo[2].text = GameManager.selectedUnit.weapon.name;
                unitInfo[3].text = GameManager.selectedUnit.weapon.ammoSize.ToString();
            }
            else
            {
                unitInfo[2].text = "-";
                unitInfo[3].text = "-";
            }
            unitInfo[4].text = GameManager.selectedUnit.armor.ToString();
            unitInfo[5].text = GameManager.selectedUnit.movementSpeed.ToString();
        }
        else
        {
            unitInfoImage.gameObject.SetActive(false);
            for (int i = 0; i <= 5; i++)
            {
                unitInfo[i].text = "";
            }
        }

        //Units 
        Unit unit;
        for (int i = 0; i < GameManager.controlls.selectedObjects.Count; i++)
        {
            //Aktiverer panelet
            if(!unitPanels[i].activeInHierarchy)
                unitPanels[i].SetActive(true);

            //Setter posisjonen
            unitPanels[i].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, i * 100, 100);

            //Henter unit komponenten
            unit = GameManager.controlls.selectedObjects[i].GetComponent<Unit>();

            //Setter navnet
            unitPanels[i].transform.GetComponentInChildren<Text>().text = unit.displayName;

            //Forholet mellom maks-størrelsen på helathbaren og hvor mye liv uniten kan ha
            float n = 96f / unit.startHealth;

            //Setter størrelsen på helath baren
            unitPanels[i].transform.FindChild("HealthBar").transform.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 2, unit.cHealth.currentHealth*n);

        }
        //Skjuler panelene som ikke er i bruk
        for (int i = GameManager.controlls.selectedObjects.Count; i < GameManager.controlls.selectionLimit; i++)
        {
            if (unitPanels[i].activeInHierarchy)
                unitPanels[i].SetActive(false);
        }
    }
}