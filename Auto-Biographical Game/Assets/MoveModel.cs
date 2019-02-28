using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveModel : MonoBehaviour
{

    public float jumpHoldGravity;
    public float jumpGravity;
    public float fallGravity;


    public Vector3 velocity;

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

   

    public int speed;

    public float sensitivityX, sensitivityY;
    private float currentX, currentY;

    private Vector3 movement;

    private Vector3 Gravity = (Physics.gravity * 0.69f);

    public KeyCode jump;
    public float jumpForce;

    private bool onGround = false;

    public Vector3 camOffset;

    public float downRayDistance;
    public float wallRayDistance;

    // Start is called before the first frame update
    void Start()
    {
       Physics.gravity = new Vector3(0,fallGravity,0);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //transform.position = Vector3.zero;   
    }
    Vector3 rayDir = Vector3.zero;
    // Update is called once per frame
    void Update()
    {
        velocity = rb.velocity;

        /* if (Physics.Raycast(myRay, rayDistance))
         {

         }*/


        CameraControlFunction();
        MovementFunction();
        RayCasting();
        JumpFunction();
    }

    void RayCasting()
    {
        if (movement != Vector3.zero)
        {
            rayDir = movement;
        }
        Ray downRay = new Ray(this.transform.position, Vector3.down);
        Ray wallRay = new Ray(this.transform.position, rayDir);

        Debug.DrawRay(downRay.origin, new Vector3(0, -downRayDistance, 0), Color.red);
        Debug.DrawRay(wallRay.origin, rayDir * wallRayDistance, Color.red);

        RaycastHit hit;

        if (Physics.Raycast(downRay.origin, downRay.direction, out hit, downRayDistance))
        {
            if (hit.transform.gameObject.tag == "ground")
            {
                Debug.Log("ON GROUND");
                onGround = true;
            }
        }
        else if (Physics.Raycast(wallRay, out hit, wallRayDistance))
        {
            if (hit.transform.gameObject.tag == "ground")
            {
                Debug.Log("ON WALL");
            }
        }
    }

    void CameraControlFunction()
    {
        //3rd Person Camera Controls
        currentX += Input.GetAxis("Mouse X") * sensitivityX;
        currentY += Input.GetAxis("Mouse Y") * sensitivityY;

        camera.localPosition = new Vector3(0, 0, -camDistance);

        camera.LookAt(centerpoint);
        centerpoint.localRotation = Quaternion.Euler(-currentY, currentX, 0);

        currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);


        centerpoint.position = player.position + centerpoint.rotation * camOffset;

        //Player turns with the camera
        Quaternion turnAngle = Quaternion.Euler(0, centerpoint.eulerAngles.y, 0);
        player.rotation = Quaternion.Slerp(player.rotation, turnAngle, Time.deltaTime * 10);

       
    }

    void MovementFunction()
    {
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
        //movement.Normalize();

        movement = player.rotation * movement;
       // rb.velocity = (movement * Time.deltaTime * speed + Gravity);
        rb.AddForce(movement * Time.deltaTime * speed, ForceMode.Impulse);
    }

    void JumpFunction()
    {
        //use three separate gravities, one for holding space, one for tapping, one for when youre falling.
        //make a touching ground state/bool using (raycasting???) or colliders with tags 
        if (onGround)
        {

            if (Input.GetKeyDown(jump))
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                onGround = false;
                
            }
        }
        else
        {
            if (Input.GetKey(jump))
            {
                Physics.gravity = new Vector3(0, jumpHoldGravity, 0);
            }
            else
            {
                Physics.gravity = new Vector3(0, jumpGravity, 0);
            }
        }
        if (rb.velocity.y < 0)
        {
            Physics.gravity = new Vector3(0, fallGravity, 0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    { 
        if(collision.gameObject.tag == "ground")
        {
            onGround = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
       if(collision.gameObject.tag == "ground")
        {
            onGround = false;
        }
    }


}
