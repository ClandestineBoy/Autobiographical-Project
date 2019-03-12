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
    private bool onWall = false;

    private Transform currentWall;

    public Vector3 camOffset;

    public float downRayDistance;
    public float wallRayDistance;

    private Vector3 wallJumpNormal;

    Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
       Physics.gravity = new Vector3(0,fallGravity,0);
        wallJumpNormal = Vector3.zero;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //transform.position = Vector3.zero;   
    }
    Vector3 rayDir = Vector3.zero;
    // Update is called once per frame
    void Update()
    {
        velocity = movement;

        /* if (Physics.Raycast(myRay, rayDistance))
         {

         }*/
         if(transform.position.y < -20)
        {
            transform.position = startPos;
        }

        CameraControlFunction();
        MovementFunction();
        RayCasting();
        JumpFunction();
    }

    void RayCasting()
    {
        if (onWall)
        {
            rayDir = new Vector3(currentWall.position.x - transform.position.x, 0, currentWall.position.z - transform.position.z);
        }
        else if (movement != Vector3.zero)
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
            if (hit.transform.gameObject.tag == "ground" && onGround)
            {
                //Debug.Log("ON GROUND");
                onGround = true;
                onWall = false;
            }
        }
        else if (Physics.Raycast(wallRay, out hit, wallRayDistance))
        {
            if (hit.transform.gameObject.tag == "ground")
            {
                //Debug.Log("ON WALL");
                if (onGround)
                {
                    onWall = true;
                    onGround = false;
                }
                wallJumpNormal = hit.normal;
                currentWall = hit.transform;
            }
        }
        else
        {
            onWall = false;
        }
    }


    bool onFlat = false;
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


        Vector3 wallRotation = wallJumpNormal;

        //Player turns with the camera
        Quaternion turnAngle = Quaternion.Euler(0, centerpoint.eulerAngles.y, 0); ;
        if (onWall)
        {
            if(wallJumpNormal.z == 0)
            {
                onFlat = true;
                if (rb.velocity.z > 0)
                {
                    turnAngle = Quaternion.Euler(0, 0, 0);
                }
                if(rb.velocity.z < 0)
                {
                    turnAngle = Quaternion.Euler(0, 180, 0);
                }
            }
            else if (wallJumpNormal.x == 0)
            {
                onFlat = true;
                if (rb.velocity.x > 0)
                {
                    turnAngle = Quaternion.Euler(0, 90, 0);
                }
                if (rb.velocity.x < 0)
                {
                    turnAngle = Quaternion.Euler(0, -90, 0);
                }
            }
            else
            {
                onFlat = false;
                //the normal of the face the player is colliding with is the hypotenuse in a triange, where that normal's x and z are the triangle's legs.
                //if we find the cotangent of the angle where the player is colliding, and subtaract it from either 0, 90, 180, or 270 (depending the normal's direction)
                //we can rotate the player object to be perpendicular to the normal of the face
                float perpNormal = 90 - Mathf.Rad2Deg*Mathf.Atan(wallJumpNormal.x / wallJumpNormal.z);
                Debug.Log(Mathf.Abs(rb.velocity.normalized.x) + "    " + Mathf.Abs(wallJumpNormal.x));

                if(wallJumpNormal.z < 0)
                {
                    if (rb.velocity.x > 0 && rb.velocity.z > 0 || rb.velocity.x > 0 && rb.velocity.z < 0)
                    {
                        clockWise = false;
                        turnAngle = Quaternion.Euler(0, 180 - perpNormal - 20, 0);
                    }
                    if (rb.velocity.x < 0 && rb.velocity.z < 0 || rb.velocity.x < 0 && rb.velocity.z > 0)
                    {
                        clockWise = true;
                        turnAngle = Quaternion.Euler(0,-perpNormal + 20, 0);
                    }
                }
                if (wallJumpNormal.z > 0)
                {
                    if (rb.velocity.x > 0 && rb.velocity.z > 0 || rb.velocity.x > 0 && rb.velocity.z < 0)
                    {
                        clockWise = true;
                        turnAngle = Quaternion.Euler(0, 180 - perpNormal + 20, 0);
                    }
                    if(rb.velocity.x < 0 && rb.velocity.z < 0 || rb.velocity.x < 0 && rb.velocity.z > 0)
                    {
                        clockWise = false;
                        turnAngle = Quaternion.Euler(0,-perpNormal - 20, 0);
                    }
                }

            }

        }
        else
        {
            turnAngle = Quaternion.Euler(0, centerpoint.eulerAngles.y, 0);
        }
        player.rotation = Quaternion.Slerp(player.rotation, turnAngle, Time.deltaTime * 10);

      
       // Debug.Log(wallRotation);

    }

    private bool clockWise = false;

    void MovementFunction()
    {
        //Player Keyboard Controls
        if (!onWall)
        {
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
            movement.Normalize();
            movement = player.rotation * movement;
        }
        else
        {
            //rb.velocity = Vector3.zero;
            if (onFlat)
            {
                movement = player.rotation * new Vector3(0, 0, 1);
            }
            else if (clockWise)
                {
                    movement = player.rotation * new Vector3(.9f, 0, 1);
                }
            else
                {
                    movement = player.rotation * new Vector3(-.9f, 0, 1);
                }
            }
        
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
        else if (onWall)
        {
            Physics.gravity = Vector3.zero;
            if (Input.GetKeyDown(jump))
            {
                wallJumpNormal *= jumpForce / 2;
                wallJumpNormal.y = jumpForce;
                rb.AddForce(wallJumpNormal, ForceMode.Impulse);
                onWall = false;

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
            if (rb.velocity.y < 0)
            {
                Physics.gravity = new Vector3(0, fallGravity, 0);
            }
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
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Finish")
        {
            Debug.Log("END");
        }
    }

}
