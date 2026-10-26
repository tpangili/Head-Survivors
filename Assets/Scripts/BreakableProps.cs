using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableProps : MonoBehaviour
{
    public float health;

    [Header("Damage Feedback")]
    public Color damageColor = new Color(1,0,0,1); // what the color of damage flash should be
    public float damageFlashDuration = 0.2f; // how long the flash should last
    public float deathFadeTime = 0.6f; // how much time it takes for the enemy to fade
    Color originalColor;
    SpriteRenderer sr;

    // Flag for prop destruction
    bool isDying = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;

        // Create the text popup when enemy takes damage.
        if(dmg > 0 && !isDying)
        {
            //Debug.Log("about to generate floating text");
            GameManager.GenerateFloatingText(Mathf.FloorToInt(dmg).ToString(), transform);
            //Debug.Log("Floating text generated");
        }

        if(health <= 0)
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

        Destroy(gameObject);
    }
}
