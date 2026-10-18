using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class EyeController : WeaponController
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
        GameObject spawnedRay = Instantiate(weaponData.Prefab, this.transform);
        spawnedRay.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * theta + 90);
        spawnedRay.GetComponentInChildren<EyeBehavior>().theta = this.theta;
    }
}
