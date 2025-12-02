using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent _enemyAgent;

    public enum EnemyState
    {
        Patrolling,
        Chasing,
        Searching
    }

    public EnemyState currentState;

    [SerializeField] private Transform[] _patrolPoints;

    Transform _player;

    [SerializeField] private float _detectionRange = 7;

    void Awake()
    {
        _enemyAgent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player").transform;
    }

    void Start()
    {
        currentState = EnemyState.Patrolling;
        SetRandomPatrolPoint();
    }

    
    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
            break;
            case EnemyState.Chasing:
                Chase();
            break;
            case EnemyState.Searching:
                Search();
            break;

            default:
                Patrol();
            break;
        }
    }

    void Patrol()
    {
        if(OnRange())
        {
            currentState = EnemyState.Chasing;
        }

        if(_enemyAgent.remainingDistance < 0.5f)
        {
            SetRandomPatrolPoint();
        }
    }

    void Chase()
    {
        if(!OnRange())
        {
            currentState = EnemyState.Patrolling;
        }
        _enemyAgent.SetDestination(_player.position);
    }

    void Search()
    {
        
    }

    void SetRandomPatrolPoint()
    {
        _enemyAgent.SetDestination(_patrolPoints[Random.Range(0, _patrolPoints.Length)].position);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        foreach (Transform point in _patrolPoints)
        {
            Gizmos.DrawWireSphere(point.position, 0.5f);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }

    bool OnRange()
    {
        if(Vector3.Distance(transform.position, _player.position) < _detectionRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
