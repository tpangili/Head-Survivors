using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingAnimation : MonoBehaviour
{
    public float frequency; // speed of movement
    public float magnitude; // range of movement
    public Vector3 direction; // direction of movement
    Vector3 initialPosition;
    Pickup pickup;

    // Start is called before the first frame update
    void Start()
    {
        // Save starting position of gameobject
        initialPosition = transform.position;
        pickup = GetComponent<Pickup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!pickup.isMagnetized)
        {
            transform.position = initialPosition + direction * Mathf.Sin(Time.time * frequency) * magnitude;
        }
    }
}
