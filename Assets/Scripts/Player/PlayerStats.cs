using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    CharacterScriptableObject characterData;

    // Current player stats
    float currentHealth;
    float currentMaxHealth;
    float currentMoveSpeed;
    float currentMight;
    float currentProjectileSpeed;
    float currentMagnet;

    #region Current Stats Properties
    public float CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            // Check if the value has changed
            if(currentHealth != value)
            {
                currentHealth = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text = "Health: " + currentHealth;
                }
                // add any additional logic here that needs to be executed when the value changes
            }
        }
    }

    public float CurrentMaxHealth
    {
        get { return currentMaxHealth; }
        set
        {
            // Check if the value has changed
            if(currentMaxHealth != value)
            {
                currentMaxHealth = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentMaxHealthDisplay.text = "Max Health: " + currentMaxHealth;
                }
                // add any additional logic here that needs to be executed when the value changes
            }
        }
    }

    public float CurrentMoveSpeed
    {
        get { return currentMoveSpeed; }
        set
        {
            // Check if the value has changed
            if(currentMoveSpeed != value)
            {
                currentMoveSpeed = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + currentMoveSpeed;
                }
                // add any additional logic here that needs to be executed when the value changes
            }
        }
    }

    public float CurrentMight
    {
        get { return currentMight; }
        set
        {
            // Check if the value has changed
            if(currentMight != value)
            {
                currentMight = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentMightDisplay.text = "Might: " + currentMight;
                }
                // add any additional logic here that needs to be executed when the value changes
            }
        }
    }

    public float CurrentProjectileSpeed
    {
        get { return currentProjectileSpeed; }
        set
        {
            // Check if the value has changed
            if(currentProjectileSpeed != value)
            {
                currentProjectileSpeed = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + currentProjectileSpeed;
                }
                // add any additional logic here that needs to be executed when the value changes
            }
        }
    }

    public float CurrentMagnet
    {
        get { return currentMagnet; }
        set
        {
            // Check if the value has changed
            if(currentMagnet != value)
            {
                currentMagnet = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentMagnetDisplay.text = "Magnet " + currentMagnet;
                }
                // add any additional logic here that needs to be executed when the value changes
            }
        }
    }
    #endregion

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
        CurrentHealth = characterData.MaxHealth;
        CurrentMaxHealth = characterData.MaxHealth;
        //CurrentRecovery = characterData.Recovery;
        CurrentMoveSpeed = characterData.MoveSpeed;
        CurrentMight = characterData.Might;
        CurrentProjectileSpeed = characterData.ProjectileSpeed;
        CurrentMagnet = characterData.Magnet;

        currentInvinciSeconds = characterData.InvinciSeconds;
        currentInvinciTimeDelta = characterData.InvinciTimeDelta; // reminder: add a property for these if we add i-frame upgrades

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

        // Set current stats for UI display
        GameManager.instance.currentHealthDisplay.text = "Health: " + currentHealth;
        GameManager.instance.currentMaxHealthDisplay.text = "Max Health: " + currentMaxHealth;
        GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + currentMoveSpeed;
        GameManager.instance.currentMightDisplay.text = "Might: " + currentMight;
        GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + currentProjectileSpeed;
        GameManager.instance.currentMagnetDisplay.text = "Magnet " + currentMagnet;
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
            CurrentHealth -= dmg;
        }
        if(CurrentHealth <= 0)
        {
            Kill();
        }
        // Activates invincibility frames
        StartCoroutine(BecomeTemporarilyInvincible());
    }

    public void Kill()
    {
        if (!GameManager.instance.isGameOver)
        {
            GameManager.instance.GameOver();
        }
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
        if(CurrentHealth < currentMaxHealth)
        {
            CurrentHealth += amount;

            if(CurrentHealth > CurrentMaxHealth)
            {
                CurrentHealth = CurrentMaxHealth;
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
