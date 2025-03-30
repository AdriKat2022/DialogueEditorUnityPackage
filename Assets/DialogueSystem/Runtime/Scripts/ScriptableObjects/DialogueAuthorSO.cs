using AdriKat.DialogueSystem.Utility;
using UnityEngine;

namespace AdriKat.DialogueSystem.Data
{
    [CreateAssetMenu(fileName = "DialogueAuthor", menuName = "Dialogue System/Dialogue Author")]

    public class DialogueAuthorSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public SerializableDictionary<string, Sprite> Sprites { get; set; }
    }
}
