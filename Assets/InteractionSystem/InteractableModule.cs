using UnityEngine;
using UnityEngine.Events;

public class InteractableModule : MonoBehaviour, Interactable
{
    [SerializeField] private UnityEvent _onInteract;

    public void Interact()
    {
        _onInteract.Invoke();
    }
}
