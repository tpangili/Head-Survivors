using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float focusSpeed;
    public Vector2 movementInput;
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector]
    public Vector2 lastMovedVector;

    [Header("References")]
    Rigidbody2D body;
    //[SerializeField] private GameObject sprite;
    public Renderer sprite;     // Hitbox sprite
    public Renderer spriteBig;  // Actual player sprite surrounding hitbox

    [Header("Stats")]
    //public int collectCount;
    public int health;
    [SerializeField] private float invinciSeconds;
    [SerializeField] private float invinciTimeDelta;
    public bool isInvincible = false;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        sprite.enabled = false;
        lastMovedVector = new Vector2(1, 0f); // Projectiles shoot right by default
    }

    private void Update()
    {
        //Getting movement input
        movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

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
        // Focus movement: character moves slower while shift is held
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            body.MovePosition(body.position + movementInput * focusSpeed * Time.fixedDeltaTime);
            sprite.enabled = true;  // Hitbox is visible while focusing
        }
        else
        {
            // Normal movement
            body.MovePosition(body.position + movementInput * moveSpeed * Time.fixedDeltaTime);
            sprite.enabled = false; // Hitbox is invisible during normal movement
        }
    }

    public void LoseHealth()
    {
        // Take away health
        if (health > 0)
        {
            health--;
            Debug.Log("Health: " + health);
        }
        // Player dies if health reaches 0
        if (health <= 0)
        {
            //Destroy(gameObject);
            Debug.Log("Game Over");
            return;
        }
        // Activates invincibility frames
        StartCoroutine(BecomeTemporarilyInvincible());
    }

    private IEnumerator BecomeTemporarilyInvincible()
    {
        //Debug.Log("Player turned invincible!");
        isInvincible = true;

        // Iterates through the seconds of being invincible
        for (float i = 0; i < invinciSeconds; i += invinciTimeDelta)
        {
            // Makes player sprite flash during i frames
            if (spriteBig.enabled)
            {
                spriteBig.enabled = false;
            }
            else
            {
                spriteBig.enabled = true;
            }
            yield return new WaitForSeconds(invinciTimeDelta);
        }

        // Restores default sprite state and removes invincibility
        spriteBig.enabled = true;
        isInvincible = false;
        //Debug.Log("Player is no longer invincible!");
    }
}
