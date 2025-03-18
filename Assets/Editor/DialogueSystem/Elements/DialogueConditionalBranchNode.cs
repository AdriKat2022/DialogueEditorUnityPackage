using AdriKat.DialogueSystem.Data;
using AdriKat.DialogueSystem.Enumerations;
using AdriKat.DialogueSystem.Graph;
using AdriKat.DialogueSystem.Utility;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AdriKat.DialogueSystem.Elements
{
    public class DialogueConditionalBranchNode : DialogueNode
    {
        [field: SerializeField] public List<DialogueConditionData> Conditions { get; set; }
        [field: SerializeField] public ConditionType ConditionToBeMet { get; set; }
        [field: SerializeField] public string NodeOnTrue { get; set; }
        [field: SerializeField] public string NodeOnFalse { get; set; }

        public override void Initialize(string nodeName, DialogueGraphView graphView, Vector2 position)
        {
            base.Initialize(nodeName, graphView, position);
            Type = DialogueType.ConditionalBranch;
            Conditions = new List<DialogueConditionData>();
            ConditionToBeMet = ConditionType.All;
        }

        public override void Draw()
        {
            base.Draw();

            Foldout conditionsFoldout = DialogueElementUtility.CreateFoldout("Conditions");
            conditionsFoldout.AddToClassList("ds-node__foldout");
            mainContainer.Insert(1, conditionsFoldout);

            // Main Container
            Button addConditionButton = DialogueElementUtility.CreateButton("Add Condition", () =>
            {
                DialogueConditionData conditionData = new();

                CreateCondition(conditionData, conditionsFoldout);
            });
            addConditionButton.AddToClassList("ds-node__button");
            mainContainer.Insert(1, addConditionButton);

            // Output Container, make two ports for true and false
            Port truePort = this.CreatePort("True", Orientation.Horizontal, Direction.Output);
            truePort.portName = "True";
            truePort.userData = NodeOnTrue;
            outputContainer.Add(truePort);

            Port falsePort = this.CreatePort("False", Orientation.Horizontal, Direction.Output);
            falsePort.portName = "False";
            falsePort.userData = NodeOnFalse;
            outputContainer.Add(falsePort);


            RefreshExpandedState();
        }

        private VisualElement CreateCondition(DialogueConditionData conditionalData, Foldout conditionsContainer)
        {
            VisualElement condtitionContainer = new();

            Button deleteButton = DialogueElementUtility.CreateButton("X", () =>
            {
                if (Conditions.Count < 1)
                {
                    return;
                }

                Conditions.Remove(conditionalData);
                conditionsContainer.Remove(condtitionContainer);
            });

            deleteButton.AddToClassList("ds-node__button");

            // TODO: Make text fields for
            // - ConditionType
            // - ConditionVariableName
            // - ConditionOperator
            // - ConditionValue

            Conditions.Add(conditionalData);
            condtitionContainer.Add(deleteButton);
            conditionsContainer.Add(condtitionContainer);

            return condtitionContainer;
        }
    }
}