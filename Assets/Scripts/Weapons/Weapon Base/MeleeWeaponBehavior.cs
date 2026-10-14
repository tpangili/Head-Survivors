using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
Base script for all projectile behaviors
[Placed on a prefab of a melee weapon]
*/

public class MeleeWeaponBehavior : MonoBehaviour
{
    public float destroyAfterSeconds;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }
}
