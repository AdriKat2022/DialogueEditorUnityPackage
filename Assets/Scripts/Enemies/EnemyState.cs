abstract public class EnemyState
{
    protected EnemyBehaviour _enemy;
    protected EnemyState(EnemyBehaviour enemy)
    {
        _enemy = enemy;
    }

    abstract public void OnEnter();
    abstract public void Update();
    abstract public void OnExit();
}