using AdriKat.DialogueSystem.Enumerations;
using System.Collections.Generic;
using UnityEngine;

namespace AdriKat.DialogueSystem.Data
{
    [System.Serializable]
    public class DialogueNodeSaveData
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public List<DialogueChoiceSaveData> Choices { get; set; }
        [field: SerializeField] public string GroupID { get; set; }
        [field: SerializeField] public DialogueType Type { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
    }
}