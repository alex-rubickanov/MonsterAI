using Pathfinding;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class EnemyController : MonoBehaviour
{
    private Unit unit;

    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private LayerMask mask;
    [SerializeField] private EnemyState state;
    [SerializeField] private float followTimeAfterEscape;

    private bool seePlayer;
    private bool savedSeePlayer;
    private float timer;


    private void Awake()
    {
        unit = GetComponent<Unit>();

        savedSeePlayer = seePlayer;
    }

    private void Start()
    {
        if (!seePlayer)
        {
            unit.GoTo(GetRandomPatrolPoint());
        }
        timer = followTimeAfterEscape;
    }

    private void Update()
    {
        SeePlayer();

        if (savedSeePlayer == seePlayer && seePlayer)
        {
            state = EnemyState.FollowPlayer;
            unit.GoTo(unit.playerTransform);
            savedSeePlayer = !seePlayer;
        }

        if (savedSeePlayer == seePlayer && !seePlayer)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                state = EnemyState.Patrolling;
                unit.GoTo(GetRandomPatrolPoint());
                savedSeePlayer = !seePlayer;
                timer = followTimeAfterEscape;
            }
        }
    }

    private void SeePlayer()
    {
        Physics.Raycast(transform.position, (playerTarget.position - transform.position), out RaycastHit hit, mask);
        if (hit.collider == null)
        {
            seePlayer = true;
        }
        else
        {
            seePlayer = false;
        }
    }

    private Transform GetRandomPatrolPoint()
    {
        int randomIndex = Random.Range(0, patrolPoints.Length);
        return patrolPoints[randomIndex];
    }

    private void OnDrawGizmos()
    {
        if (playerTarget == null) return;

        Gizmos.color = seePlayer ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, (playerTarget.position - transform.position));
    }

    private enum EnemyState
    {
        Patrolling,
        FollowPlayer,
    }
}