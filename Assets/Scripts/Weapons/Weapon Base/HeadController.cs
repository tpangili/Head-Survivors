using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadController : WeaponController
{
    [Header("Head Timer Info")]
    public float currentHeadTimer = 20;
    public float headTimerLow = 5;
    
    InventoryManager inventory;

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
        currentHeadTimer = weaponData.HeadTimer;
        headTimerLow = weaponData.HeadTimerLow;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        currentHeadTimer -= Time.deltaTime;
        // Displays in inventory how much time is left
        inventory.UpdateHeadTimerDisplay(currentHeadTimer, weaponIndex);

        // Head flashes red if timer is low
        if(currentHeadTimer <= headTimerLow && !isFlashing && !finishedFlash)
        {
            isFlashing = true;
            StartCoroutine(FlashRed());
        }

        // Stops head from flashing red if timer is restored
        if(currentHeadTimer > headTimerLow)
        {
            StopCoroutine(FlashRed());
            isFlashing = false;
            finishedFlash = false;
            sr.color = originalColor;
        }

        if(currentHeadTimer <= 0 && !isFlashing)
        {
            // Removes head from inventory and destroys it
            StartCoroutine(KillFade());
        }
    }

    protected override void Attack()
    {
        base.Attack();
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
