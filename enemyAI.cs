using UnityEngine;
using UnityEngine.AI;
// Credit:  https://www.youtube.com/watch?v=UjkSFoLxesw (Enemy movement and chase functions, attacking and other minor behavior has been modified

public class enemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsPlayer, whatIsGround;

    // patroling
    public Vector3 walkPoint;
    bool walkPointSet;
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
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

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

        // check that point is on ground and not outside of map
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
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

       // DON'T NEED AN ATTACK FUNCTION BECAUSE ATTACK HAPPENS ON CONTACT //
    }

    // Take damage here
}
