using UnityEngine;

namespace AdriKat.DialogueSystem.Conditions
{

    [CreateAssetMenu(fileName = "DialogueVariableNames", menuName = "Dialogue/Conditions Names", order = 1)]
    public class ConditionVariableNamesSO : ScriptableObject
    {
        [field: SerializeField] public string[] BoolVarNames { get; private set; }
        [field: SerializeField] public string[] IntVarNames { get; private set; }
        [field: SerializeField] public string[] StringVarNames { get; private set; }

        public string[] GetVarNames(Condition.DialogueVariableType valueType)
        {
            return valueType switch
            {
                Condition.DialogueVariableType.Bool => BoolVarNames,
                Condition.DialogueVariableType.Int => IntVarNames,
                Condition.DialogueVariableType.String => StringVarNames,
                _ => null,
            };
        }
    }
}