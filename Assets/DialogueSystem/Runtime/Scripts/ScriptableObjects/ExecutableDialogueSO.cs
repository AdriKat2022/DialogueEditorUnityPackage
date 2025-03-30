using AdriKat.DialogueSystem.Enumerations;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdriKat.DialogueSystem.Data
{
    [Serializable]
    public class ExecutableDialogueSO : DialogueSO
    {
        [field: SerializeField, TextArea()] public string Text { get; set; }
        [field: SerializeField] public List<DialogueChoiceData> Choices { get; set; }
        [field: SerializeField] public DialogueAuthorData Author { get; set; }
        [field: SerializeField] public DialogueType Type { get; set; }

        public void Initialize(string dialogueName, string text, List<DialogueChoiceData> choices, DialogueAuthorData author, DialogueType dialogueType, bool isStartingDialogue)
        {
            DialogueName = dialogueName;
            Text = text;
            Choices = choices;
            Author = author;
            Type = dialogueType;
            IsStartingDialogue = isStartingDialogue;
        }
    }
}