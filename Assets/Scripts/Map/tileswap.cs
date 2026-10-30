using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class tileswap : MonoBehaviour
{
    public Tilemap map;
    public Tile t1_1, t1_2;
    public Tile t2_1, t2_2;
    public Tile t3_1, t3_2;
    public int WaveToStartLevel2;
    public int WaveToStartLevel3;
    public EnemySpawner WaveCounterSource;

    public bool l2 = false;
    public bool l3 = false;

    void Start()
    {
        if (WaveCounterSource.currentWaveCount >= WaveToStartLevel2)
        {
            L2Transition();
        }
        else if (WaveCounterSource.currentWaveCount >= WaveToStartLevel3)
        {
            L3Transition();
        }
    }

    void Update()
    {
        if (WaveCounterSource.currentWaveCount >= WaveToStartLevel2 && !l2)
        {
            L2Transition();
        }
        else if (WaveCounterSource.currentWaveCount >= WaveToStartLevel3 && !l3)
        {
            L3Transition();
        }
    }

    public void L2Transition()
    {
        map.SwapTile(t1_1, t2_1);
        map.SwapTile(t1_2, t2_2);
        l2 = true;
    }
    public void L3Transition()
    {
        map.SwapTile(t2_1, t3_1);
        map.SwapTile(t2_2, t3_2);
        l3 = true;
    }
}
