using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PatrolEnemyState : EnemyState
{
    private readonly Transform[] _patrolPoints = new Transform[2];
    private bool _patrolAvailable = true;
    private int _patrolIndex = 0;

    public PatrolEnemyState(EnemyBehaviour enemy) : base(enemy) { }

    public override void OnEnter()
    {
        RefreshPatrolPoints();
        _enemy.NavMeshAgent.isStopped = false;
    }

    private void RefreshPatrolPoints()
    {
        if (_enemy.PossiblePatrolPoints.Length < 2)
        {
            Debug.LogWarning("Not enough patrol points to patrol.");
            _patrolAvailable = false;
            return;
        }

        // Get two random patrol points
        int[] randomPoints = UniqueRandomList(DateTime.Now.Millisecond, _enemy.PossiblePatrolPoints.Length, 2);

        _patrolPoints[0] = _enemy.PossiblePatrolPoints[randomPoints[0]];
        _patrolPoints[1] = _enemy.PossiblePatrolPoints[randomPoints[1]];

        Debug.Log($"Patrol points: {_patrolPoints[0].name} and {_patrolPoints[1].name}");

        _patrolIndex = 0;
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        CheckPlayerRange();

        if (!_patrolAvailable) return;

        ManagePatroling();
    }

    private void ManagePatroling()
    {
        // Move towards the patrol point
        _enemy.SetDestination(_patrolPoints[_patrolIndex].position);

        if (!_enemy.NavMeshAgent.pathPending && _enemy.NavMeshAgent.remainingDistance < _enemy.PatrolPointReachPrecision)
        {
            _patrolIndex = (_patrolIndex + 1) % 2;
        }
    }

    private void CheckPlayerRange()
    {
        // If we see the player, we change to the ChaseState
        Collider2D collider2D = Physics2D.OverlapCircle(_enemy.transform.position, _enemy.PatrolingRangeDetection, _enemy.PlayerLayer);
        if (collider2D)
        {
            _enemy.ChasingTransform = collider2D.transform;
            _enemy.ChangeState(_enemy.ChaseState);
            return;
        }

        // If the player is too far, we change to the IdleState
        collider2D = Physics2D.OverlapCircle(_enemy.transform.position, _enemy.IdleRangeDetection, _enemy.PlayerLayer);
        if (!collider2D)
        {
            _enemy.ChangeState(_enemy.IdleState);
        }
    }

    // I'll move this region to a helper package later
    #region Randomness Code

    /// <summary>
    /// Generates a list of unique random numbers between 0 and N-1
    /// </summary>
    /// <param name="seed">Randomness seed, same seed will produce same results.</param>
    /// <param name="N">The maximum selectable number (excluded).</param>
    /// <param name="M">How many unique numbers will be returned.</param>
    /// <returns>A list of M positive unique numbers strictly less than N.</returns>
    /// <exception cref="ArgumentException"></exception>
    static int[] UniqueRandomList(int seed, int N, int M)
    {
        if (N < 2) throw new ArgumentException("N must be at least 2");
        if (M > N || M < 1) throw new ArgumentException("M must be between 1 and N");

        HashSet<int> selectedNumbers = new(); // Ensures uniqueness
        int baseSeed = seed % N; // Initial seed

        while (selectedNumbers.Count < M)
        {
            int newNumber = (baseSeed + seed * selectedNumbers.Count) % N;
            selectedNumbers.Add(newNumber);
            baseSeed = (baseSeed + 1) % N; // Adjust to reduce clustering
        }

        return selectedNumbers.ToArray();
    }
    #endregion
}
