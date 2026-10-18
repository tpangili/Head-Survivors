using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBehavior : MeleeWeaponBehavior
{
    [HideInInspector]
    public float theta;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    private void FixedUpdate()
    {
        //transform.localPosition = new Vector3(0.7f * Mathf.Cos(theta), 0.4f * Mathf.Sin(theta) + 0.4f, 0);
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * theta + 90);
        //transform.Rotate(0, 0, Time.fixedDeltaTime / 2);
        theta += Time.fixedDeltaTime / 2;
    }
}
