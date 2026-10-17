using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [HideInInspector]
    public bool isMagnetized = false;
    Transform player;
    PlayerCollector collector;

    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
        collector = FindObjectOfType<PlayerCollector>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isMagnetized)
        {
            // Constantly moves enemy towards player
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, collector.pullSpeed * Time.deltaTime);
        }
    }
}
