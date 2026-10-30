using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TragicController : HeadController
{
    public int tearCount;
    [HideInInspector]
    public float theta = 0;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    private void FixedUpdate()
    {
        transform.localPosition = new Vector3(0.7f * Mathf.Cos(theta), 0.4f * Mathf.Sin(theta) + 0.4f, 0);
        theta += Time.fixedDeltaTime / 2;
    }

    protected override void Attack()
    {
        base.Attack();
        for (int i = 0; i < tearCount; i++)
        {
            GameObject tear = Instantiate(weaponData.Prefab, this.transform.position, Quaternion.Euler(0, 0, Mathf.Rad2Deg * theta + 90 + (i * 360 / tearCount)));
        }
    }
}
