using System;
using UnityEngine;

[Serializable]
public class DialogueChoiceSaveData
{
    [field: SerializeField] public string Text { get; set; }
    [field: SerializeField] public string NodeID { get; set; }
}
