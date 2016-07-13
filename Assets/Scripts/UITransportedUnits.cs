using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UITransportedUnits : MonoBehaviour
{
    public GameObject panel;
    public GameObject unitPanelPrefab;
    public int numberOfPanels;
    public List<GameObject> panels;
    public Transport transport;
    public bool autoToggle = true;
    private Unit unit;
    private bool visible = true;
    private int prevUnitCount = 0;
    public static UITransportedUnits self;

    void Start()
    {
        self = this;
        panels = new List<GameObject>();
        for (int i = 0; i < numberOfPanels; i++)
        {
            panels.Add(GameObject.Instantiate(unitPanelPrefab));
            panels[i].transform.SetParent(panel.transform, false);
            panels[i].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, i * 50, 50);
            if (panels[i].activeInHierarchy)
                panels[i].SetActive(false);
        }
    }

    void Update()
    {
        if (GameManager.selectedUnit == null)
        {
            if (panel.activeInHierarchy)
                panel.SetActive(false);
        }
        else
        {
            transport = GameManager.selectedUnit.GetComponent<Transport>();
            if (transport == null)
            {
                if (panel.activeInHierarchy)
                    panel.SetActive(false);
            }
            else
            {
                if (!panel.activeInHierarchy)
                    panel.SetActive(true);

                if (autoToggle)
                {
                    if (transport.items.Count == 0)
                        Hide();
                    else if (transport.items.Count != prevUnitCount)
                    {
                        Show();
                        prevUnitCount = transport.items.Count;
                    }
                }
                for (int i = 0; i < transport.items.Count; i++)
                {
                    if (!panels[i].activeInHierarchy)
                        panels[i].SetActive(true);

                    unit = transport.items[i].unit;
                    panels[i].transform.GetComponentInChildren<Text>().text = unit.displayName;

                    //Forholet mellom maks-størrelsen på helathbaren og hvor mye liv uniten kan ha
                    float n = 155f / unit.startHealth;

                    //Setter størrelsen på helath baren
                    panels[i].transform.FindChild("HealthBar").transform.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 42, unit.currentHealth * n);
                    panels[i].transform.FindChild("Icon").transform.GetComponent<Image>().sprite = unit.icon;
                }
                for (int i = transport.items.Count; i < numberOfPanels; i++)
                {
                    if (panels[i].activeInHierarchy)
                        panels[i].SetActive(false);
                }
            }
        }
    }

    public void ToggleButtonClicked()
    {
        visible = !visible;
        if (visible)
            Show();
        else
            Hide();
    }
    void Show()
    {
        visible = true;
        panel.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 200);
    }
    void Hide()
    {
        visible = false;
        panel.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -200, 200);
    }

    public void UnitPanelClicked(RectTransform rect)
    {
        int index = self.panels.IndexOf(rect.gameObject);
        Debug.Log(index);
        Debug.Log(self.transport.items.Count);
        self.transport.Remove(self.transport.items[index]);
    }
}
