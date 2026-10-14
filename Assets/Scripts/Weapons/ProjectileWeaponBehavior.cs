using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
Base script for all projectile behaviors
[Placed on a prefab of a projectile weapon]
*/

public class ProjectileWeaponBehavior : MonoBehaviour
{
    protected Vector3 direction;      // direction projectile goes
    public float destroyAfterSeconds; // how long projectile lasts on field

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    public void DirectionChecker(Vector3 dir)
    {
        direction = dir;

        float dirx = direction.x;
        float diry = direction.y;

        Vector3 scale = transform.localScale;
        Vector3 rotation = transform.rotation.eulerAngles;

        // Flips direction of projectile if needed
        if(dirx < 0 && diry == 0)
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
        }

        transform.localScale = scale;
        transform.rotation = Quaternion.Euler(rotation);  // need to convert before setting
    }
}
