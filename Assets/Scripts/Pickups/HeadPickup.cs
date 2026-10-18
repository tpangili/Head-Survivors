using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadPickup : Pickup, ICollectible
{
    public GameObject HeadToPickup;

    public void Collect()
    {
        PlayerStats player = FindObjectOfType<PlayerStats>();
        player.SpawnWeapon(HeadToPickup);
        Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Collect();
        }
    }
}
