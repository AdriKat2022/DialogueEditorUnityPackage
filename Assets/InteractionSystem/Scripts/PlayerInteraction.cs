using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private GameObject _interactionIcon;
    [SerializeField] private InputActionReference _interactionInputReference;

    private Interactable _interactable;

    private void OnEnable()
    {
        _interactionInputReference.action.Enable();
        _interactionInputReference.action.performed += OnInteractionInput;
    }

    private void OnDisable()
    {
        _interactionInputReference.action.performed -= OnInteractionInput;
        _interactionInputReference.action.Disable();
    }

    private void OnInteractionInput(InputAction.CallbackContext context)
    {
        if (_interactable == null)
        {
            Debug.LogWarning("Failed interaction!");
            return;
        }

        _interactable.Interact();
        _interactable = null;
        _interactionIcon.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_interactable != null)
        {
            return;
        }

        if (collision.TryGetComponent(out _interactable))
        {
            _interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Interactable interactable = collision.GetComponent<Interactable>();

        if (_interactable != interactable)
        {
            return;
        }

        _interactable = null;
        _interactionIcon.SetActive(false);
    }
}
