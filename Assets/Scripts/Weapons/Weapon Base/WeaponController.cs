using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")]
    public WeaponScriptableObject weaponData;
    private float currentCooldown;
    protected int weaponIndex; // NOTE: set in AddWeapon() function, used mainly for head timer ui display

    protected PlayerController pm;

    public Sprite restSprite;
    public Sprite attackSprite;
    public SpriteRenderer headDisplay;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        pm = FindObjectOfType<PlayerController>();
        // Weapon has to wait for cooldown before activating
        currentCooldown = weaponData.CooldownDuration;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        // Checks if cooldown is finished
        if(currentCooldown <= 0f)
        {
            Attack();
        }
    }

    protected virtual void Attack()
    {
        // Reset cooldown after attack
        currentCooldown = weaponData.CooldownDuration;
        if (attackSprite != null && restSprite != null)
        {
            StartCoroutine(AttackAnim());
        }
    }

    IEnumerator AttackAnim()
    {
        headDisplay.sprite = attackSprite;
        yield return new WaitForSeconds(2);
        headDisplay.sprite = restSprite;
    }
}
