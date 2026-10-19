using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="WeaponScriptableObject", menuName ="ScriptableObjects/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    [SerializeField]
    GameObject prefab;
    public GameObject Prefab { get => prefab; private set => prefab = value; }

    // Base stats for weapons
    [SerializeField]
    float damage;
    public float Damage { get => damage; private set => damage = value; }

    [SerializeField]
    float speed;
    public float Speed { get => speed; private set => speed = value; }

    [SerializeField]
    float cooldownDuration;
    public float CooldownDuration { get => cooldownDuration; private set => cooldownDuration = value; }

    [SerializeField]
    int pierce;      // max times weapon can hit enemy before destroyed
    public int Pierce { get => pierce; private set => pierce = value; }

    [SerializeField]
    Sprite head;
    public Sprite Head { get => head; private set => head = value; }

    [SerializeField]
    Sprite headAction;
    public Sprite HeadAction { get => headAction; private set => headAction = value; }

    // NOTE: Leveling up is handled by replacing the current prefab with a leveled up prefab

    [SerializeField]
    int level;  // not meant to be modified in the game [only in editor]
    public int Level { get => level; private set => level = value; }

    [SerializeField]
    GameObject nextLevelPrefab; // prefab for what object becomes when leveled up, not prefab to be spawned at next level
    public GameObject NextLevelPrefab { get => nextLevelPrefab; private set => nextLevelPrefab = value; }

    [SerializeField]
    Sprite icon;    // not meant to be modified in game [only in editor]
    public Sprite Icon { get => icon; private set => icon = value; }
}
