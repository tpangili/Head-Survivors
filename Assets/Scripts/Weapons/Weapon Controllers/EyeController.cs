using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class EyeController : WeaponController
{
    public float headTimer = 30;
    public float headTimerLow = 10;
    InventoryManager inventory;

    [HideInInspector]
    public float theta = 0;

    [Header("Low Timer Flash Red")]
    SpriteRenderer sr;  // Sprite renderer for head controller
    float flashSeconds = 1.5f;          // How long head flashes red
    float flashSecondsDelta = 0.25f;    // Intervals between flashes
    Color flashColor = Color.red;
    Color originalColor;
    bool isFlashing = false;
    bool finishedFlash = false;

    public float deathFadeTime = 0.6f;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        inventory = FindObjectOfType<InventoryManager>();
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    protected override void Update()
    {
        base.Update();

        headTimer -= Time.deltaTime;

        // Head flashes red if timer is low
        if(headTimer <= headTimerLow && !isFlashing && !finishedFlash)
        {
            isFlashing = true;
            StartCoroutine(FlashRed());
        }

        if(headTimer <= 0 && !isFlashing)
        {
            // Removes head from inventory and destroys it
            StartCoroutine(KillFade());
        }
    }

    private void FixedUpdate()
    {
        transform.localPosition = new Vector3(0.7f * Mathf.Cos(theta), 0.4f * Mathf.Sin(theta) + 0.4f, 0);
        theta += Time.fixedDeltaTime / 2;
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedRay = Instantiate(weaponData.Prefab, this.transform);
        spawnedRay.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * theta + 90);
        spawnedRay.GetComponentInChildren<EyeBehavior>().theta = this.theta;
    }

    private IEnumerator FlashRed()
    {
        flashSeconds = headTimerLow;
        // Iterates through the seconds of flashing red
        for (float i = 0; i < flashSeconds; i += flashSecondsDelta)
        {
            // Makes player sprite flash during i frames
            if (sr.color == originalColor)
            {
                sr.color = flashColor;
            }
            else
            {
                sr.color = originalColor;
            }
            yield return new WaitForSeconds(flashSecondsDelta);
        }
        isFlashing = false;
        finishedFlash = true;
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

        inventory.RemoveWeapon(this);
    }
}
