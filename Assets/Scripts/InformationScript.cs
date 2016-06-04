using UnityEngine;
using System.Collections;
using UnityEngine.UI; 

public class InformationScript : MonoBehaviour
{
    public Text hitPoints;
    public Text unitName;
    public Text generalInfo;
    public Text timer;
    public float tid2;
    public int tid;
	
	void Update ()
    {
        timer.text = tid.ToString();
        if (GameManager.controlls.selectedObjects.Count > 0)
        {
            Unit unit = GameManager.controlls.selectedObjects[0].GetComponent<Unit>();
            if (unit != null)
            {
                hitPoints.text = "HP: " + unit.cHealth.currentHealth.ToString();//unit Health
                unitName.text = unit.displayName; //unit navn
                generalInfo.text = "Weapon: " + unit.weapon.ToString() + "\n" + "Optics: " + unit.optics.ToString() + "\n" + "Armor: " + unit.armor.ToString() + "\n" + "Movement Speed: " + unit.movementSpeed.ToString() + "\n" + "Range: " + unit.weapon.range.ToString() + "\n" + "Rate of fire: " + unit.weapon.firerate.ToString() + "\n" + "Damage: " + unit.weapon.damage.ToString() + " " + unit.weapon.damageType.ToString();
                //Lang string med generel info
            }
            else
            {
                Clear();
            }
        }
        else
        {
            Clear();
        }
    }
    void Clear()
    {
        hitPoints.text = "";
        unitName.text = "";
        generalInfo.text = "";
    }
}
