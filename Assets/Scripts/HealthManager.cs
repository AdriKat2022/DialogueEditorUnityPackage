using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour
{
    #region Serialized Fields and Getters
    [Header("Health Settings")]
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _currentHealth = 100;

    [Header("Additional Settings")]
    [SerializeField] private float defense = 0f;

    [Header("Animation Settings")]
    [SerializeField] private bool shakeCameraOnHit = false;
    [SerializeField] private float baseShakeIntensity = 0.2f;
    [SerializeField] private float shakePerDamageIntensity = 0.02f;

    [Header("Events")]
    public UnityEvent OnDeath;
    public delegate void HealthChangedEvent(int oldHealth, int currentHealth, int maxHealth);
    public HealthChangedEvent OnHealthChanged;

    public bool IsDead => _isDead;
    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;

    #endregion

    private bool _isDead;

    #region Health Control Functions
    public void DealDamage(int damage)
    {
        int computedDamage = (int)Mathf.Max(damage - defense, 0);

        int oldHealth = _currentHealth;
        _currentHealth = Mathf.Clamp(_currentHealth - computedDamage, 0, _maxHealth);

        OnHealthChanged?.Invoke(oldHealth, _currentHealth, _maxHealth);
        CheckDeath();
    }

    public void Heal(int heal)
    {
        int oldHealth = _currentHealth;
        _currentHealth = Mathf.Clamp(_currentHealth + heal, 0, _maxHealth);

        OnHealthChanged?.Invoke(oldHealth, _currentHealth, _maxHealth);
        CheckDeath();
    }

    public void FullHeal()
    {
        int oldHealth = _currentHealth;
        _currentHealth = _maxHealth;

        OnHealthChanged?.Invoke(oldHealth, _currentHealth, _maxHealth);
    }

    public void InstaKill()
    {
        int oldHealth = _currentHealth;
        _currentHealth = 0;

        OnHealthChanged?.Invoke(oldHealth, _currentHealth, _maxHealth);
        CheckDeath();
    }
    #endregion

    private void CheckDeath()
    {
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            _isDead = true;
            OnDeath.Invoke();
        }
    }
}
