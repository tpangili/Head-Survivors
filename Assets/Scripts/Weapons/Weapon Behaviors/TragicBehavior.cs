using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TragicBehavior : ProjectileWeaponBehavior
{
    public GameObject target = null;
    public int rotationSpeed = 300;
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
        transform.position += transform.right * currentSpeed * Time.deltaTime;
        if (target != null)
        {
            Vector3 Vector2Enemy = (target.transform.position - transform.position).normalized;
            Vector3 cross = Vector3.Cross(Vector3.forward, Vector2Enemy);
            float thetaMult = -Vector3.Dot(transform.right, cross);
            transform.Rotate(Vector3.forward, thetaMult * Time.deltaTime * rotationSpeed);
        }
    }
}
