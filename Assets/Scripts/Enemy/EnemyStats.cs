using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;
    EnemySpawner spawner;

    // Current stats
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentDamage;

    public float despawnDistance = 20f;
    Transform player;

    [Header("Damage Feedback")]
    public Color damageColor = new Color(1,0,0,1); // what the color of damage flash should be
    public float damageFlashDuration = 0.2f; // how long the flash should last
    public float deathFadeTime = 0.6f; // how much time it takes for the enemy to fade
    Color originalColor;
    SpriteRenderer sr;
    EnemyMovement movement;

    // Flag for enemy death
    bool isDying = false;

    void Awake()
    {
        // Initializes enemy stats
        currentMoveSpeed = enemyData.MoveSpeed;
        currentHealth = enemyData.MaxHealth;
        currentDamage = enemyData.Damage;
    }

    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        movement = GetComponent<EnemyMovement>();
        spawner = FindObjectOfType<EnemySpawner>();
    }

    void Update()
    {
        if(Vector2.Distance(transform.position, player.position) >= despawnDistance)
        {
            ReturnEnemy();
        }
    }
    
    // This function always needs at least 2 values: the amount of damage dealt (dmg)
    // and where the damage is coming from (sourcePosition)
    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f)
    {
        currentHealth -= dmg;
        StartCoroutine(DamageFlash());

        // Create the text popup when enemy takes damage.
        if(dmg > 0 && !isDying)
        {
            //Debug.Log("about to generate floating text");
            GameManager.GenerateFloatingText(Mathf.FloorToInt(dmg).ToString(), transform);
            //Debug.Log("Floating text generated");
        }

        // Apply knockback if it is not zero
        if(knockbackForce > 0 && !isDying)
        {
            // Gets direction of knockback
            Vector2 dir = (Vector2)transform.position - sourcePosition;
            movement.Knockback(dir.normalized * knockbackForce, knockbackDuration);
        }

        if(currentHealth <= 0)
        {
            Kill();
        }
    }

    // Coroutine function that makes the enemy flash when taking damage
    IEnumerator DamageFlash()
    {
        sr.color = damageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        sr.color = originalColor;
    }

    public void Kill()
    {
        /*EnemySpawner es = FindObjectOfType<EnemySpawner>();
        es.OnEnemyKilled();
        Destroy(gameObject);*/
        isDying = true;
        StartCoroutine(KillFade());
    }

    IEnumerator KillFade()
    {
        // Waits for a single frame
        WaitForEndOfFrame w = new WaitForEndOfFrame();  // used for one frame delays
        float t = 0, origAlpha = sr.color.a;

        // Fires every frame to smoothly translate alpha value to 0
        while(t < deathFadeTime)
        {
            yield return w;
            t += Time.deltaTime;

            // Set the color for this frame
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (1 - t / deathFadeTime) * origAlpha);
        }
        spawner.OnEnemyKilled();

        //Debug.Log("enemy about to be destroyed");
        Destroy(gameObject);
        //Debug.Log("enemy destroyed");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("COLLIDE");
        // Reference the script from the collided collider and deal damage using TakeDamage()
        if(collision.gameObject.CompareTag("Hitbox"))
        {
            PlayerStats player = collision.gameObject.GetComponent<PlayerStats>();
            if (!player.isInvincible)
            {
                player.TakeDamage(currentDamage);
            }
        }
    }

    void ReturnEnemy()
    {
        EnemySpawner es = FindObjectOfType<EnemySpawner>();
        transform.position = player.position + es.relativeSpawnPoints[Random.Range(0, es.relativeSpawnPoints.Count)].position;
    }
}
