using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 1f;
    public float lifetime = 1f;
    public float rotation = 0f;
    public float speed = 1f;
    // Used for spreading bullets in row or line
    public bool spreadEnable = false;
    public float bulletSide;

    private Vector2 spawnPoint;
    private float timer = 0f;

    private void Awake()
    {
        // Destroys bullet after specified number of seconds
        Invoke(nameof(SelfDestruct), lifetime);
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = new Vector2(transform.position.x, transform.position.y);
        //Debug.Log("Spawn: " + spawnPoint);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.deltaTime;
        transform.position = Movement(timer);
    }

    private Vector2 Movement(float timer)
    {
        float x = timer * speed * transform.right.x;
        float y;
        if (spreadEnable)
        {
            y = timer * speed * transform.up.y * (bulletSide * -1);
            Vector2 spreadAngle = new Vector2(x+spawnPoint.x, y+spawnPoint.y);
            return spreadAngle.normalized;
        }
        else
        {
            y = timer * speed * transform.right.y;
        }
        return new Vector2(x+spawnPoint.x, y+spawnPoint.y);
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Handles a regular bullet doing damage
        if (other.CompareTag("Hitbox"))
        {
            //Debug.Log("Hit!");
            PlayerStats player = other.gameObject.GetComponent<PlayerStats>();
            if (player.isInvincible)
            {
                return;
            }
            Destroy(gameObject);
            player.TakeDamage(damage);
        }
    }
}
