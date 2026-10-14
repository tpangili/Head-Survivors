using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : WeaponController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedKnife = Instantiate(prefab);
        // Position assigned to same as object parented to player
        // That way it spawns from the same spot within the player
        spawnedKnife.transform.position = transform.position;
        // Reference and set direction based on player direction
        spawnedKnife.GetComponent<KnifeBehavior>().DirectionChecker(pm.lastMovedVector);
    }
}
