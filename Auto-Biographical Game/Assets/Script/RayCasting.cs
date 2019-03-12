using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCasting : MonoBehaviour
{

    public float rayDistance = 2;

    void Update()
    {
        Ray myRay = new Ray(this.transform.position, Vector3.down);

        Debug.DrawRay(myRay.origin, new Vector3(0, -rayDistance, 0), Color.red);

        if(Physics.Raycast(myRay, rayDistance))
        {
           
        }
    }
}
