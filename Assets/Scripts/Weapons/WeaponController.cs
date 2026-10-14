using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")]
    public GameObject prefab;
    public float damage;
    public float speed;
    public float cooldownDuration;
    private float currentCooldown;
    public int pierce;      // max times weapon can hit enemy before destroyed

    protected PlayerController pm;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        pm = FindObjectOfType<PlayerController>();
        // Weapon has to wait for cooldown before activating
        currentCooldown = cooldownDuration;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        // Checks if cooldown is finished
        if(currentCooldown <= 0f)
        {
            Attack();
        }
    }

    protected virtual void Attack()
    {
        // Reset cooldown after attack
        currentCooldown = cooldownDuration;
    }
}
