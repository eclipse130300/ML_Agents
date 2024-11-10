using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour
{
    public float speed = 50.0f;
    public float rotationSpeed = 100.0f;
    public float visibleDistance = 200.0f;

    void Update()
    {
        // Get the horizontal and vertical axis.
        // By default they are mapped to the arrow keys.
        // The value is in the range -1 to 1
        float translationInput = Input.GetAxis("Vertical");
        float rotationInput = Input.GetAxis("Horizontal");
        
        float translation = Time.deltaTime * translationInput * speed;
        float rotation = Time.deltaTime * rotationInput * rotationSpeed;

        // Move translation along the object's z-axis
        transform.Translate(0, 0, translation);

        // Rotate around our y-axis
        transform.Rotate(0, rotation, 0);
        
        Debug.DrawRay(transform.position, transform.forward * visibleDistance, Color.red);
        Debug.DrawRay(transform.position, transform.right * visibleDistance, Color.red);
        
        RaycastHit hit;
        float fdist = visibleDistance,
              rdist = visibleDistance,
              ldist = visibleDistance,
              r45dist = visibleDistance,
              l45dist = visibleDistance;

        if (Physics.Raycast(transform.position, transform.forward, out hit, visibleDistance))
        {
            fdist = hit.distance;
        }
        
        if (Physics.Raycast(transform.position, transform.right, out hit, visibleDistance))
        {
            rdist = hit.distance;
        }
        
        if (Physics.Raycast(transform.position, -transform.right, out hit, visibleDistance))
        {
            ldist = hit.distance;
        }
        
        if (Physics.Raycast(transform.position, Quaternion.AngleAxis(45, transform.up) * transform.right, out hit, visibleDistance))
        {
            r45dist = hit.distance;
        }
        
        if (Physics.Raycast(transform.position, Quaternion.AngleAxis(- 45, transform.up) * -transform.right, out hit, visibleDistance))
        {
            l45dist = hit.distance;
        }
        
        string td = fdist + "," + rdist + "," + ldist + "," + r45dist + "," + l45dist + "," + translationInput + "," + rotationInput;
    }
}
