using AdriKat.DialogueSystem.Data;
using AdriKat.DialogueSystem.Enumerations;
using AdriKat.DialogueSystem.Graph;
using AdriKat.DialogueSystem.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AdriKat.DialogueSystem.Elements
{
    public class DialogueNode : Node
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string DialogueName { get; set; }
        [field: SerializeField] public string DialogueText { get; set; }
        [field: SerializeField] public List<DialogueChoiceSaveData> Choices { get; set; }
        [field: SerializeField] public bool HasAuthor { get; set; }
        [field: SerializeField] public DialogueAuthorData AuthorDecorator { get; set; }
        [field: SerializeField] public DialogueType Type { get; set; }
        [field: SerializeField] public DialogueGroup Group { get; set; }

        protected DialogueGraphView graphView;
        private Color defaultBackgroundColor;
        protected bool showDialogueTextContents = true;
        private bool isFirstDraw = true;

        public virtual void Initialize(string nodeName, DialogueGraphView graphView, Vector2 position)
        {
            this.graphView = graphView;
            ID = Guid.NewGuid().ToString();
            DialogueName = nodeName;
            Choices = new List<DialogueChoiceSaveData>();
            AuthorDecorator = new DialogueAuthorData();
            DialogueText = "DialogueText";

            defaultBackgroundColor = new Color(29 / 255f, 29 / 255f, 30 / 255f);

            SetPosition(new Rect(position, Vector2.zero));

            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");
        }

        #region Overrided methods
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconnect Input Ports", action => { DisconnectInputPorts(); });
            evt.menu.AppendAction("Disconnect Output Ports", action => { DisconnectOutputPorts(); });

            base.BuildContextualMenu(evt);
        }
        #endregion

        public virtual void Draw()
        {
            if (isFirstDraw)
            {
                isFirstDraw = false;
                Debug.Log("First draw");
                DrawInputPorts();
                DrawOutputPorts();
                DrawTitleContainer();
            }

            // Extensions container
            if (showDialogueTextContents)
            {
                DrawDialogueAuthor();
                DrawDialogueText();
            }
        }

        #region Draw Methods
        protected virtual void DrawInputPorts()
        {
            // Input container
            Port inputPort = this.CreatePort("Dialogue Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Dialogue Connection";
            inputContainer.Add(inputPort);
        }

        protected virtual void DrawOutputPorts()
        {

        }

        private void DrawTitleContainer()
        {
            TextField dialogueNameTextField = DialogueElementUtility.CreateTextField(DialogueName, null, evt =>
            {
                TextField target = (TextField)evt.target;
                target.value = evt.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

                // Keep track of error inducing names
                if (string.IsNullOrEmpty(target.value))
                {
                    if (!string.IsNullOrEmpty(DialogueName))
                    {
                        graphView.NameErrorAmount++;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(DialogueName))
                    {
                        graphView.NameErrorAmount--;
                    }
                }

                if (Group == null)
                {
                    graphView.RemoveUngroupedNode(this);
                    DialogueName = evt.newValue;
                    graphView.AddUngroupedNode(this);
                }
                else
                {
                    // Save the current group, cause it will be removed when the node is removed
                    DialogueGroup currentGroup = Group;
                    graphView.RemoveGroupedNode(this, Group);
                    DialogueName = evt.newValue;
                    graphView.AddGroupedNode(this, currentGroup);
                }
            });
            dialogueNameTextField.AddClasses(
                "ds-node__textfield",
                "ds-node__filename-textfield",
                "ds-node__textfield__hidden"
                );
            titleContainer.Insert(0, dialogueNameTextField);
        }

        private void DrawDialogueAuthor()
        {
            VisualElement customDataContainer = new();
            customDataContainer.AddToClassList("ds-node__custom-data-container");


            // Create a checkbox for using an author
            Toggle useAuthorToggle = new("Has an Author")
            {
                value = HasAuthor
            };
            useAuthorToggle.AddToClassList("ds-node__toggle");
            useAuthorToggle.RegisterValueChangedCallback(evt =>
            {
                HasAuthor = evt.newValue;
                RefreshUI();
            });
            customDataContainer.Add(useAuthorToggle);
            extensionContainer.Add(customDataContainer);

            // Only show the rest if we want to show the author
            if (!HasAuthor) return;

            // Make it collapsible
            Foldout authorFoldout = DialogueElementUtility.CreateFoldout("Author");
            authorFoldout.AddToClassList("ds-node__foldout");
            customDataContainer.Add(authorFoldout);

            // Fetch all authors from the specified folder
            string[] guids = AssetDatabase.FindAssets("t:DialogueAuthorSO", new[] { DialogueIOUtility.AUTHORS_FOLDER });

            if (guids.Length == 0)
            {
                authorFoldout.AddHelpBox("No authors found!\nCreate your first one by clicking the button below.", HelpBoxMessageType.Warning);

                // Create a button to create a new Author
                Button createAuthorButton = DialogueElementUtility.CreateButton("Create Author", () =>
                {
                    string path = DialogueIOUtility.AUTHORS_FOLDER;
                    DialogueAuthorSO newAuthor = DialogueIOUtility.CreateAsset<DialogueAuthorSO>(path, "DefaultAuthor");
                    newAuthor.Name = "DefaultAuthor";
                    DialogueIOUtility.SaveAsset(newAuthor);
                    AuthorDecorator.AuthorData = newAuthor;
                    EditorApplication.delayCall += () => RefreshUI();
                });

                createAuthorButton.AddToClassList("ds-node__button");
                authorFoldout.Add(createAuthorButton);

                return;
            }

            // Convert GUIDs to actual Author objects
            DialogueAuthorSO[] authors = guids
                .Select(guid => AssetDatabase.LoadAssetAtPath<DialogueAuthorSO>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(author => author != null)
                .ToArray();

            if (AuthorDecorator.AuthorData == null)
            {
                AuthorDecorator.AuthorData = authors[0];
            }

            // Extract names for the dropdown
            List<string> authorNames = authors.Select(a => a.name).ToList();

            // DropdownField for selecting an author
            DropdownField authorDropdown = new(authorNames, AuthorDecorator.AuthorData.name);
            authorDropdown.AddToClassList("ds-node__dropdown");
            authorFoldout.Add(authorDropdown);
            authorDropdown.RegisterValueChangedCallback(evt =>
            {
                string selectedAuthorName = evt.newValue;
                DialogueAuthorSO selectedAuthor = authors.First(a => a.name == selectedAuthorName);

                if (selectedAuthor == null)
                {
                    Debug.LogError("The selected author doesn't exist anymore!\nThis can happen if you changed the object's filename.");
                    return;
                }

                AuthorDecorator.AuthorData = selectedAuthor;
                RefreshUI();
            });

            Toggle showMugshotToggle = new("Show Mugshot")
            {
                value = AuthorDecorator.ShowMugshot
            };
            showMugshotToggle.AddToClassList("ds-node__toggle");
            showMugshotToggle.RegisterValueChangedCallback(evt =>
            {
                AuthorDecorator.ShowMugshot = evt.newValue;
                RefreshUI();
            });
            authorFoldout.Add(showMugshotToggle);

            if (!AuthorDecorator.ShowMugshot)
            {
                return;
            }

            // Make a dropdown for the available emotions for the selected author and draw the selected one
            Foldout authorEmotionFoldout = DialogueElementUtility.CreateFoldout("Mugshot Emotion");
            authorEmotionFoldout.AddToClassList("ds-node__foldout");
            authorFoldout.Add(authorEmotionFoldout);

            var authorEmotions = AuthorDecorator.AuthorData.Sprites.Keys;

            if (authorEmotions.Count == 0)
            {
                string authorPath = AssetDatabase.GetAssetPath(AuthorDecorator.AuthorData);
                authorEmotionFoldout.AddHelpBox($"No emotion found for this author.\nYou can add some in the Author ScriptableObject at\n\"{authorPath}\".", HelpBoxMessageType.Warning);
                return;
            }

            if (string.IsNullOrEmpty(AuthorDecorator.Emotion))
            {
                AuthorDecorator.Emotion = authorEmotions.First();
            }

            DropdownField authorEmotionDropdown = new(authorEmotions.ToList(), AuthorDecorator.Emotion);
            authorEmotionDropdown.AddToClassList("ds-node__dropdown");
            authorEmotionDropdown.RegisterValueChangedCallback(evt =>
            {
                AuthorDecorator.Emotion = evt.newValue;
                RefreshUI();
            });
            authorEmotionFoldout.Add(authorEmotionDropdown);

            // Draw the selected emotion
            if (AuthorDecorator.AuthorData.Sprites.TryGetValue(AuthorDecorator.Emotion, out Sprite authorEmotionSprite))
            {
                if (authorEmotionSprite == null)
                {
                    authorEmotionFoldout.AddHelpBox($"This emotion ({AuthorDecorator.Emotion}) doesn't have any sprite.\nMugshot will be disabled.", HelpBoxMessageType.Info);
                    return;
                }
                Image authorEmotionImage = new()
                {
                    image = authorEmotionSprite.texture
                };
                authorEmotionFoldout.Add(authorEmotionImage);
            }
            else
            {
                // Make warning helpbox
                authorEmotionFoldout.AddHelpBox($"This emotion ({AuthorDecorator.Emotion}) doesn't exist anymore!\nPlease select another one.", HelpBoxMessageType.Error);
            }
        }

        private void DrawDialogueText()
        {
            VisualElement customDataContainer = new();
            extensionContainer.Add(customDataContainer);
            customDataContainer.AddToClassList("ds-node__custom-data-container");

            Foldout textFoldout = DialogueElementUtility.CreateFoldout("Dialogue Text");
            customDataContainer.Add(textFoldout);

            TextField textTextField = DialogueElementUtility.CreateTextArea(DialogueText, null, callback =>
            {
                DialogueText = callback.newValue;
            });

            textTextField.AddClasses(
                "ds-node__textfield",
                "ds-node__quote-textfield"
                );
            textFoldout.Add(textTextField);
        }

        private void RefreshUI()
        {
            extensionContainer.Clear();

            Draw();

            MarkDirtyRepaint();
        }
        #endregion

        #region Utility Methods

        public void DisconnectAllPorts()
        {
            DisconnectInputPorts();
            DisconnectOutputPorts();
        }

        public void DisconnectPorts(VisualElement container)
        {
            foreach (Port port in container.Children())
            {
                if (!port.connected)
                {
                    continue;
                }

                graphView.DeleteElements(port.connections);
            }
        }

        public void DisconnectInputPorts()
        {
            DisconnectPorts(inputContainer);
        }

        public void DisconnectOutputPorts()
        {
            DisconnectPorts(outputContainer);
        }

        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }

        public bool IsStartingNode()
        {
            Port inputPort = inputContainer.Children().First() as Port;

            return !inputPort.connected;
        }
        #endregion
    }
}