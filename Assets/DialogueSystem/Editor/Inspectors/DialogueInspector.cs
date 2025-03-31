using System.Collections.Generic;

namespace AdriKat.DialogueSystem.Inspector
{
    using AdriKat.DialogueSystem.Core;
    using AdriKat.DialogueSystem.Data;
    using AdriKat.DialogueSystem.Utility;
    using UnityEditor;

    [CustomEditor(typeof(Dialogue))]
    public class DialogueInspector : Editor
    {
        private SerializedProperty _dialogueContainerProperty;
        private SerializedProperty _dialogueGroupProperty;
        private SerializedProperty _dialogueProperty;

        private SerializedProperty _groupedDialogueProperty;
        private SerializedProperty _startingDialogueOnlyProperty;

        private SerializedProperty _selectedDialogueGroupIndexProperty;
        private SerializedProperty _selectedDialogueIndexProperty;


        private void OnEnable()
        {
            _dialogueContainerProperty = serializedObject.FindProperty("_dialogueContainer");
            _dialogueGroupProperty = serializedObject.FindProperty("_dialogueGroup");
            _dialogueProperty = serializedObject.FindProperty("_dialogue");
            _groupedDialogueProperty = serializedObject.FindProperty("_groupedDialogues");
            _startingDialogueOnlyProperty = serializedObject.FindProperty("_startingDialogueOnly");
            _selectedDialogueGroupIndexProperty = serializedObject.FindProperty("_selectedDialogueGroupIndex");
            _selectedDialogueIndexProperty = serializedObject.FindProperty("_selectedDialogueIndex");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDialogueContainerArea();
            DialogueInspectorUtility.DrawSpace();

            DialogueContainerSO dialogueContainer = _dialogueContainerProperty.objectReferenceValue as DialogueContainerSO;

            if (dialogueContainer == null)
            {
                StopDrawing("Start by assigning a dialogue container. A dialogue container is equivalent to a dialogue graph.");
                return;
            }

            DrawFiltersArea();
            DialogueInspectorUtility.DrawSpace();

            bool currentGroupedDialoguesFilter = _groupedDialogueProperty.boolValue;
            bool currentStartingDialoguesOnlyFilter = _startingDialogueOnlyProperty.boolValue;

            List<string> dialogueNames;
            string dialogueFolderPath = $"{DialogueIOUtility.DIALOGUES_SAVE_PATH}/{dialogueContainer.FileName}";
            string dialogueInfoMessage;

            if (currentGroupedDialoguesFilter)
            {
                List<string> dialogueGroupNames = dialogueContainer.GetDialogueGroupNames();

                if (dialogueGroupNames.Count == 0)
                {
                    StopDrawing("There are no dialogue groups found in the dialogue container.");
                    return;
                }

                DrawDialogueGroupArea(dialogueContainer, dialogueGroupNames);
                DialogueInspectorUtility.DrawSpace();

                DialogueGroupSO dialogueGroup = (DialogueGroupSO)_dialogueGroupProperty.objectReferenceValue;
                dialogueNames = dialogueContainer.GetGroupedDialogueNames(dialogueGroup, currentStartingDialoguesOnlyFilter);
                dialogueFolderPath += $"/{DialogueIOUtility.DIALOGUES_GROUPSPACE_FOLDER}/{dialogueGroup.GroupName}/Dialogues";
                dialogueInfoMessage = "There are no " + (currentStartingDialoguesOnlyFilter ? "starting" : "") + " dialogues in the selected group!";
            }
            else
            {
                dialogueNames = dialogueContainer.GetUngroupedDialogueNames(currentStartingDialoguesOnlyFilter);
                dialogueFolderPath += $"/{DialogueIOUtility.DIALOGUES_GLOBALSPACE_FOLDER}/Dialogues";
                dialogueInfoMessage = "There are no " + (currentStartingDialoguesOnlyFilter ? "starting " : "") + "dialogues in the global space of this container!";
            }

            if (dialogueNames.Count == 0)
            {
                StopDrawing(dialogueInfoMessage);
                return;
            }

            DrawDialogueArea(dialogueNames, dialogueFolderPath);

            serializedObject.ApplyModifiedProperties();
        }

        #region Draw Methods

        private void DrawDialogueContainerArea()
        {
            DialogueInspectorUtility.DrawHeader("Dialogue Container");
            _dialogueContainerProperty.DrawPropertyField();
        }

        private void DrawFiltersArea()
        {
            DialogueInspectorUtility.DrawHeader("Filters");
            _groupedDialogueProperty.DrawPropertyField();
            _startingDialogueOnlyProperty.DrawPropertyField();
        }

        private void DrawDialogueGroupArea(DialogueContainerSO dialogueContainer, List<string> dialogueGroupNames)
        {
            DialogueInspectorUtility.DrawHeader("Dialogue Group");

            int oldSelectedDialogueGroupIndex = _selectedDialogueGroupIndexProperty.intValue;

            DialogueGroupSO oldDialogueGroup = _dialogueGroupProperty.objectReferenceValue as DialogueGroupSO;

            string oldDialogueGroupName = oldDialogueGroup == null ? string.Empty : oldDialogueGroup.name;

            UpdateIndexOnDialogueGroupUpdate(
                dialogueGroupNames,
                _selectedDialogueGroupIndexProperty,
                oldSelectedDialogueGroupIndex,
                oldDialogueGroupName,
                oldDialogueGroup == null);

            _selectedDialogueGroupIndexProperty.DrawPopup("Dialogue Group", dialogueGroupNames.ToArray());
            string selectedDialogueGroupName = dialogueGroupNames[_selectedDialogueGroupIndexProperty.intValue];
            DialogueGroupSO selectedDialogueGroup = DialogueIOUtility.LoadAsset<DialogueGroupSO>($"{DialogueIOUtility.DIALOGUES_SAVE_PATH}/{dialogueContainer.FileName}/{DialogueIOUtility.DIALOGUES_GROUPSPACE_FOLDER}/{selectedDialogueGroupName}", selectedDialogueGroupName);
            _dialogueGroupProperty.objectReferenceValue = selectedDialogueGroup;

            DialogueInspectorUtility.DrawDisabledFields(() => _dialogueGroupProperty.DrawPropertyField());
        }

        private void DrawDialogueArea(List<string> dialogueNames, string dialogueFolderPath)
        {
            DialogueInspectorUtility.DrawHeader("Dialogue");
            int oldSelectedDialogueIndex = _selectedDialogueIndexProperty.intValue;
            DialogueSO oldDialogue = _dialogueProperty.objectReferenceValue as DialogueSO;
            string oldDialogueName = oldDialogue == null ? string.Empty : oldDialogue.name;
            UpdateIndexOnDialogueGroupUpdate(dialogueNames, _selectedDialogueIndexProperty, oldSelectedDialogueIndex, oldDialogueName, oldDialogue == null);
            _selectedDialogueIndexProperty.DrawPopup("Dialogue", dialogueNames.ToArray());
            string selectedDialogueName = dialogueNames[_selectedDialogueIndexProperty.intValue];
            DialogueSO selectedDialogue = DialogueIOUtility.LoadAsset<DialogueSO>(dialogueFolderPath, selectedDialogueName);
            _dialogueProperty.objectReferenceValue = selectedDialogue;
            DialogueInspectorUtility.DrawDisabledFields(() => _dialogueProperty.DrawPropertyField());
        }

        private void StopDrawing(string reason)
        {
            EditorGUILayout.HelpBox(reason, MessageType.Info, true);
            DialogueInspectorUtility.DrawSpace();
            EditorGUILayout.HelpBox("You need to select a dialogue for this component to work properly at runtime!", MessageType.Warning, true);
            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        private void UpdateIndexOnDialogueGroupUpdate(List<string> optionNames, SerializedProperty indexProperty, int oldSelectedPropertyIndex, string oldPropertyName, bool isOldPropertyNull)
        {
            if (isOldPropertyNull)
            {
                indexProperty.intValue = 0;
                return;
            }

            bool oldIndexIsOutOfBounds = oldSelectedPropertyIndex >= optionNames.Count;
            bool oldNameIsDifferentThanSelectedName = oldIndexIsOutOfBounds || oldPropertyName != optionNames[oldSelectedPropertyIndex];

            if (oldNameIsDifferentThanSelectedName)
            {
                if (optionNames.Contains(oldPropertyName))
                {
                    indexProperty.intValue = optionNames.IndexOf(oldPropertyName);
                }
                else
                {
                    indexProperty.intValue = 0;
                }
            }
        }
    }
}