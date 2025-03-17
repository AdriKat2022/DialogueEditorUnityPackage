using UnityEngine;

public class ChaseEnemyState : EnemyState
{
    private Transform _chasingTransform;

    public ChaseEnemyState(EnemyBehaviour enemy) : base(enemy) { }

    public override void OnEnter()
    {
        _chasingTransform = _enemy.ChasingTransform;
        _enemy.NavMeshAgent.isStopped = false;
    }

    public override void OnExit()
    {
    }

    public override void Update()
    {
        if (_chasingTransform == null || Vector3.Distance(_enemy.transform.position, _chasingTransform.position) > _enemy.MaxChaseRange)
        {
            _enemy.ChangeState(_enemy.IdleState);
            return;
        }

        if (Vector3.Distance(_enemy.transform.position, _chasingTransform.position) < _enemy.AttackRange)
        {
            _enemy.ChangeState(_enemy.AttackState);
            return;
        }

        _enemy.SetDestination(_chasingTransform.position);
    }
}
