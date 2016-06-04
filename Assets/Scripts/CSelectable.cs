using UnityEngine;
using System.Collections;

public class CSelectable : MonoBehaviour
{
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
        if (!CameraControlls.visibleObjects.Contains(transform))
            CameraControlls.visibleObjects.Add(this.transform);
    }
    void OnBecameInvisible()
    {
        if (CameraControlls.visibleObjects.Contains(transform))
            CameraControlls.visibleObjects.Remove(this.transform);
    }
    void Update()
    {
        Vector3 directionToCamera = Camera.main.transform.position - transform.position;

        Quaternion newRotation = Quaternion.LookRotation(directionToCamera);
        selectionCanvas.rotation = newRotation;
    }
}