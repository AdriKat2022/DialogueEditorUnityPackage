using UnityEngine;

public class PlayerModifierArea : MonoBehaviour
{
    [SerializeField] private int _speedMultiplierModifier = 20;
    [SerializeField] private ParticleSystem _particleEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<IsometricMovementController>().SpeedMultiplier += _speedMultiplierModifier;
            if (_particleEffect != null)
            {
                _particleEffect.Play();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<IsometricMovementController>().SpeedMultiplier -= _speedMultiplierModifier;
            if (_particleEffect != null)
            {
                _particleEffect.Stop();
            }
        }
    }
}
