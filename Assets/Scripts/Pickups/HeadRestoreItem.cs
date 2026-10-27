using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadRestoreItem : Pickup, ICollectible
{
    public int headToRestore;

    public void Collect()
    {
        InventoryManager inventory = FindObjectOfType<InventoryManager>();
        inventory.RestoreHeadTimers(headToRestore);
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
