using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadPickup : Pickup, ICollectible
{
    public GameObject HeadToPickup;

    public void Collect()
    {
        Debug.Log("HEAD ADDED");
        PlayerStats player = FindObjectOfType<PlayerStats>();
        InventoryManager inventory = FindObjectOfType<InventoryManager>();
        // Check if head is already in weapon inventory (99 means NO)
        int index = inventory.CheckInventory(HeadToPickup.GetComponent<WeaponController>());
        //Debug.Log("Inventory checked");
        if(index == 99)
        {
            //Debug.Log("spawning weapon");
            player.SpawnWeapon(HeadToPickup);
        }
        else
        {
            //Debug.Log("Leveling up weapon");
            inventory.LevelUpWeapon(index, 99);
        }
        Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("HEAD TRIGGER ENTERED");
        if (col.CompareTag("Player"))
        {
            Debug.Log("HEAD PICKED UP BY PLAYER");
            Collect();
        }
    }
}
