using System;
using UnityEngine;

namespace AdriKat.DialogueSystem.Data
{
    [Serializable]
    public class DialogueAuthorData
    {
        [field: SerializeField] public DialogueAuthorSO AuthorData { get; set; }
        [field: SerializeField] public bool ShowMugshot { get; set; } = true;
        [field: SerializeField] public string Emotion { get; set; } = "Default";
    }
}
