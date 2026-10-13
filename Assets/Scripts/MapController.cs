using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;  // Stores prefabs of different terrain chunks
    public GameObject player;       // Reference position of player
    public float checkerRadius;
    private Vector3 noTerrainPosition;      // Next position without a chunk
    public LayerMask terrainMask;   // Track which layer is and isn't terrain
    public GameObject currentChunk; // Reference position of last spawned chunk
    PlayerController pm;

    [Header("Optimization")]
    public List<GameObject> spawnedChunks;
    private GameObject latestChunk;
    public float maxOpDist; // Must be greater than the length and width of the tilemap
    private float opDist; // Current distance from chunk to player
    private float opCooldown;
    public float opCooldownDur;

    // Start is called before the first frame update
    void Start()
    {
        pm = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        ChunkChecker();
        ChunkOptimizer();
    }

    // Checks if a chunk needs to be spawned
    void ChunkChecker()
    {
        // Exits if no chunk can be spawned
        if (!currentChunk)
        {
            return;
        }
        // Checks player direction
        if(pm.movementInput.x > 0 && pm.movementInput.y == 0)   // right
        {
            if(!Physics2D.OverlapCircle(currentChunk.transform.Find("Right").position, checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Right").position;
                SpawnChunk();
            }
        }
        else if(pm.movementInput.x < 0 && pm.movementInput.y == 0)  // left
        {
            if(!Physics2D.OverlapCircle(currentChunk.transform.Find("Left").position, checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Left").position;
                SpawnChunk();
            }
        }
        else if(pm.movementInput.x == 0 && pm.movementInput.y > 0)  // up
        {
            if(!Physics2D.OverlapCircle(currentChunk.transform.Find("Up").position, checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Up").position;
                SpawnChunk();
            }
        }
        else if(pm.movementInput.x == 0 && pm.movementInput.y < 0)  // down
        {
            if(!Physics2D.OverlapCircle(currentChunk.transform.Find("Down").position, checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Down").position;
                SpawnChunk();
            }
        }
        else if(pm.movementInput.x > 0 && pm.movementInput.y > 0)  // right up
        {
            if(!Physics2D.OverlapCircle(currentChunk.transform.Find("Right Up").position, checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Right Up").position;
                SpawnChunk();
            }
        }
        else if(pm.movementInput.x > 0 && pm.movementInput.y < 0)  // right down
        {
            if(!Physics2D.OverlapCircle(currentChunk.transform.Find("Right Down").position, checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Right Down").position;
                SpawnChunk();
            }
        }
        else if(pm.movementInput.x < 0 && pm.movementInput.y > 0)  // left up
        {
            if(!Physics2D.OverlapCircle(currentChunk.transform.Find("Left Up").position, checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Left Up").position;
                SpawnChunk();
            }
        }
        else if(pm.movementInput.x < 0 && pm.movementInput.y < 0)  // left down
        {
            if(!Physics2D.OverlapCircle(currentChunk.transform.Find("Left Down").position, checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Left Down").position;
                SpawnChunk();
            }
        }
    }

    // Spawns new terrain chunk
    void SpawnChunk()
    {
        int rand = Random.Range(0, terrainChunks.Count);
        latestChunk = Instantiate(terrainChunks[rand], noTerrainPosition, Quaternion.identity);
        spawnedChunks.Add(latestChunk);
    }

    // Disables and re-enables chunks based on distance from player
    void ChunkOptimizer()
    {
        opCooldown -= Time.deltaTime;
        if(opCooldown <= 0f)
        {
            opCooldown = opCooldownDur;
        }
        else
        {
            return;
        }

        // Goes through spawned chunks and checks if needs to be disabled/enabled
        foreach(GameObject chunk in spawnedChunks)
        {
            opDist = Vector3.Distance(pm.transform.position, chunk.transform.position);
            if(opDist > maxOpDist)
            {
                chunk.SetActive(false);
            }
            else
            {
                chunk.SetActive(true);
            }
        }
    }
}
