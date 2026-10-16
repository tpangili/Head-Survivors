using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public CharacterScriptableObject characterData;

    // Current player stats
    float currentHealth;
    float currentRecovery;
    float currentMoveSpeed;
    float currentMight;
    float currentProjectileSpeed;

    // Invincibility frame data
    [SerializeField]
    private float invinciSeconds;
    [SerializeField]
    private float invinciTimeDelta;
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

    void Awake()
    {
        // Initialize starting character stats
        currentHealth = characterData.MaxHealth;
        currentRecovery = characterData.Recovery;
        currentMoveSpeed = characterData.MoveSpeed;
        currentMight = characterData.Might;
        currentProjectileSpeed = characterData.ProjectileSpeed;
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
        for (float i = 0; i < invinciSeconds; i += invinciTimeDelta)
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
            yield return new WaitForSeconds(invinciTimeDelta);
        }

        // Restores default sprite state and removes invincibility
        spriteBig.enabled = true;
        isInvincible = false;
        //Debug.Log("Player is no longer invincible!");
    }
}
