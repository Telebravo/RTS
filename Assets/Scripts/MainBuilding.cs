using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainBuilding : MonoBehaviour {
    public GameObject panel; 
    public void SelectedMain(bool selected)
    {
        if(selected == true)
        {
            panel.SetActive(true);
        }
        else
        {
            panel.SetActive(false);
        }
    }

}
