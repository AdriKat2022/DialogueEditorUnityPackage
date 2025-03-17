using AdriKat.Editor.DialogueSystem.Graph.Elements;
using System.Collections.Generic;

namespace AdriKat.Editor.DialogueSystem.Graph
{
    public class DialogueNodeErrorData
    {
        public DialogueErrorData ErrorData { get; set; }
        public List<DialogueNode> Nodes { get; set; }

        public DialogueNodeErrorData()
        {
            ErrorData = new DialogueErrorData();
            Nodes = new List<DialogueNode>();
        }
    }
}