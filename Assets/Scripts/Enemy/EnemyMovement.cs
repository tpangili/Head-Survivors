using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    EnemyStats enemy;
    Transform player;

    // Variables for knockback when enemies are damaged
    Vector2 knockbackVelocity;
    float knockbackDuration;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
        enemy = GetComponent<EnemyStats>();
    }

    // Update is called once per frame
    void Update()
    {
        // If currently being knocked back, proceed with the knockback
        if(knockbackDuration > 0)
        {
            transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
            knockbackDuration -= Time.deltaTime;
        }
        else
        {
            // Constantly moves enemy towards player
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemy.currentMoveSpeed * Time.deltaTime);
        }
    }

    public void Knockback(Vector2 velocity, float duration)
    {
        // Ignore the knockback if the duration is greater than 0
        if(knockbackDuration > 0)
        {
            return;
        }

        // Begins the knockback
        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }
}
