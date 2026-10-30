using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TragicHomingBehavior : MonoBehaviour
{
    private TragicBehavior MainBehavior;
    // Start is called before the first frame update
    void Start()
    {
        MainBehavior = GetComponentInParent<TragicBehavior>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" && MainBehavior.target == null)
        {
            MainBehavior.target = collision.gameObject;
        }
    }
}
