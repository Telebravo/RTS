using UnityEngine;
using System.Collections;

[AddComponentMenu("Unit/CSelectable")]
[RequireComponent(typeof(Unit))]
public class CSelectable : MonoBehaviour
{
    [HideInInspector]
    public Unit unit;

    [HideInInspector]
    public bool selected = false;

    public Transform selectionCanvas;
    public string CallComponent;
    public string CallFunction;

    void Selected()
    {
        Debug.Log(gameObject.name + " selected");

        selected = true;
        selectionCanvas.gameObject.SetActive(true);

        if (!string.IsNullOrEmpty(CallComponent) && !string.IsNullOrEmpty(CallFunction))
        {
            Component component = GetComponent(CallComponent);
            component.SendMessage(CallFunction, selected);
        }
    }

    void Deselected()
    {
        Debug.Log(gameObject.name + " deselected");

        selected = false;
        selectionCanvas.gameObject.SetActive(false);

        if (!string.IsNullOrEmpty(CallComponent) && !string.IsNullOrEmpty(CallFunction))
        {
            Component component = GetComponent(CallComponent);
            component.SendMessage(CallFunction, selected);
        }
    }

    void OnBecameVisible()
    {
        GameManager.controlls.SetVisible(unit, true);
    }

    void OnBecameInvisible()
    {
        GameManager.controlls.SetVisible(unit, false);
    }

    void Update()
    {
        Vector3 directionToCamera = Camera.main.transform.position - transform.position;

        Quaternion newRotation = Quaternion.LookRotation(directionToCamera);
        selectionCanvas.rotation = newRotation;
    }
}