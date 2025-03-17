
using UnityEngine;

public class AttackEnemyState : EnemyState
{
    private HealthManager _playerHealthManager;
    private bool _isDamageable = false;
    private float _attackTimer = 0;

    public AttackEnemyState(EnemyBehaviour enemy) : base(enemy) { }

    public override void OnEnter()
    {
        _enemy.NavMeshAgent.isStopped = true;

        if (_enemy.ChasingTransform.TryGetComponent(out HealthManager healthManager))
        {
            _playerHealthManager = healthManager;
            _isDamageable = true;
        }
        else
        {
            Debug.LogWarning("No health manager attached to the chased game object.");
        }

        _attackTimer = _enemy.AttackCooldown / 2f;
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        if (_enemy.ChasingTransform == null || Vector3.Distance(_enemy.transform.position, _enemy.ChasingTransform.position) > _enemy.AttackRange)
        {
            _enemy.ChangeState(_enemy.ChaseState);
            return;
        }

        if (_attackTimer < 0)
        {
            if (_isDamageable)
            {
                _playerHealthManager.DealDamage((int)_enemy.AttackDamage);
            }
            _attackTimer = _enemy.AttackCooldown;
        }
        _attackTimer -= Time.deltaTime;
    }
}
