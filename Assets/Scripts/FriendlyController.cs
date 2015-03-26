using UnityEngine;
using System.Collections;

/*
 * List of selectable Friendly types 
 */
public enum FriendlyType
{
    stationary, patrol
}

/*
 * List of posible states an Friendly can be in
 */
public enum FriendlyState
{
    idle, attack
}

public class FriendlyController : MonoBehaviour {
    public FriendlyType type;
    private FriendlyState _state = FriendlyState.idle;      // Local variable to represent our state
    
    public GameObject enemyPatrol;
    public GameObject enemyStationary;

    // Health
    public float currentHealth = 5f;
    public float coolDown = 2f;             // Length of damage cooldown in seconds
    private bool onCoolDown = false;        // Cooldown active or not

    // Target (usually the player)
    public string targetLayer = "Player";   // TODO: Make this a list, for players and friendly NPC's
    public GameObject target;
    
    // Spot
    public float spotRadius = 3;            // Radius in which a player can be spotted
    public bool drawSpotRadiusGismo = true; // Visual aid in determening if the spot radius
    private Collider2D[] collisionObjects;
    public bool playerSpotted = false;      // Debug purposes, to see in the editor if an Friendly spotted the player

    public bool playerIsLeft;               // Simple check to see if the player is left to the enemy, important for facing.
    private bool facingLeft = true;         // For determining which way the player is currently facing.

    // Patrol
    public float walkSpeed = 1f;            // Amount of velocity
    private bool walkingRight;              // Simple check to see in what direction the Friendly is moving, important for facing.
    public float collideDistance = 0.5f;    // Distance from Friendly to check for a wall.
    public bool edgeDetection = true;       // If checked, it will try to detect the edge of a platform
    private bool collidingWithWall = false; // If true, it touched a wall and should flip.
    private bool collidingWithGround = true;// If true, it is not about to fall off an edge

    void FixedUpdate()
    {
        switch (_state)
        {
            case FriendlyState.idle:
                Idle();
                break;
        }

        if (currentHealth <= 0)
        {
            FriendlyDeath();
        }
    }

    /*
     * Take damage when hit with an enemy projectile. When this entity gets hit
     * it will get a period in which it can not be hurt ('onCoolDown'), granting
     * it invincibility for a short period of time.
     */
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyProjectile"))
        {
            if (!onCoolDown && currentHealth > 0)
            {
                StartCoroutine(coolDownDMG());
                Debug.Log(this.gameObject.name + ": Au!");
                currentHealth -= 1;
            }
        }
    }

    /*
     * Sets the delay when this entity can get hurt again.
     */
    IEnumerator coolDownDMG()
    {
        onCoolDown = true;
        yield return new WaitForSeconds(coolDown);
        onCoolDown = false;
    }

    /*
     * Friendly death
     * 
     * When an friendly dies, it will be replaced with an enemy of the same type.
     */
    void FriendlyDeath()
    {
        Debug.Log(this.gameObject.name + ": 'Oh nee, ik ben slecht geworden!'");
        if (type == FriendlyType.patrol)
        {
            Instantiate(enemyPatrol, this.transform.position, this.transform.rotation);
        }
        else if (type == FriendlyType.stationary)
        {
            Instantiate(enemyStationary, this.transform.position, this.transform.rotation);
        }
        Destroy(this.gameObject);
    }

    /*
     * Idle state
     * 
     * In this state, the Friendly will wait to spot a player, and then it will go to its attack state.
     * Patroling Friendlys will resume to patrol after the player is out of reach
     */
    private void Idle()
    {
        // Sends the patroling Friendly to patrol
        if (type == FriendlyType.patrol)
        {
            Patrol();
        }

        // Will set 'playerSpotted' to true if spotted
        IsPlayerInRange();
        if (playerSpotted)
        {
            FacePlayer();
            if (type == FriendlyType.patrol)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            }
        }
    }

    /*
     * Patrol script for Friendly, 
     * will walk untill the collidingWithWall linecast hits a collider, then walk the other way
     * or (if checked) will detect if the Friendly is to hit the edge of a platform
     */
    private void Patrol()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(walkSpeed, GetComponent<Rigidbody2D>().velocity.y);

        FaceDirectionOfWalking();

        collidingWithWall = Physics2D.Linecast(
            new Vector2((this.transform.position.x + collideDistance), (this.transform.position.y - (GetComponent<SpriteRenderer>().bounds.size.y / 4))),
            new Vector2((this.transform.position.x + collideDistance), (this.transform.position.y + (GetComponent<SpriteRenderer>().bounds.size.y / 2))),
            ~(
                (1 << LayerMask.NameToLayer("EnemeyProjectile")) +
                (1 << LayerMask.NameToLayer("Projectile"))
            ) // Collide with all layers, except itself
        );

        if (edgeDetection)
        {
            collidingWithGround = Physics2D.Linecast(
                new Vector2(this.transform.position.x, this.transform.position.y),
                new Vector2((this.transform.position.x + collideDistance), (this.transform.position.y - (GetComponent<SpriteRenderer>().bounds.size.y))),
                ~(
                    (1 << this.gameObject.layer)
                ) // Collide with all layers, except itself
            );
        }
        else
        {
            collidingWithGround = true;
        }

        if (collidingWithWall || !collidingWithGround)
        {
            // Debug.Log(this.name + " hit a wall, now walking the other way.");
            walkSpeed *= -1;
            collideDistance *= -1;
        }
    }

    /*
     * This method makes sure the friendly will be facing the direction it is going in
     */
    private void FaceDirectionOfWalking()
    {
        if (GetComponent<Rigidbody2D>().velocity.x > 0)
        {
            walkingRight = true;
        }
        else
        {
            walkingRight = false;
        }
        if (walkingRight && facingLeft)
        {
            Flip();
        }
        else if (!walkingRight && !facingLeft)
        {
            Flip();
        }
    }

    /*
     * Checks to see if an entity of the "Player" layer has entered the range of the Friendly.
     * 
     * Gets a list colliders that collided with the overlapcircle and uses the first result to 
     * become the target of the Friendly. This is so that you don't have to manually add the target to every Friendly
     * and will help when multiplayer is implemented
     */
    private void IsPlayerInRange()
    {
        collisionObjects = Physics2D.OverlapCircleAll(this.transform.position, spotRadius, 1 << LayerMask.NameToLayer(targetLayer));

        if (collisionObjects.Length > 0)
        {
            target = collisionObjects[0].gameObject;
            playerSpotted = true;
        }
        else
        {
            playerSpotted = false;
        }
    }

    /*
     * Script to make the friendly face the player
     */
    private void FacePlayer()
    {
        //Player could be destroyed
        if (target != null)
        {
            playerIsLeft = target.transform.position.x < this.transform.position.x;

            if (!playerIsLeft && facingLeft)
            {
                Flip();
            }
            else if (playerIsLeft && !facingLeft)
            {
                Flip();
            }
        }
    }

    /*
     * Flips the sprite the other way around so it will face left/right.
     * 
     * Used by both FacePlayer() and FaceDirectionOfWalking().
     */
    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingLeft = !facingLeft;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    /*
     * Draws a circle gizmo to show the field of view or 'agro' range of an friendly
     */
    private void OnDrawGizmos()
    {
        if (drawSpotRadiusGismo)
        {
            Gizmos.color = Color.green;
            //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
            Gizmos.DrawWireSphere(this.transform.position, spotRadius);
        }

        // Draws the collision for the patrol friendlies
        if (type == FriendlyType.patrol)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(
                new Vector2((this.transform.position.x + collideDistance), (this.transform.position.y - (GetComponent<SpriteRenderer>().bounds.size.y / 4))),
                new Vector2((this.transform.position.x + collideDistance), (this.transform.position.y + (GetComponent<SpriteRenderer>().bounds.size.y / 2)))
                );

            if (edgeDetection)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(
                    new Vector2(this.transform.position.x, this.transform.position.y),
                    new Vector2((this.transform.position.x + collideDistance), (this.transform.position.y - (GetComponent<SpriteRenderer>().bounds.size.y)))
                    );
            }
        }
    }
}
