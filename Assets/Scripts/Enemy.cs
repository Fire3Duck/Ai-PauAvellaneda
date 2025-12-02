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
    Transform _player;

    Vector3 _playerLastPositionKnow;

    //Patrullar
    [SerializeField] private Transform[] _patrolPoints;

    

    //Detección
    [SerializeField] private float _detectionRange = 4;

    //Busqueda
    private float _searchTimer;
    [SerializeField] private float _searchWaitTime = 15;
    [SerializeField] private float _searchRadius = 10;

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
            currentState = EnemyState.Searching;
        }
        _enemyAgent.SetDestination(_player.position);

        _playerLastPositionKnow = _player.position;
    }

    void Search()
    {
        if(OnRange())
        {
            currentState = EnemyState.Chasing;
        }

        _searchTimer += Time.deltaTime;

        if(_searchTimer < _searchWaitTime)
        {
            if(_enemyAgent.remainingDistance < 0.5f)
            {
                Vector3 randomPoint;
                if(RandomSearchPoint(_playerLastPositionKnow, _searchRadius, out randomPoint))
                {
                    _enemyAgent.SetDestination(randomPoint);
                }
            }
        }
        else
        {
            currentState =EnemyState.Patrolling;
            _searchTimer = 0;
        }
    }

    bool RandomSearchPoint(Vector3 center, float radius, out Vector3 point)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * radius;

        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomPoint, out hit, 4, NavMesh.AllAreas))
        {
            point = hit.position;
            return true;
        }

        point = Vector3.zero;
        return false;
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
