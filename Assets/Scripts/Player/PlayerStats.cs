using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    CharacterScriptableObject characterData;

    // Current player stats
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentMaxHealth;
    [HideInInspector]
    public float currentRecovery;
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentMight;
    [HideInInspector]
    public float currentProjectileSpeed;
    [HideInInspector]
    public float currentMagnet;

    // Spawned Weapon
    // NOTE: Heads equipped are classified as weapons
    //public List<GameObject> spawnedWeapons;

    // Invincibility frame data
    [HideInInspector]
    public float currentInvinciSeconds;
    [HideInInspector]
    public float currentInvinciTimeDelta;
    [HideInInspector]
    public bool isInvincible = false;

    public Renderer spriteBig;

    // Experience and Level
    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;   // how much experience to get before leveling up

    // Nested class for defining level range and corresponding experience cap
    // to increase for that range
    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }

    // Contains info for each level
    public List<LevelRange> levelRanges;

    InventoryManager inventory;
    public int weaponIndex;
    public int passiveItemIndex;

    //TESTING
    public GameObject firstPassiveItemTest, secondPassiveItemTest;


    void Awake()
    {
        // Assigns data based on selected character
        characterData = CharacterSelector.GetData();
        CharacterSelector.instance.DestroySingleton();

        inventory = GetComponent<InventoryManager>();

        // Initialize starting character stats
        currentHealth = characterData.MaxHealth;
        currentMaxHealth = characterData.MaxHealth;
        currentRecovery = characterData.Recovery;
        currentMoveSpeed = characterData.MoveSpeed;
        currentMight = characterData.Might;
        currentProjectileSpeed = characterData.ProjectileSpeed;
        currentInvinciSeconds = characterData.InvinciSeconds;
        currentInvinciTimeDelta = characterData.InvinciTimeDelta;
        currentMagnet = characterData.Magnet;

        // Spawn starting weapon
        SpawnWeapon(characterData.StartingWeapon);
        //TESTING
        SpawnPassiveItem(firstPassiveItemTest);
        SpawnPassiveItem(secondPassiveItemTest);
    }

    private void Start()
    {
        // Initialize experience cap
        experienceCap = levelRanges[0].experienceCapIncrease;
    }

    public void IncreaseExperience(int amount)
    {
        experience += amount;

        LevelUpChecker();
    }

    void LevelUpChecker()
    {
        // Checks if player has enough experience to level up
        if(experience >= experienceCap)
        {
            // Increases level and resets experience
            level++;
            experience -= experienceCap;

            // Determines how much to increase experience cap
            int experienceCapIncrease = 0;            
            foreach(LevelRange range in levelRanges)
            {
                // Checks which level range the player is in
                if(level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                    break;
                }
            }
            experienceCap += experienceCapIncrease;
        }
    }

    public void TakeDamage(float dmg)
    {
        if(currentHealth > 0)
        {
            currentHealth -= dmg;
        }
        if(currentHealth <= 0)
        {
            Kill();
        }
        // Activates invincibility frames
        StartCoroutine(BecomeTemporarilyInvincible());
    }

    public void Kill()
    {
        Debug.Log("PLAYER IS DEAD");
        return;
    }

    private IEnumerator BecomeTemporarilyInvincible()
    {
        //Debug.Log("Player turned invincible!");
        isInvincible = true;

        // Iterates through the seconds of being invincible
        for (float i = 0; i < currentInvinciSeconds; i += currentInvinciTimeDelta)
        {
            // Makes player sprite flash during i frames
            if (spriteBig.enabled)
            {
                spriteBig.enabled = false;
            }
            else
            {
                spriteBig.enabled = true;
            }
            yield return new WaitForSeconds(currentInvinciTimeDelta);
        }

        // Restores default sprite state and removes invincibility
        spriteBig.enabled = true;
        isInvincible = false;
        //Debug.Log("Player is no longer invincible!");
    }

    public void RestoreHealth(float amount)
    {
        if(currentHealth < currentMaxHealth)
        {
            currentHealth += amount;

            if(currentHealth > currentMaxHealth)
            {
                currentHealth = currentMaxHealth;
            }
        }
    }

    public void SpawnWeapon(GameObject weapon)
    {
        // Check if inventory is full
        if(weaponIndex >= inventory.weaponSlots.Count - 1)
        {
            Debug.LogError("Inventory slots already full");
            return;
        }

        // Spawn the starting weapon
        GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform); // Weapon is child of the player
        inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>());
        //spawnedWeapons.Add(spawnedWeapon); // Add to list of spawned weapons

        weaponIndex++;
    }

    public void SpawnPassiveItem(GameObject passiveItem)
    {
        // Check if inventory is full
        if(passiveItemIndex >= inventory.passiveItemSlots.Count - 1)
        {
            Debug.LogError("Inventory slots already full");
            return;
        }

        // Spawn the starting passive item
        GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform); // Passive item is child of the player
        inventory.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<PassiveItem>());
        //spawnedWeapons.Add(spawnedWeapon); // Add to list of spawned weapons

        passiveItemIndex++;
    }
}
