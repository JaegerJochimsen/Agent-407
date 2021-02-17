using UnityEngine;
using UnityEngine.AI;
// Credit:  https://www.youtube.com/watch?v=UjkSFoLxesw (Enemy movement and chase functions, attacking and other minor behavior has been modified

public class enemyAI : MonoBehaviour
{
    /*
     * make damage tick off of proximity;
     * set bool to false when attacked, then reset when knocked back (out of attack range)
     * try not baking environ -- may also be the "is kinematic" option (may move through world though) -- can implement own physics (ray down for dist measuring)
     * 
     */
    public CharacterController controller;
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsPlayer, whatIsGround;

    public healthBar healthBar;

    public float enemyDamage = 20f;
    public float enemyHealth = 50f;
    public float knockBack = 10f;

    float knockUp = 2f;

    // patroling
    public Vector3 walkPoint;
    public bool walkPointSet = false;
    public float walkPointRange;
 
    // states
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    static public Vector3 dir;

    private void Update()
    {
        // check for player position
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // determine current state
        if (!playerInSightRange && !playerInAttackRange)
            Patroling();
        if (playerInSightRange && !playerInAttackRange)
            Chasing();
        if (playerInAttackRange)
            Attacking();
    }

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();

    }
    
    private void Patroling()
    {
        // determine where walking to, if not set then set it
        if (!walkPointSet)
            SearchWalkPoint();
        if (walkPointSet)
            agent.SetDestination(walkPoint);

        // calculate distance to destination
        Vector3 distanceToWalkPoint = walkPoint - transform.position;

        // walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // calc random point in range for patrolling
        float randz = Random.Range(-walkPointRange, walkPointRange);
        float randx = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randx, transform.position.y, transform.position.z + randz);

        // check that enemy destination is within the map range
        if (walkPoint.x <= -190f)
            walkPoint.x = -185f;
        else if (walkPoint.x >= 190f)
            walkPoint.x = 185f;

        if (walkPoint.z <= -175f)
            walkPoint.z = -170f;
        else if (walkPoint.z >= 210f)
            walkPoint.z = 205f;

        walkPointSet = true;
    }

    private void Chasing()
    {
        agent.SetDestination(player.position);

        // have enemy look at player
        transform.LookAt(player);

        dir = player.transform.position - agent.transform.position;
    }
    
    private void Attacking()
    {
        // stop movement when player is reached
        agent.SetDestination(transform.position);
        Vector3 direction = controller.transform.position - agent.transform.position;
        direction.Normalize();

        // if the enemy is within striking distance of player -> apply damage and knockback
        if ((((transform.position - controller.transform.position).z <= 1.5f && (transform.position - controller.transform.position).x <= 1.5f)) && !MovePlayer.inAirKnock){
            // set bool so that when we land we can stop moving back
            MovePlayer.inAirKnock = true;

            MovePlayer.velocity.x += knockBack * direction.x;
            MovePlayer.velocity.z += knockBack * direction.z;
            MovePlayer.velocity.y += Mathf.Sqrt(knockUp * (-2f) * MovePlayer.grav);
            MovePlayer.velocity.y += MovePlayer.grav * Time.deltaTime;

            // if we have health left then subtract from it, else we die (IMPLEMENT PLAYER DEATH FUNCT AND SCREEN)
            if (MovePlayer.health > enemyDamage)
            {
                MovePlayer.health -= enemyDamage;
            }
            else
            {
                MovePlayer.health = 0;
            }
        }
        // we have made changes to health so update health bar
        healthBar.SetHealth(MovePlayer.health);

        if(MovePlayer.inAirKnock)
            controller.Move(MovePlayer.velocity * Time.deltaTime);
    }
    // public so that we can call this function from another script
    public void TakeDamage(float amount)
    {
        enemyHealth -= amount;
        if (enemyHealth <= 0f)
        {
            Die();
        }
    }

    // local function, don't need other scripts to see it so not public
    void Die()
    {
        Destroy(gameObject);
    }
}
