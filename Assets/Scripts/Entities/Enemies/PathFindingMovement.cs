using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PathFindingMovement : SetTargettable
{
    [Header("Settings")] 
    [SerializeField] private float sightRange;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Patrol")]
    [SerializeField] private float walkPointRange;
    [SerializeField] private Transform seePoint;
    
    private bool _walkPointSet;
    private bool _playerInSightRange;
    private bool _playerInAttackRange;
    private Vector3 _walkPoint;
    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //Patrol if no target
        if (!_target)
        {
            Patrol();
            return;
        }
        
        //I can see you...
        transform.LookAt(_target.transform);

        //Check for sight and attack range
        float distanceToPlayer = Vector3.Distance(_target.transform.position, seePoint.position);
        _playerInSightRange = distanceToPlayer < sightRange;
        _playerInAttackRange = distanceToPlayer < attackRange;
        
        //Assuming sight range is larger than attack range
        if (_playerInSightRange)
        {
            if (_playerInAttackRange)
                StopMoving();
            else
                ChasePlayer();
        }
        else
            Patrol();
    }

    private void StopMoving()
    {
        _agent.SetDestination(transform.position);
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
        if (Vector3.Distance(transform.position, _walkPoint) < 1f)
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
        _walkPointSet = Physics.Raycast(raycastPoint, -transform.up, 10f, whatIsGround);
    }

    private void ChasePlayer()
    {
        _agent.SetDestination(_target.transform.position);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
