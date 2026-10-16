using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRateManager : MonoBehaviour
{
    // Nested class to hold info for each item drop
    [System.Serializable]
    public class Drops
    {
        public string name;
        public GameObject itemPrefab;
        public float dropRate;
    }

    // List of possible item drops
    public List<Drops> drops;

    // Drops item upon destroying enemy/misc.
    void OnDestroy()
    {
        float randomNumber = UnityEngine.Random.Range(0f, 100f);
        List<Drops> possibleDrops = new List<Drops>();

        foreach(Drops rate in drops)
        {
            // Checks rarity of each possible drop compared to the generated number
            // Ex: Generate 30, get item with 40% drop rates but not 25%
            if(randomNumber <= rate.dropRate)
            {
                possibleDrops.Add(rate);
            }
        }
        // Check if there are possible drops
        if(possibleDrops.Count > 0)
        {
            // Chooses a random item among the possible drops to spawn
            Drops drops = possibleDrops[UnityEngine.Random.Range(0, possibleDrops.Count)];
            Instantiate(drops.itemPrefab, transform.position, Quaternion.identity);
        }
    }
}
