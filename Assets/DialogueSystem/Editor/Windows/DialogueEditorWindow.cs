using AdriKat.DialogueSystem.Graph;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AdriKat.DialogueSystem.Utility
{
    public class DialogueEditorWindow : EditorWindow
    {
        private static readonly string _defaultFilename = "DialogueFileName";
        private static TextField _filenameTextField;

        private DialogueGraphView _graphView;
        private Button _saveButton;


        [MenuItem("Window/Dialogue Editor Window")]
        public static void ShowExample()
        {
            GetWindow<DialogueEditorWindow>("Dialogue Editor Window");
        }

        private void OnEnable()
        {
            AddGraphView();
            AddToolBar();
            AddStyles();
        }

        public static void UpdateFileName(string filename)
        {
            _filenameTextField.value = filename;
        }

        private void AddToolBar()
        {
            Toolbar toolbar = new();
            _filenameTextField = DialogueElementUtility.CreateTextField(_defaultFilename, "File Name:", callback =>
            {
                _filenameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
            });

            _saveButton = DialogueElementUtility.CreateButton("Save", () => Save());
            Button loadButton = DialogueElementUtility.CreateButton("Load", () => Load());
            Button clearButton = DialogueElementUtility.CreateButton("Clear", () => Clear());

            toolbar.Add(_filenameTextField);
            toolbar.Add(_saveButton);
            toolbar.Add(loadButton);
            toolbar.Add(clearButton);

            toolbar.AddStyleSheets("DialogueToolBarStyles");
            rootVisualElement.Add(toolbar);
        }


        #region Toolbar Methods
        private void Save()
        {
            if (string.IsNullOrEmpty(_filenameTextField.value))
            {
                EditorUtility.DisplayDialog("Invalid file name", "Please enter a valid file name", "Ok");
                return;
            }

            // Check if the file name already exists
            if (DialogueIOUtility.GraphExists(_filenameTextField.value))
            {
                bool overwrite = EditorUtility.DisplayDialog("Overwriting Graph", $"The graph {_filenameTextField.value} already exists.\n" +
                    $"Do you want to overwrite it and update all its related dialogues?", "Overwrite", "Cancel");
                if (!overwrite)
                {
                    return;
                }
            }

            DialogueIOUtility.Initialize(_graphView, _filenameTextField.value);
            DialogueIOUtility.Save();
        }

        private void Load()
        {
            // If the graphs save path does not exist or is empty warn the user that there are no graphs to load

            if (!Directory.Exists(DialogueIOUtility.GRAPHS_SAVE_PATH))
            {
                EditorUtility.DisplayDialog("No Graphs Found", $"No graphs found to load. The folder {DialogueIOUtility.GRAPHS_SAVE_PATH} doesn't exist!\n" +
                    "Start by creating some nodes and then saving the graph.", "Ok");
                return;
            }

            if (Directory.GetFiles(DialogueIOUtility.GRAPHS_SAVE_PATH).Length == 0)
            {
                EditorUtility.DisplayDialog("No Graphs Found", $"No graphs found to load. The folder {DialogueIOUtility.GRAPHS_SAVE_PATH} is empty!\n" +
                    "Start by creating some nodes and then saving the graph.", "Ok");
                return;
            }

            string path = EditorUtility.OpenFilePanel("Dialogue Graphs", DialogueIOUtility.GRAPHS_SAVE_PATH, "asset");

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("Invalid file path");
                return;
            }

            Clear();
            DialogueIOUtility.Initialize(_graphView, Path.GetFileNameWithoutExtension(path));
            DialogueIOUtility.Load();
        }

        private void Clear()
        {
            _graphView.ClearGraph();
            _filenameTextField.value = _defaultFilename;
        }
        #endregion

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("DialogueVariables");
        }

        private void AddGraphView()
        {
            _graphView = new DialogueGraphView(this);
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        public void EnableSaving()
        {
            _saveButton.SetEnabled(true);
        }

        public void DisableSaving()
        {
            _saveButton.SetEnabled(false);
        }
    }
}