using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkTrigger : MonoBehaviour
{
    MapController mc;
    public GameObject targetMap;

    // Start is called before the first frame update
    void Start()
    {
        mc = FindObjectOfType<MapController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Checks if player enters chunk and sets as current chunk
        if(collision.CompareTag("Player"))
        {
            mc.currentChunk = targetMap;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Checks if player exits chunk and removes current chunk
        if(collision.CompareTag("Player"))
        {
            if(mc.currentChunk == targetMap)
            {
                mc.currentChunk = null;
            }
        }
    }
}
