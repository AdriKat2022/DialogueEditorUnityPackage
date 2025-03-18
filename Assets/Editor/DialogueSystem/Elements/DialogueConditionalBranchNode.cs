using AdriKat.DialogueSystem.Data;
using AdriKat.DialogueSystem.Enumerations;
using AdriKat.DialogueSystem.Graph;
using AdriKat.DialogueSystem.Utility;
using System.Collections.Generic;
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

            // Main Container
            Button addConditionButton = DialogueElementUtility.CreateButton("Add Condition", () =>
            {
                DialogueConditionData conditionData = new();

                VisualElement choicePort = CreateCondition(conditionData);
                outputContainer.Add(choicePort);
            });
            addConditionButton.AddToClassList("ds-node__button");
            mainContainer.Insert(1, addConditionButton);

            // Output Container
            foreach (DialogueChoiceSaveData choice in Choices)
            {
                VisualElement choicePort = CreateCondition(choice);
                outputContainer.Add(choicePort);
            }
            RefreshExpandedState();
        }

        private VisualElement CreateCondition(object userData)
        {
            VisualElement condtitionContainer = new();
            DialogueConditionData conditionalData = (DialogueConditionData)userData;

            Button deleteButton = DialogueElementUtility.CreateButton("X", () =>
            {
                if (Conditions.Count == 1)
                {
                    return;
                }

                Conditions.Remove(conditionalData);
            });

            deleteButton.AddToClassList("ds-node__button");

            // TODO: Make text fields for
            // - ConditionType
            // - ConditionVariableName
            // - ConditionOperator
            // - ConditionValue

            condtitionContainer.Add(deleteButton);

            outputContainer.Add(condtitionContainer);

            return condtitionContainer;
        }
    }
}