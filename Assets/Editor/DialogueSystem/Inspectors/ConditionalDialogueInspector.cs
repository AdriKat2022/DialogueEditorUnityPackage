using AdriKat.DialogueSystem.Conditions;
using UnityEditorInternal;

namespace AdriKat.Editor.DialogueSystem.Inspector.Conditions
{
    using System;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(ConditionalDialogue))]
    public class ConditionalDialogueInspector : Editor
    {
        private SerializedProperty _dialogueVariablesNamesSO;

        private SerializedProperty _conditions;
        private SerializedProperty _conditionsToBeMet;

        private SerializedProperty _dialogueOnTrue;
        private SerializedProperty _dialogueOnFalse;

        private ReorderableList reorderableList;

        private void OnEnable()
        {
            _dialogueVariablesNamesSO = serializedObject.FindProperty("_dialogueVariablesNamesSO");

            _conditions = serializedObject.FindProperty("_conditions");
            _conditionsToBeMet = serializedObject.FindProperty("_conditionsToBeMet");

            _dialogueOnTrue = serializedObject.FindProperty("_dialogueOnTrue");
            _dialogueOnFalse = serializedObject.FindProperty("_dialogueOnFalse");

            reorderableList = new ReorderableList(serializedObject, _conditions, true, true, true, true)
            {
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Conditions"),
                drawElementCallback = DrawConditionCallback,
                elementHeight = EditorGUIUtility.singleLineHeight * 3.2f
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space(8);

            EditorGUILayout.LabelField("Dialogue Variables Names", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_dialogueVariablesNamesSO);

            EditorGUILayout.Space(8);

            if (_dialogueVariablesNamesSO == null || _dialogueVariablesNamesSO.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Assign the DialogueVariableNamesSO scriptable object to get the variable names.\n" +
                    "To create one, Right Click -> Dialogue -> Condition Names.", MessageType.Warning);

                // Button to create a new DialogueVariableNamesSO
                if (GUILayout.Button("Create Dialogue Variable Environment"))
                {
                    _dialogueVariablesNamesSO.objectReferenceValue = MakeNewVariableEnvironment();
                }

                serializedObject.ApplyModifiedProperties();
                return;
            }

            EditorGUILayout.Space(8);

            reorderableList.DoLayoutList();

            // Display a warning if the list is empty
            if (_conditions.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No conditions are set.\n" +
                    "The dialogue will throw an error if you attempt to execute it.", MessageType.Warning);
            }

            EditorGUILayout.Space(8);
            EditorGUILayout.PropertyField(_conditionsToBeMet);
            EditorGUILayout.Space(8);
            EditorGUILayout.PropertyField(_dialogueOnTrue);
            EditorGUILayout.PropertyField(_dialogueOnFalse);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawConditionCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            // Rect is the space given for the element

            SerializedProperty element = _conditions.GetArrayElementAtIndex(index);

            int conditionTypeIndex = element.FindPropertyRelative("_conditionValueType").enumValueIndex;
            string conditionTypeString = element.FindPropertyRelative("_conditionValueType").enumDisplayNames[conditionTypeIndex];

            rect.y += 2;

            Rect conditionTypeSelectionRectHeader =
                new(rect.x, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight);
            Rect conditionTypeSelectionRect =
                new(rect.x + rect.width / 3, rect.y, 2 * rect.width / 3, EditorGUIUtility.singleLineHeight);

            Rect variableNameChoiceRect =
                new(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 1.5f, 3 * rect.width / 10f, EditorGUIUtility.singleLineHeight);
            Rect comparisonTypeRect =
                new(rect.x + 4 * rect.width / 10f, rect.y + EditorGUIUtility.singleLineHeight * 1.5f, 2 * rect.width / 10f, EditorGUIUtility.singleLineHeight);
            Rect valueToCompareRect =
                new(rect.x + 7 * rect.width / 10f, rect.y + EditorGUIUtility.singleLineHeight * 1.5f, 2 * rect.width / 10f, EditorGUIUtility.singleLineHeight);

            // Draw the condition type in the middle of the rect
            EditorGUI.LabelField(conditionTypeSelectionRectHeader, "Condition Type", EditorStyles.boldLabel);
            EditorGUI.PropertyField(conditionTypeSelectionRect, element.FindPropertyRelative("_conditionValueType"), GUIContent.none);

            switch ((Condition.DialogueVariableType)conditionTypeIndex)
            {
                case Condition.DialogueVariableType.Bool:
                    SerializedProperty boolKey = element.FindPropertyRelative("_boolKey");
                    SerializedProperty boolComparisonType = element.FindPropertyRelative("_boolComparisonType");
                    SerializedProperty boolValue = element.FindPropertyRelative("_boolValue");
                    DrawDropdownForPropertyAndType(variableNameChoiceRect, boolKey, Condition.DialogueVariableType.Bool);
                    boolComparisonType.enumValueFlag = (int)(Condition.BoolComparisonType)EditorGUI.EnumPopup(
                        comparisonTypeRect,
                        (Condition.BoolComparisonType)boolComparisonType.enumValueFlag
                        );
                    boolValue.boolValue = EditorGUI.Toggle(valueToCompareRect, boolValue.boolValue);
                    break;
                case Condition.DialogueVariableType.Int:
                    SerializedProperty intKey = element.FindPropertyRelative("_intKey");
                    SerializedProperty intComparisonType = element.FindPropertyRelative("_intComparisionType");
                    SerializedProperty intValue = element.FindPropertyRelative("_intValue");
                    DrawDropdownForPropertyAndType(variableNameChoiceRect, intKey, Condition.DialogueVariableType.Int);
                    intComparisonType.enumValueFlag = (int)(Condition.IntComparisonType)EditorGUI.EnumPopup(
                        comparisonTypeRect,
                        (Condition.IntComparisonType)intComparisonType.enumValueFlag
                        );
                    intValue.intValue = EditorGUI.IntField(valueToCompareRect, intValue.intValue);
                    break;
                case Condition.DialogueVariableType.String:
                    SerializedProperty stringKey = element.FindPropertyRelative("_stringKey");
                    SerializedProperty stringValue = element.FindPropertyRelative("_stringValue");
                    SerializedProperty stringComparisonType = element.FindPropertyRelative("_stringComparisonType");
                    DrawDropdownForPropertyAndType(variableNameChoiceRect, stringKey, Condition.DialogueVariableType.String);
                    stringComparisonType.enumValueFlag = (int)(Condition.StringComparisonType)EditorGUI.EnumPopup(
                        comparisonTypeRect,
                        (Condition.StringComparisonType)stringComparisonType.enumValueFlag
                        );
                    stringValue.stringValue = EditorGUI.TextField(valueToCompareRect, stringValue.stringValue);
                    break;
            }
        }

        private void DrawDropdownForPropertyAndType(Rect rect, SerializedProperty property, Condition.DialogueVariableType type)
        {
            // Get the names of the variables based on the type
            ConditionVariableNamesSO dialogueVariableNames = _dialogueVariablesNamesSO.objectReferenceValue as ConditionVariableNamesSO;
            string[] variableNames = dialogueVariableNames.GetVarNames(type);

            // If the variable names are not found, show a warning
            if (variableNames == null || variableNames.Length == 0)
            {
                EditorGUILayout.HelpBox($"No names were found for this type! Create one in the {dialogueVariableNames.name} object.", MessageType.Warning);
                return;
            }

            int selected = Array.IndexOf(variableNames, property.stringValue);
            selected = EditorGUI.Popup(rect, selected, variableNames);
            selected = Mathf.Clamp(selected, 0, variableNames.Length - 1);

            property.stringValue = variableNames[selected];
        }

        public static ConditionVariableNamesSO MakeNewVariableEnvironment()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Dialogue Variable Names", "DialogueVariableNames", "asset", "Save Dialogue Variable Names", "Assets/Resources");

            if (path.Length == 0)
            {
                Debug.LogWarning("No path was selected to save the DialogueVariableNamesSO.");
                return null;
            }

            // Create a new DialogueVariableNamesSO
            ConditionVariableNamesSO dialogueVariablesNamesSO = ScriptableObject.CreateInstance<ConditionVariableNamesSO>();
            dialogueVariablesNamesSO.name = "DialogueConditionVariableEnvironment";

            // Save it in the Resources folder
            AssetDatabase.CreateAsset(dialogueVariablesNamesSO, path);
            AssetDatabase.SaveAssets();

            return dialogueVariablesNamesSO;
        }
    }
}