using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;    // Position of target object
    public Vector3 offset;      // Offset for 2D position

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Camera follows target with standard offset for 2D position
        transform.position = target.position + offset;
    }
}
