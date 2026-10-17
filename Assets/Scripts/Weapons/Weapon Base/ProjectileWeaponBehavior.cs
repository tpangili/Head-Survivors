using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
Base script for all projectile behaviors
[Placed on a prefab of a projectile weapon]
*/

public class ProjectileWeaponBehavior : MonoBehaviour
{
    public WeaponScriptableObject weaponData;
    protected Vector3 direction;      // direction projectile goes
    public float destroyAfterSeconds; // how long projectile lasts on field

    // Current stats
    protected float currentDamage;
    protected float currentSpeed;
    protected float currentCooldownDuration;
    protected int currentPierce;

    void Awake()
    {
        // Initialize weapon stats
        currentDamage = weaponData.Damage;
        currentSpeed = weaponData.Speed;
        currentCooldownDuration = weaponData.CooldownDuration;
        currentPierce = weaponData.Pierce;
    }

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

    // Handles dealing damage to enemy
    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.TakeDamage(currentDamage);
            ReducePierce();
        }
        else if(col.CompareTag("Prop"))
        {
            if(col.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(currentDamage);
                ReducePierce();
            }
        }
    }

    void ReducePierce()
    {
        currentPierce--;
        if(currentPierce <= 0)
        {
            Destroy(gameObject);
        }
    }
}
