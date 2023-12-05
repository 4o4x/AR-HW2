using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public float moveSpeed = 15.0f;
    public float mouseSensitivity = 30.0f;
    public float scrollSpeed = 50.0f;
    public float verticalMovementSpeed = 15.0f;

    void Update()
    {
        CameraMovements();
        CameraRotation();
        CameraZoom();
        CameraVerticalMovement();
    }

    void CameraMovements()
    {
        float horizontalMovement = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float verticalMovement = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        transform.Translate(new Vector3(horizontalMovement, 0, verticalMovement));
    }

    void CameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up, mouseX);
        transform.Rotate(Vector3.left, mouseY);
    }

    void CameraZoom()
    {
        float scrollMovement = Input.GetAxis("Mouse ScrollWheel") * scrollSpeed * Time.deltaTime;

        Camera.main.fieldOfView += scrollMovement * -1f;
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 10f, 80f);
    }


    void CameraVerticalMovement()
    {
        float verticalMovement = 0f;

        if (Input.GetKey(KeyCode.E))
        {
            verticalMovement = -verticalMovementSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            verticalMovement = verticalMovementSpeed * Time.deltaTime;
        }

        transform.Translate(0, verticalMovement, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    // void Update()
    // {

    //     int speed = 5;
    //     if(Input.GetKey(KeyCode.A)){
    //         transform.position += new Vector3(-1*speed*Time.deltaTime,0,0);
    //     }

    //     if(Input.GetKey(KeyCode.W)){
    //         transform.position += new Vector3(0,0,1*speed*Time.deltaTime);
    //     }

    //     if(Input.GetKey(KeyCode.S)){
    //         transform.position += new Vector3(0,0,-1*speed*Time.deltaTime);
    //     }

    //     if(Input.GetKey(KeyCode.D)){
    //         transform.position += new Vector3(1*speed*Time.deltaTime,0,0);
    //     }
    // }
}
