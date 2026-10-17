using UnityEngine;

public interface ICollectible
{
    void Collect();
    void OnTriggerEnter2D(Collider2D col);
}
