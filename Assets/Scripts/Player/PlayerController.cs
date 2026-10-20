using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    //private float currentMoveSpeed;
    //public float focusSpeed;  // Half the current speed
    public Vector2 movementInput;
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector]
    public Vector2 lastMovedVector;

    [Header("References")]
    Rigidbody2D body;
    PlayerStats player;
    //[SerializeField] private GameObject sprite;
    public Renderer sprite;     // Hitbox sprite
    public Renderer spriteBig;  // Actual player sprite surrounding hitbox

    /*[Header("Stats")]
    //public int collectCount;
    public int health;
    [SerializeField] private float invinciSeconds;
    [SerializeField] private float invinciTimeDelta;
    public bool isInvincible = false;*/

    private void Awake()
    {
        player = GetComponent<PlayerStats>();
        body = GetComponent<Rigidbody2D>();
        //currentMoveSpeed = player.currentMoveSpeed;
    }

    private void Start()
    {
        sprite.enabled = false;
        lastMovedVector = new Vector2(1, 0f); // Projectiles shoot right by default
    }

    private void Update()
    {
        // Prevents movement if game is over
        if (GameManager.instance.isGameOver)
        {
            return;
        }

        //Getting movement input
        movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        // Tracks last moved x and y directions
        // (Used for any weapon aiming based on direction of player movement)
        if(movementInput.x != 0)
        {
            lastHorizontalVector = movementInput.x;
            lastMovedVector = new Vector2(lastHorizontalVector, 0f);
        }
        if(movementInput.y != 0)
        {
            lastVerticalVector = movementInput.y;
            lastMovedVector = new Vector2(0f, lastVerticalVector);
        }

        // Tracks last moved direction while moving
        if(movementInput.x != 0 && movementInput.y != 0)
        {
            lastMovedVector = new Vector2(lastHorizontalVector, lastVerticalVector);
        }
    }

    private void FixedUpdate()
    {
        // Prevents movement if game is over
        if (GameManager.instance.isGameOver)
        {
            return;
        }
        
        // Focus movement: character moves slower while shift is held
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            body.MovePosition(body.position + movementInput * (player.CurrentMoveSpeed/2) * Time.fixedDeltaTime);
            sprite.enabled = true;  // Hitbox is visible while focusing
        }
        else
        {
            // Normal movement
            body.MovePosition(body.position + movementInput * player.CurrentMoveSpeed * Time.fixedDeltaTime);
            sprite.enabled = false; // Hitbox is invisible during normal movement
        }
    }
}
