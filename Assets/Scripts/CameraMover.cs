using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraMover : MonoBehaviour
{

    public float CameraSpeed1;
    private float CameraSpeed;
    public float ZoomSpeed;

    public float SideBuffer;
    public float TopDownBuffer;

    private float tempy;

    Vector3 MousePosStart;
    Vector3 InitialTransformRot;
    public float CameraRotationSpeed;



    void Update()
    {
        // deb.text = Input.inputString;
        if (SideBuffer > Input.mousePosition.x || Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow))
        {
            //transform.position = new Vector3( transform.position.x - CameraSpeed , transform.position.y, transform.position.z);
            //transform.position -= new Vector3(CameraSpeed, 0, 0);
            transform.Translate(new Vector3(-CameraSpeed, 0, 0), Space.Self);
        }
        else if (Screen.width - SideBuffer < Input.mousePosition.x || Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow))
        {
            //transform.position = new Vector3(transform.position.x + CameraSpeed, transform.position.y, transform.position.z);
            //transform.position += new Vector3(CameraSpeed, 0, 0);
            transform.Translate(new Vector3(CameraSpeed, 0, 0), Space.Self);
        }

        if ((TopDownBuffer > Input.mousePosition.y || Input.GetKey("s") || Input.GetKey(KeyCode.DownArrow)) && !Input.GetKey(KeyCode.LeftShift))
        {
            //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - CameraSpeed);
            //transform.position -= new Vector3(0, 0, CameraSpeed);
            tempy = transform.position.y;
            transform.Translate(new Vector3(0, 0, - CameraSpeed), Space.Self);
            transform.position += new Vector3(0,tempy - transform.position.y,0);
        }
        else if ((Screen.height - TopDownBuffer < Input.mousePosition.y || Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow)) && !Input.GetKey(KeyCode.LeftShift))
        {
            //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + CameraSpeed);
            //transform.position += new Vector3(0, 0, CameraSpeed);
            tempy = transform.position.y;
            transform.Translate(new Vector3(0, 0, CameraSpeed), Space.Self);
            transform.position += new Vector3(0, tempy - transform.position.y, 0);
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey("s"))
        {
            //transform.position = new Vector3( transform.position.x - CameraSpeed , transform.position.y, transform.position.z);
            //transform.position -= new Vector3(CameraSpeed, 0, 0);
            transform.Translate(new Vector3(0, -CameraSpeed, 0), Space.World);
        }
        else if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey("w"))
        {
            //transform.position = new Vector3( transform.position.x - CameraSpeed , transform.position.y, transform.position.z);
            //transform.position -= new Vector3(CameraSpeed, 0, 0);
            transform.Translate(new Vector3(0, +CameraSpeed, 0), Space.World);
        }


            if (Input.GetMouseButtonDown(2))
        {
            MousePosStart = Input.mousePosition;

            InitialTransformRot = transform.rotation.eulerAngles;
        }
        if (Input.GetMouseButton(2))
        {
            transform.rotation = Quaternion.Euler( InitialTransformRot.x + (MousePosStart.y - Input.mousePosition.y) * CameraRotationSpeed, InitialTransformRot.y - (MousePosStart.x - Input.mousePosition.x) * CameraRotationSpeed, transform.rotation.z);
        }
        if (Input.mouseScrollDelta.y < 0 || (transform.position.y > 1 && Input.mouseScrollDelta.y > 0) )
        {
            transform.Translate(new Vector3(0, 0, Input.mouseScrollDelta.y * ZoomSpeed * transform.position.y));
        }
        CameraSpeed = CameraSpeed1 * transform.position.y;
    }
}