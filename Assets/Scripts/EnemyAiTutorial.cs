using UnityEngine;
using UnityEngine.AI;

public class EnemyAiTutorial : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private float sightRange;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Patrol")]
    [SerializeField] private float walkPointRange;

    [SerializeField] private Transform seePoint;

    [Header("Attacking")]
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private GameObject projectile;
    
    private bool _alreadyAttacked;
    private bool _walkPointSet;
    private bool _playerInSightRange;
    private bool _playerInAttackRange;
    private Vector3 _walkPoint;
    private Transform _player;
    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //Dont do anything if no players
        if (Player.Instances == null || Player.Instances.Count == 0)
            return;
        
        //Pick on the unlucky first player
        _player = Player.GetClosest(transform.position).transform;

        //Check for sight and attack range
        float distanceToPlayer = Vector3.Distance(_player.transform.position, seePoint.position);
        _playerInSightRange = distanceToPlayer < sightRange;
        _playerInAttackRange = distanceToPlayer < attackRange;
        
        //Assuming sight range is larger than attack range
        if (_playerInSightRange)
        {
            Debug.Log("IM COMING FOR YOU CHILD");
            if (_playerInAttackRange)
                AttackPlayer();
            else
                ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (!_walkPointSet)
        {
            SearchWalkPoint();
            return;
        }
        
        if (_walkPointSet)
            _agent.SetDestination(_walkPoint);

        //Check if close to walk point
        float distanceToWalkPoint = Vector3.Distance(transform.position, _walkPoint);
        if (distanceToWalkPoint < 1f)
            _walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random direction to walk in range
        float angle = Random.Range(0, 2 * Mathf.PI); //Random angle
        float distance = Random.Range(0, walkPointRange); //Random distance
        Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)); //Direction from angle
        _walkPoint = transform.position + direction * distance; //Bada bing bada boom, walkPoint
        
        //Check that walk point is actually walkable
        Vector3 raycastPoint = _walkPoint + Vector3.up * 5f;
        if (Physics.Raycast(raycastPoint, -transform.up, 10f, whatIsGround))
            _walkPointSet = true;
    }

    private void ChasePlayer()
    {
        _agent.SetDestination(_player.position);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        _agent.SetDestination(transform.position);
        
        //I can see you...
        transform.LookAt(_player);

        if (!_alreadyAttacked)
        {
            //Attack code here
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);
            //End of attack code

            _alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        _alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
