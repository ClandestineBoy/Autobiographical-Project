using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveModel : MonoBehaviour
{
    public float Y_ANGLE_MIN;
    public float Y_ANGLE_MAX;

    public Transform player;
    public Transform camera;
    public Transform centerpoint;

    public float camDistance = 3;

    public Rigidbody rb;

    public KeyCode forward;
    public KeyCode back;
    public KeyCode left;
    public KeyCode right;

    public KeyCode jump;
    public float jumpForce;

    public int speed;

    public float sensitivityX, sensitivityY;
    private float currentX, currentY;

    private Vector3 movement;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //transform.position = Vector3.zero;   
    }

    // Update is called once per frame
    void Update()
    {
        //3rd Person Camera Controls
        currentX += Input.GetAxis("Mouse X") * sensitivityX;
        currentY += Input.GetAxis("Mouse Y") * sensitivityY;

        camera.localPosition = new Vector3(0, 0, -camDistance);

        camera.LookAt(centerpoint);
        centerpoint.localRotation = Quaternion.Euler(-currentY, currentX, 0);

        currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);


        centerpoint.position = player.position + centerpoint.rotation * new Vector3(-1.5f,.5f,0);

        //Player turns with the camera
        Quaternion turnAngle = Quaternion.Euler(0, centerpoint.eulerAngles.y, 0);
        player.rotation = Quaternion.Slerp(player.rotation, turnAngle, Time.deltaTime * 10);

        //Player Keyboard Controls
        movement = Vector3.zero;
        if (Input.GetKey(forward))
        {
            movement += Vector3.forward;
        }
        if (Input.GetKey(back))
        {
            movement += Vector3.back;
        }
        if (Input.GetKey(left))
        {
            movement += Vector3.left;
        }
        if (Input.GetKey(right))
        {
            movement += Vector3.right;
        }

        movement = player.rotation * movement;
        rb.AddForce(movement * Time.deltaTime * speed + Gravity);
    }
    Vector3 Gravity = (Physics.gravity * 0.4f);
}
