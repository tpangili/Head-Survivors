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
}
