using AdriKat.DialogueSystem.Enumerations;
using AdriKat.DialogueSystem.Triggers;
using UnityEngine;

namespace AdriKat.DialogueSystem.Conditions
{
    public class ConditionalDialogue : TriggerableDialogue
    {
        [SerializeField] private DialogueVariableNamesSO _dialogueVariablesNamesSO;

        [Header("Condition(s)")]
        [SerializeField] private Condition[] _conditions;
        [SerializeField] private ConditionType _conditionsToBeMet;

        [Header("Dialogues")]
        [SerializeField] private TriggerableDialogue _dialogueOnTrue;
        [SerializeField] private TriggerableDialogue _dialogueOnFalse;

        public override void Trigger()
        {
            if (_dialogueVariablesNamesSO == null)
            {
                Debug.LogWarning("No DialogueVariablesNamesSO assigned to this conditional dialogue.", gameObject);
                return;
            }

            if (_conditions == null || _conditions.Length == 0)
            {
                Debug.LogWarning("No conditions set for this conditional dialogue.", gameObject);
                return;
            }

            // Check that all conditions are met
            bool finalResult = false;

            if (_conditionsToBeMet == ConditionType.All)
            {
                finalResult = true;
                foreach (Condition condition in _conditions)
                {
                    if (!condition.Evaluate())
                    {
                        finalResult = false;
                        break;
                    }
                }
            }
            else if (_conditionsToBeMet == ConditionType.Any)
            {
                finalResult = false;
                foreach (Condition condition in _conditions)
                {
                    if (condition.Evaluate())
                    {
                        finalResult = true;
                        break;
                    }
                    else
                    {
                        Debug.Log("Condition not met: " + condition);
                    }
                }
            }

            if (finalResult)
            {
                if (_dialogueOnTrue != null)
                {
                    _dialogueOnTrue.Trigger();
                }
            }
            else
            {
                if (_dialogueOnFalse != null)
                {
                    _dialogueOnFalse.Trigger();
                }
            }
        }


    }
}