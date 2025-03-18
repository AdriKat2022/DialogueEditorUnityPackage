using AdriKat.DialogueSystem.Variables;
using System;
using UnityEngine;

namespace AdriKat.DialogueSystem.Data
{
    [Serializable]
    public class DialogueConditionData
    {
        [field: SerializeField] public DialogueVariableNamesSO DialogueVariablesNamesSO;

        [field: SerializeField] public DialogueVariableType ConditionValueType { get; set; }
        [field: Space]
        [field: SerializeField] public string BoolKey { get; set; }
        [field: SerializeField] public BoolComparisonTypeEnum BoolComparisonType { get; set; }
        [field: SerializeField] public bool BoolValue { get; set; }
        [field: Space]
        [field: SerializeField] public string IntKey { get; set; }
        [field: SerializeField] public IntComparisonTypeEnum IntComparisonType { get; set; }
        [field: SerializeField] public int IntValue { get; set; }
        [field: Space]
        [field: SerializeField] public string StringKey { get; set; }
        [field: SerializeField] public StringComparisonTypeEnum StringComparisonType { get; set; }
        [field: SerializeField] public string StringValue { get; set; }

        public bool Evaluate()
        {
            switch (ConditionValueType)
            {
                case DialogueVariableType.Bool:
                    bool? value = DialogueVariables.GetBool(BoolKey);

                    if (value != null)
                    {
                        // Swtich return according to the bool comparison
                        return BoolComparisonType switch
                        {
                            BoolComparisonTypeEnum.Is => (bool)value == BoolValue,
                            BoolComparisonTypeEnum.And => (bool)value && BoolValue,
                            BoolComparisonTypeEnum.Or => (bool)value || BoolValue,
                            BoolComparisonTypeEnum.Xor => (bool)value ^ BoolValue,
                            _ => false
                        };
                    }

                    Debug.LogError($"Bool variable with key '{BoolKey}' not found.");
                    break;

                case DialogueVariableType.Int:
                    int? intValue = DialogueVariables.GetInt(IntKey);

                    if (intValue != null)
                    {
                        // Swtich return according to the int comparison
                        return IntComparisonType switch
                        {
                            IntComparisonTypeEnum.Equal => intValue == IntValue,
                            IntComparisonTypeEnum.NotEqual => intValue != IntValue,
                            IntComparisonTypeEnum.Greater => intValue > IntValue,
                            IntComparisonTypeEnum.GreaterOrEqual => intValue >= IntValue,
                            IntComparisonTypeEnum.Less => intValue < IntValue,
                            IntComparisonTypeEnum.LessOrEqual => intValue <= IntValue,
                            _ => false
                        };
                    }

                    Debug.LogError($"Int variable with key '{IntKey}' not found.");
                    break;

                case DialogueVariableType.String:
                    string stringValue = DialogueVariables.GetString(StringKey);

                    if (stringValue != null)
                    {
                        return StringComparisonType switch
                        {
                            StringComparisonTypeEnum.Equal => stringValue == StringValue,
                            StringComparisonTypeEnum.NotEqual => stringValue != StringValue,
                            StringComparisonTypeEnum.Contains => stringValue.Contains(StringValue),
                            StringComparisonTypeEnum.StartsWith => stringValue.StartsWith(StringValue),
                            StringComparisonTypeEnum.EndsWith => stringValue.EndsWith(StringValue),
                            _ => false
                        };
                    }

                    Debug.LogError($"String variable with key '{StringKey}' not found.");
                    break;
            }

            return false;
        }

        #region Enums
        public enum DialogueVariableType
        {
            Bool,
            Int,
            String
        }


        public enum BoolComparisonTypeEnum
        {
            Is,
            And,
            Or,
            Xor
        }

        public enum IntComparisonTypeEnum
        {
            Equal,
            NotEqual,
            Greater,
            GreaterOrEqual,
            Less,
            LessOrEqual
        }

        public enum StringComparisonTypeEnum
        {
            Equal,
            NotEqual,
            Contains,
            StartsWith,
            EndsWith
        }
        #endregion
    }
}