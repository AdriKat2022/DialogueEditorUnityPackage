using AdriKat.DialogueSystem.Enumerations;
using AdriKat.Editor.DialogueSystem.Graph.Data;
using AdriKat.Editor.DialogueSystem.Utility;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace AdriKat.Editor.DialogueSystem.Graph.Elements
{
    public class DialogueSingleChoiceNode : DialogueNode
    {
        public override void Initialize(string nodeName, DialogueGraphView graphView, Vector2 position)
        {
            base.Initialize(nodeName, graphView, position);

            Type = DialogueType.SingleChoice;

            DialogueChoiceSaveData choiceData = new()
            {
                Text = "Next Dialogue",
            };

            Choices.Add(choiceData);
        }

        public override void Draw()
        {
            base.Draw();

            foreach (DialogueChoiceSaveData choice in Choices)
            {
                Port choicePort = this.CreatePort(choice.Text);
                choicePort.userData = choice;
                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }

    }
}