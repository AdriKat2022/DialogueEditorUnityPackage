using AdriKat.Editor.DialogueSystem.Graph.Elements;
using System.Collections.Generic;

namespace AdriKat.Editor.DialogueSystem.Graph
{
    public class DialogueGroupErrorData
    {
        public DialogueErrorData ErrorData { get; set; }
        public List<DialogueGroup> Groups { get; set; }

        public DialogueGroupErrorData()
        {
            ErrorData = new DialogueErrorData();
            Groups = new List<DialogueGroup>();
        }
    }
}