using UnityEngine;

public class SingleDialogue : TriggerableDialogue
{
    [SerializeField] private Dialogue _dialogue;
    [SerializeField] private DialogueController _dialogueBox;

    public override void Trigger()
    {
        if (_dialogue != null)
        {
            _dialogueBox.StartDialogue(_dialogue);
        }
    }
}
