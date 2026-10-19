using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadPickup : Pickup, ICollectible
{
    public GameObject HeadToPickup;

    public void Collect()
    {
        //Debug.Log("HEAD ADDED");
        PlayerStats player = FindObjectOfType<PlayerStats>();
        player.SpawnWeapon(HeadToPickup);
        Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log("HEAD TRIGGER ENTERED");
        if (col.CompareTag("Player"))
        {
            //Debug.Log("HEAD PICKED UP BY PLAYER");
            Collect();
        }
    }
}
