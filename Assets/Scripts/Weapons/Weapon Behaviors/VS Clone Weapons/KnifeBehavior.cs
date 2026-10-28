using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeBehavior : ProjectileWeaponBehavior
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        // Set movement of knife
        // NOTE: Apply the projectile speed multiplier to each projectile weapon's
        // initial speed when making more of those
        currentSpeed = GetCurrentProjectileSpeed();
        transform.position += direction * currentSpeed * Time.deltaTime;
    }
}
