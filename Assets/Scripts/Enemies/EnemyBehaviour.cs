using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    #region Serialized Fields
    // I don't like having all fields public, but it's easier to access them from the states themselves.
    // Otherwise, I would have to create a lot of getters and setters, which would make the code harder to read.

    [Header("Speed & properties")]
    public float Speed = 2f;
    public LayerMask WallLayer;
    public GameObject EnemySprite;

    [Header("Idle")]
    public float MinTimeWaitIdleAnimation = 2f;
    public float MaxTimeWaitIdleAnimation = 5f;
    public float IdleRangeDetection = 10f;
    public LayerMask PlayerLayer;

    [Header("Patrol")]
    public float PatrolingRangeDetection = 5f;
    public float PatrolPointReachPrecision = 0.2f;
    public Transform[] PossiblePatrolPoints;
    public Animator PatrolIcon;

    [Header("Chasing")]
    public float MaxChaseRange = 6f;
    public float AttackRange = 1f;
    public Animator ChasingIcon;

    [Header("Attacking")]
    public float AttackCooldown = 1f;
    public float AttackDamage = 1f;
    public Animator AttackingIcon;

    [Header("Debug")]
    [SerializeField] private bool _showDebug = false;
    #endregion

    // EnemyStates Variables
    private EnemyState _currentState;
    [HideInInspector] public IdleEnemyState IdleState;
    [HideInInspector] public ChaseEnemyState ChaseState;
    [HideInInspector] public PatrolEnemyState PatrolState;
    [HideInInspector] public AttackEnemyState AttackState;

    [HideInInspector] public Transform ChasingTransform;

    [HideInInspector] public NavMeshAgent NavMeshAgent;

    #region Gizmos
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!_showDebug) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, IdleRangeDetection);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, PatrolingRangeDetection);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, MaxChaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
#endif
    #endregion

    private void Start()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        NavMeshAgent.speed = Speed;
        NavMeshAgent.autoBraking = false;
        NavMeshAgent.updateRotation = false;
        NavMeshAgent.updateUpAxis = false;
        NavMeshAgent.stoppingDistance = 0.01f;

        // Initialize EnemyStates
        IdleState = new IdleEnemyState(this);
        ChaseState = new ChaseEnemyState(this);
        PatrolState = new PatrolEnemyState(this);
        AttackState = new AttackEnemyState(this);

        ChangeState(IdleState);
    }

    private void Update()
    {
        _currentState?.Update();
        UpdateRotation();
    }

    /// <summary>
    /// Should be used to move the enemy since this function takes care of wall check and sprite rotation.
    /// </summary>
    /// <param name="deltaPosition">The movement vector.</param>
    [Obsolete("Use SetDestination instead.")]
    public void Move(Vector3 deltaPosition)
    {
        if (Physics2D.OverlapCircle(transform.position + deltaPosition, 0.1f, WallLayer) == null)
        {
            transform.position += deltaPosition;
            EnemySprite.transform.up = deltaPosition.normalized;
        }
    }

    /// <summary>
    /// Sets the destination of the NavMeshAgent. Should be used to move the enemy. 
    /// </summary>
    /// <param name="destination"></param>
    public void SetDestination(Vector3 destination)
    {
        NavMeshAgent.SetDestination(destination);
    }

    /// <summary>
    /// Changes the state of the enemy. Use the variables IdleState, ChaseState, PatrolState and AttackState to change the state.
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(EnemyState newState)
    {
        if (_showDebug)
        {
            Debug.Log($"Changing state from {_currentState?.GetType().Name} to {newState.GetType().Name}");
        }

        _currentState?.OnExit();
        _currentState = newState;
        _currentState.OnEnter();

        // Update the icons
        switch (_currentState)
        {
            case IdleEnemyState _:
                PatrolIcon.gameObject.SetActive(false);
                ChasingIcon.gameObject.SetActive(false);
                AttackingIcon.gameObject.SetActive(false);
                break;
            case ChaseEnemyState _:
                PatrolIcon.gameObject.SetActive(false);
                ChasingIcon.gameObject.SetActive(true);
                AttackingIcon.gameObject.SetActive(false);
                break;
            case PatrolEnemyState _:
                PatrolIcon.gameObject.SetActive(true);
                ChasingIcon.gameObject.SetActive(false);
                AttackingIcon.gameObject.SetActive(false);
                break;
            case AttackEnemyState _:
                PatrolIcon.gameObject.SetActive(false);
                ChasingIcon.gameObject.SetActive(false);
                AttackingIcon.gameObject.SetActive(true);
                break;
        }
    }

    private void UpdateRotation()
    {
        // Get the speed of the navMeshAgent
        Vector3 speed = NavMeshAgent.velocity;

        // Apply the rotation according to the speed
        if (speed != Vector3.zero)
        {
            EnemySprite.transform.up = speed.normalized;
        }
    }
}
