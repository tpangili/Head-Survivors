using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class ComicController : HeadController
{
    [HideInInspector]
    public float theta = 0;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }
    private void FixedUpdate()
    {
        transform.localPosition = new Vector3(0.7f * Mathf.Cos(theta), 0.4f * Mathf.Sin(theta) + 0.4f, 0);
        theta += Time.fixedDeltaTime / 2;
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedGarlic = Instantiate(weaponData.Prefab);
        spawnedGarlic.transform.position = transform.position;  // Same position as object parented to player
        spawnedGarlic.transform.parent = transform;             // Spawns below object
    }
}
