using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerBullet : Bullet
{
    public int headDamage;
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // Handles a regular bullet doing damage
        if (other.CompareTag("Hitbox"))
        {
            //Debug.Log("Hit!");
            PlayerStats player = other.gameObject.GetComponent<PlayerStats>();
            InventoryManager inventory = other.gameObject.GetComponent<InventoryManager>();
            if (player.isInvincible)
            {
                return;
            }
            Destroy(gameObject);
            inventory.DepleteHeadTimers(headDamage);
        }
    }
}
