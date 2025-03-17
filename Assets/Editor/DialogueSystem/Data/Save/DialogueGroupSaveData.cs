using UnityEngine;

namespace AdriKat.Editor.DialogueSystem.Graph.Data
{
    [System.Serializable]
    public class DialogueGroupSaveData
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }

    }
}