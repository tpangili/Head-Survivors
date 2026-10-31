using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    // Weapon Inventory
    public List<WeaponController> weaponSlots = new List<WeaponController>(6);
    public List<int> weaponLevels = new List<int>(6);
    public List<Image> weaponUISlots = new List<Image>(6);
    public List<Text> HeadTimerDisplays = new List<Text>(6); // UI displays for head timers

    // Passive Item Inventory
    public List<PassiveItem> passiveItemSlots = new List<PassiveItem>(6);
    public int[] passiveItemLevels = new int[6];
    public List<Image> passiveItemUISlots = new List<Image>(6);

    [System.Serializable]
    public class WeaponUpgrade
    {
        public int weaponUpgradeIndex;
        public GameObject initialWeapon;
        public WeaponScriptableObject weaponData;
    }

    [System.Serializable]
    public class PassiveItemUpgrade
    {
        public int passiveItemUpgradeIndex;
        public GameObject initialPassiveItem;
        public PassiveItemScriptableObject passiveItemData;
    }

    [System.Serializable]
    public class UpgradeUI
    {
        public Text upgradeNameDisplay;
        public Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    public List<WeaponUpgrade> weaponUpgradeOptions = new List<WeaponUpgrade>(); // list of upgrade options for weapons on level up screen
    public List<PassiveItemUpgrade> passiveItemUpgradeOptions = new List<PassiveItemUpgrade>(); // list of upgrade options for passive items on level up screen
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();    // list of ui for upgrade options present in the scene

    PlayerStats player;

    void Start()
    {
        player = GetComponent<PlayerStats>();
    }

    // Adds weapon to specific inventory slot
    public void AddWeapon(int slotIndex, WeaponController weapon)
    {
        weaponSlots[slotIndex] = weapon;
        weaponSlots[slotIndex].weaponIndex = slotIndex;     // used for head timer ui
        weaponLevels[slotIndex] = weapon.weaponData.Level;
        weaponUISlots[slotIndex].enabled = true; // enable image component
        weaponUISlots[slotIndex].sprite = weapon.weaponData.Icon;

        // Enables head timer display if weapon is a head
        if(slotIndex >= 1)
        {
            HeadTimerDisplays[slotIndex].enabled = true;
        }

        if(GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void UpdateHeadTimerDisplay(float currentHeadTimer, int slotIndex)
    {
        // Rounds down head timer
        int seconds = Mathf.FloorToInt(currentHeadTimer);
        // Makes sure seconds display is never negative
        if(seconds <= 0)
        {
            seconds = 0;
        }
        // Displays how much is left on head timer
        HeadTimerDisplays[slotIndex].text = seconds.ToString();
    }

    // Used when head timer restore item is picked up by player
    public void RestoreHeadTimers(int headToRestore)
    {
        bool noHead = true;
        // Iterates through weapons starting from 1
        // to skip first non-head default weapon
        for(int i = 1; i < weaponSlots.Count; i++)
        {
            if(weaponSlots[i] != null)
            {
                noHead = false;
                HeadController head = weaponSlots[i].GetComponent<HeadController>();
                head.currentHeadTimer += headToRestore;
                GameManager.GenerateFloatingText("TIMER UP!!!", head.transform);
            }
        }

        // Returns if player has no heads
        if(noHead)
        {
            GameManager.GenerateFloatingText("NO HEAD?", player.transform);
            Debug.Log("NO HEAD");
            return;
        }
    }

    // Used when timer bullets deplete head timers
    public void DepleteHeadTimers(int headToDeplete)
    {
        // Iterates through weapons starting from 1
        // to skip first non-head default weapon
        for(int i = 1; i < weaponSlots.Count; i++)
        {
            if(weaponSlots[i] != null)
            {
                HeadController head = weaponSlots[i].GetComponent<HeadController>();
                head.currentHeadTimer -= headToDeplete;
                GameManager.GenerateFloatingText("TIMER DOWN!!!", head.transform);
                if(head.currentHeadTimer <= 0)
                {
                    head.Kill();
                }
            }
        }
    }

    // Adds passive item to specific inventory slot
    public void AddPassiveItem(int slotIndex, PassiveItem passiveItem)
    {
        passiveItemSlots[slotIndex] = passiveItem;
        passiveItemLevels[slotIndex] = passiveItem.passiveItemData.Level;
        passiveItemUISlots[slotIndex].enabled = true; // enable image component
        passiveItemUISlots[slotIndex].sprite = passiveItem.passiveItemData.Icon;

        if(GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public int CheckInventory(WeaponController head)
    {
        int slotIndex = 99;
        for(int i = 0; i < weaponSlots.Count; i++)
        {
            if(weaponSlots[i] != null && weaponSlots[i].weaponData.InitialHead == head.weaponData.InitialHead)
            {
                slotIndex = i;
                break;
            }
        }
        return slotIndex;
    }

    public void RemoveWeapon(WeaponController head)
    {
        for(int i = 0; i < weaponSlots.Count; i++)
        {
            if(weaponSlots[i] != null && weaponSlots[i].weaponData.InitialHead == head.weaponData.InitialHead)
            {
                weaponSlots.RemoveAt(i);
                weaponSlots.Add(null);
                weaponLevels.RemoveAt(i);
                weaponLevels.Add(0);
                weaponUISlots[i].enabled = false;
                HeadTimerDisplays[i].enabled = false;
                // Adjusts the UI displays for inventory and head timers
                if((i + 1) < weaponUISlots.Count && weaponUISlots[i + 1].enabled == true)
                {
                    //HeadController keptHead;
                    for(int j = i; j < weaponUISlots.Count; j++)
                    {
                        if((j + 1) < weaponUISlots.Count && weaponUISlots[j + 1].enabled == true)
                        {
                            // Brings the next active head one UI slot back
                            weaponUISlots[j].GetComponent<Image>().sprite = weaponUISlots[j + 1].GetComponent<Image>().sprite;
                            HeadTimerDisplays[j].text = HeadTimerDisplays[j + 1].text;
                            // Displays that moved UI slot
                            weaponUISlots[j].enabled = true;
                            HeadTimerDisplays[j].enabled = true;
                            // Disables the previously occupied UI slot
                            weaponUISlots[j + 1].enabled = false;
                            HeadTimerDisplays[j + 1].enabled = false;
                            // Adjusts weaponIndex back one for head controller
                            if(weaponSlots[j] != null)
                            {
                                weaponSlots[j].weaponIndex = j;
                            }
                        }
                    }
                }
                //weaponUISlots[i].sprite = null;
                player.weaponIndex--;
                Destroy(head.gameObject);
                break;
            }
        }
    }

    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        if(weaponSlots.Count > slotIndex)
        {
            WeaponController weapon = weaponSlots[slotIndex];
            HeadController head;
            float lastHeadTime = 0;

            // Checks if current weapon has a next level
            if (!weapon.weaponData.NextLevelPrefab)
            {
                // Adds time to head timer if head level is maxed out
                if(upgradeIndex == 99)
                {
                    head = weapon.GetComponent<HeadController>();
                    head.currentHeadTimer += 20;
                    GameManager.GenerateFloatingText("MAX LEVEL!!!\nTIMER UP!!!", head.transform);
                    return;
                }
                // NOTE: Should be changed later to allow for infinite upgrading
                Debug.LogError("NO NEXT LEVEL FOR " + weapon.name);
                return;
            }
            // Initializes variables if weapon is a head
            if(upgradeIndex == 99)
            {
                head = weapon.GetComponent<HeadController>();
                lastHeadTime = head.currentHeadTimer;
            }
            
            GameObject upgradedWeapon = Instantiate(weapon.weaponData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradedWeapon.transform.SetParent(transform);  // Set weapon to be a child of the player
            AddWeapon(slotIndex, upgradedWeapon.GetComponent<WeaponController>());
            Destroy(weapon.gameObject);
            weaponLevels[slotIndex] = upgradedWeapon.GetComponent<WeaponController>().weaponData.Level; // Set level of weapon

            // Checks if current head's timer is greater than the leveled up head's default timer
            if(upgradeIndex == 99)
            {
                HeadController levelUpHead = weaponSlots[slotIndex].GetComponent<HeadController>();
                if(lastHeadTime >= levelUpHead.currentHeadTimer)
                {
                        levelUpHead.currentHeadTimer = lastHeadTime + 10;
                }
            }
            

            // Checks if a head is being upgraded
            if(upgradeIndex != 99)
            {
                weaponUpgradeOptions[upgradeIndex].weaponData = upgradedWeapon.GetComponent<WeaponController>().weaponData;
            }

            if(GameManager.instance != null && GameManager.instance.choosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }

    public void LevelUpPassiveItem(int slotIndex, int upgradeIndex)
    {
        if(passiveItemSlots.Count > slotIndex)
        {
            PassiveItem passiveItem = passiveItemSlots[slotIndex];
            // Checks if current passive item has a next level
            if (!passiveItem.passiveItemData.NextLevelPrefab)
            {
                // NOTE: Should be changed later to allow for infinite upgrading
                Debug.LogError("NO NEXT LEVEL FOR " + passiveItem.name);
                return;
            }
            GameObject upgradedPassiveItem = Instantiate(passiveItem.passiveItemData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradedPassiveItem.transform.SetParent(transform);  // Set passive item to be a child of the player
            AddPassiveItem(slotIndex, upgradedPassiveItem.GetComponent<PassiveItem>());
            Destroy(passiveItem.gameObject);
            passiveItemLevels[slotIndex] = upgradedPassiveItem.GetComponent<PassiveItem>().passiveItemData.Level; // Set level of passive item

            passiveItemUpgradeOptions[upgradeIndex].passiveItemData = upgradedPassiveItem.GetComponent<PassiveItem>().passiveItemData;

            if(GameManager.instance != null && GameManager.instance.choosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }

    void ApplyUpgradeOptions()
    {
        List<WeaponUpgrade> availableWeaponUpgrades = new List<WeaponUpgrade>(weaponUpgradeOptions);
        List<PassiveItemUpgrade> availablePassiveItemUpgrades = new List<PassiveItemUpgrade>(passiveItemUpgradeOptions);

        foreach(var upgradeOption in upgradeUIOptions)
        {
            
            // Exits if there are no additional upgrades to add to upgrade options
            if(availableWeaponUpgrades.Count == 0 && availablePassiveItemUpgrades.Count == 0)
            {
                return;
            }

            int upgradeType;
            if(availableWeaponUpgrades.Count == 0)
            {
                upgradeType = 2;
            }
            else if (availablePassiveItemUpgrades.Count == 0)
            {
                upgradeType = 1;
            }
            else
            {
                upgradeType = Random.Range(1, 3); // choose between weapon and passive items
            }

            // If weapon is chosen as upgrade option
            // NOTE: Will only be used to upgrade starting weapon
            if(upgradeType == 1)
            {
                WeaponUpgrade chosenWeaponUpgrade = availableWeaponUpgrades[Random.Range(0, availableWeaponUpgrades.Count)];

                // Removes version of weapon so it doesn't appear as a duplicate
                availableWeaponUpgrades.Remove(chosenWeaponUpgrade);
                
                // Safeguard in case chosen weapon upgrade is null
                if(chosenWeaponUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);
                    bool newWeapon = false;
                    // Searches inventory for a weapon slot that matches the weapon upgrade
                    // Checks if it is a new weapon or existing weapon
                    for(int i = 0; i < weaponSlots.Count; i++)
                    {
                        // Checks if it is an existing weapon
                        if(weaponSlots[i] != null && weaponSlots[i].weaponData == chosenWeaponUpgrade.weaponData)
                        {
                            newWeapon = false;
                            if (!newWeapon)
                            {
                                if (!chosenWeaponUpgrade.weaponData.NextLevelPrefab)
                                {
                                    DisableUpgradeUI(upgradeOption);
                                    break;
                                }

                                // Apply button functionality
                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, chosenWeaponUpgrade.weaponUpgradeIndex));   
                                // Set description and name to that of the weapon's next level
                                upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Description;
                                upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Name;
                            }
                            break;
                        }
                        else
                        {
                            newWeapon = true;
                        }
                    }
                    if (newWeapon)
                    {
                        // Apply button functionality
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnWeapon(chosenWeaponUpgrade.initialWeapon));
                        // Apply initial description and name of weapon
                        upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.weaponData.Description;
                        upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.weaponData.Name;
                    }

                    upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.weaponData.Icon;
                }
            }
            else if(upgradeType == 2)
            {
                PassiveItemUpgrade chosenPassiveItemUpgrade = passiveItemUpgradeOptions[Random.Range(0, availablePassiveItemUpgrades.Count)];

                // Removes version of passive item so it doesn't appear again as a duplicate
                availablePassiveItemUpgrades.Remove(chosenPassiveItemUpgrade);
                
                if(chosenPassiveItemUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);

                    bool newPassiveitem = false;
                    for(int i = 0; i < passiveItemSlots.Count; i++)
                    {
                        if(passiveItemSlots[i] != null && passiveItemSlots[i].passiveItemData == chosenPassiveItemUpgrade.passiveItemData)
                        {
                            newPassiveitem = false;
                            if (!newPassiveitem)
                            {
                                // Safeguard in case there's no further upgrade levels
                                // NOTE: adjust later so that items can be infinitely upgraded
                                if (!chosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab)
                                {
                                    DisableUpgradeUI(upgradeOption);
                                    break;
                                }

                                // Apply button functionality
                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, chosenPassiveItemUpgrade.passiveItemUpgradeIndex));
                                // Set description and name to that of the passive item's next level
                                upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.Description;
                                upgradeOption.upgradeNameDisplay.text = chosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.Name;
                            }
                            break;
                        }
                        else
                        {
                            newPassiveitem = true;
                        }
                    }
                    if (newPassiveitem)
                    {
                        // Apply button functionality
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnPassiveItem(chosenPassiveItemUpgrade.initialPassiveItem));
                        // Apply initial description and name of weapon
                        upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveItemUpgrade.passiveItemData.Description;
                        upgradeOption.upgradeNameDisplay.text = chosenPassiveItemUpgrade.passiveItemData.Name;
                    }

                    upgradeOption.upgradeIcon.sprite = chosenPassiveItemUpgrade.passiveItemData.Icon;
                }
            }
        }
    }

    void RemoveUpgradeOptions()
    {
        foreach(var upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption); // disable all ui options before applying upgrades
        }
    }

    public void RemoveAndApplyUpgrades()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }

    void DisableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }

    void EnableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }
}
