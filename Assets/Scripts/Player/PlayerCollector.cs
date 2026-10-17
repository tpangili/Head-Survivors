using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    PlayerStats player;
    CircleCollider2D playerCollector;
    public float pullSpeed;

    void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        playerCollector = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        playerCollector.radius = player.currentMagnet;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Check if other game object has ICollectible interface
        if(col.gameObject.TryGetComponent(out Pickup collectible))
        {
            // Pulls item towards player
            /*Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();
            Vector2 forceDir = (transform.position - col.transform.position).normalized;
            rb.AddForce(forceDir * pullSpeed);*/
            collectible.isMagnetized = true;

            // Item is collected
            //collectible.Collect();
        }
    }
}
