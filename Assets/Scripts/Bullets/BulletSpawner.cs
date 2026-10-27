using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class BulletSpawner : MonoBehaviour
{
    public enum SpawnerType { Straight, Spin, Chase }
    public enum SpinDirection { Left, Right }
    public enum BulletArrange { Single, Row, Line }

    [Header("Bullet Attributes")]
    public GameObject bullet;
    public GameObject bullet2;
    PlayerController player;
    public float bulletLife = 1f;
    public float speed = 1f;
    public float bulletSpace = 1f; // Spacing between bullets if in a row or line
    public int bulletNum = 1; // Number of bullets in a row or line
    public bool spreadEnable = true;  // Enables bullets in row to spread apart (DOES NOT WORK)

    [Header("Spawner Attributes")]
    public SpawnerType spawnerType;
    public SpinDirection spinDirection;
    public BulletArrange bulletArrange;
    public float firingRate = 1f;
    public float switchRate = 1f; // The rate at which spawner switches between bullet types
    public float spinSpeed = 1f;

    private GameObject spawnedBullet;

    private float timer = 0f;
    private float bulletCount = 0f;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        // Reverses usual direction if spawner is spinning to the right
        if(spawnerType == SpawnerType.Spin && spinDirection == SpinDirection.Right)
        {
            spinSpeed *= -1;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Used to meter out bullets depending on firing rate
        timer += Time.deltaTime;
        // Checks which spawner type is being used
        switch(spawnerType)
        {
            case SpawnerType.Spin:
                // Spins spawner around z axis in desired direction
                transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z + spinSpeed);
                break;
            case SpawnerType.Chase:
                // Calculates the angle of player's position and rotates spawner accordingly
                Vector3 direction = player.transform.position - transform.position;
                float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
                break;
            default:
                break;
        }

        // Keeps track of how often to fire
        if(timer >= firingRate)
        {
            //bulletCount++;
            Fire();
            timer = 0;
        }
    }

    private void Fire()
    {
        // Checks if bullet gameobject exists
        if (!bullet)
        {
            return;
        }

        // Variable to control which side of initial bullet additional bullet is spawned
        float switchSide = 1f;
        Vector3 offset = Vector3.zero;
        // Loops until all needed bullets are spawned
        for (int i = 0; i < bulletNum; i++)
        {
            bulletCount++;
            // Checks which arrangement to spawn bullets in
            switch(bulletArrange)
            {
                case BulletArrange.Single:
                    i = bulletNum;
                    break;
                case BulletArrange.Row:
                    // Sets offset for bullets left and right of center bullet
                    if (i != 0)
                    {
                        offset = transform.up * bulletSpace * switchSide;
                    }
                    break;
                case BulletArrange.Line:
                    // Sets offset for bullets above and below center bullet
                    if (i != 0)
                    {
                        offset = transform.right * bulletSpace * switchSide;
                    }
                    break;
                default:
                    break;
            }

            // Checks if conditions are right to spawn secondary bullet type
            if(bulletCount % switchRate == 0 && bullet2)
            {
                spawnedBullet = Instantiate(bullet2, transform.position + offset, Quaternion.identity);
            }
            else
            {
                spawnedBullet = Instantiate(bullet, transform.position + offset, Quaternion.identity);
            }
            // Increases scale and switches side of offset
            if (i != 0 && i % 2 == 0)
            {
                switchSide += bulletSpace;
            }
            switchSide *= -1;
            // Initalizes all bullet attributes before firing
            spawnedBullet.GetComponent<Bullet>().speed = speed;
            spawnedBullet.GetComponent<Bullet>().lifetime = bulletLife;
            spawnedBullet.transform.rotation = transform.rotation;
            // Makes bullets in row spread out
            if (spreadEnable && i != 0 && bulletArrange == BulletArrange.Row)
            {
                // pass them to bullet
                spawnedBullet.GetComponent<Bullet>().spreadEnable = spreadEnable;
                spawnedBullet.GetComponent<Bullet>().bulletSide = switchSide;
            }
        }
    }
}
