using UnityEngine;

public class IdleEnemyState : EnemyState
{
    private float _timer = 0;

    public IdleEnemyState(EnemyBehaviour enemy) : base(enemy) { }

    public override void OnEnter()
    {
        _enemy.NavMeshAgent.isStopped = true;
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer < 0)
        {
            _enemy.EnemySprite.transform.Rotate(0, 0, Random.Range(0, 360));
            _timer = Random.Range(_enemy.MinTimeWaitIdleAnimation, _enemy.MaxTimeWaitIdleAnimation);
        }

        CheckPlayerRange();
    }

    private void CheckPlayerRange()
    {
        // If we see the player, we change to the ChaseState
        Collider2D collider2D = Physics2D.OverlapCircle(_enemy.transform.position, _enemy.IdleRangeDetection, _enemy.PlayerLayer);
        if (collider2D)
        {
            _enemy.ChasingTransform = collider2D.transform;
            _enemy.ChangeState(_enemy.PatrolState);
        }
    }
}
