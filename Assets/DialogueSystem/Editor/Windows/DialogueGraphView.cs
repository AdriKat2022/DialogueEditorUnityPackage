using AdriKat.DialogueSystem.Data;
using AdriKat.DialogueSystem.Elements;
using AdriKat.DialogueSystem.Enumerations;
using AdriKat.DialogueSystem.Utility;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace AdriKat.DialogueSystem.Graph
{
    public class DialogueGraphView : GraphView
    {
        private readonly DialogueEditorWindow _editorWindow;

        private readonly SerializableDictionary<string, DialogueNodeErrorData> _ungroupedNodes;
        private readonly SerializableDictionary<string, DialogueGroupErrorData> _groups;
        private readonly SerializableDictionary<Group, SerializableDictionary<string, DialogueNodeErrorData>> _groupedNodes;

        private int _nameErrorAmount = 0;
        public int NameErrorAmount
        {
            get => _nameErrorAmount;

            set
            {
                _nameErrorAmount = value;

                if (_nameErrorAmount == 0)
                {
                    _editorWindow.EnableSaving();
                }
                else
                {
                    _editorWindow.DisableSaving();
                }
            }
        }

        public DialogueGraphView(DialogueEditorWindow editorWindow)
        {
            _editorWindow = editorWindow;
            _ungroupedNodes = new SerializableDictionary<string, DialogueNodeErrorData>();
            _groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, DialogueNodeErrorData>>();
            _groups = new SerializableDictionary<string, DialogueGroupErrorData>();

            AddManipulators();
            AddGridBackground();

            OnElementsDeleted();
            OnGroupElementsAdded();
            OnGroupElementsRemoved();
            OnGroupRenamed();
            OnGraphViewChanged();

            AddStyles();
        }

        #region Overrided Methods
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new();
            ports.ForEach((port) =>
            {
                // Check if the port is not the same as the start port and if the port is not on the same node as the start port
                if (startPort != port && startPort.node != port.node)
                {
                    compatiblePorts.Add(port);
                }
            });
            return compatiblePorts;
        }
        #endregion

        #region Manipulators
        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(CreateNodeContextualMenu("Add Single Choice Node", DialogueType.SingleChoice));
            this.AddManipulator(CreateNodeContextualMenu("Add Multiple Choice Node", DialogueType.MultipleChoice));
            this.AddManipulator(CreateNodeContextualMenu("Add Conditional Branch Node", DialogueType.ConditionalBranch));
            this.AddManipulator(CreateGroupContextualMenu());
        }
        #endregion

        #region Nodes
        private IManipulator CreateNodeContextualMenu(string actionTitle, DialogueType dialogueType)
        {
            ContextualMenuManipulator contextualMenu = new ContextualMenuManipulator((evt) =>
                {
                    evt.menu.AppendAction(actionTitle, (e) => AddElement(CreateNode($"New{dialogueType}", dialogueType, e.eventInfo.localMousePosition)));
                });
            return contextualMenu;
        }

        public DialogueNode CreateNode(string nodeName, DialogueType dialogueType, Vector2 position, bool shouldDraw = true)
        {
            Type nodeType = Type.GetType($"AdriKat.DialogueSystem.Elements.Dialogue{dialogueType}Node");
            DialogueNode node = (DialogueNode)Activator.CreateInstance(nodeType);
            node.Initialize(nodeName, this, position);

            if (shouldDraw)
            {
                node.Draw();
            }

            AddUngroupedNode(node);

            return node;
        }

        public void AddGroupedNode(DialogueNode node, DialogueGroup group)
        {
            string nodeName = node.DialogueName.ToLower();
            node.Group = group;

            if (!_groupedNodes.ContainsKey(group))
            {
                _groupedNodes.Add(group, new SerializableDictionary<string, DialogueNodeErrorData>());
            }

            if (!_groupedNodes[group].ContainsKey(nodeName))
            {
                DialogueNodeErrorData nodeErrorData = new();
                nodeErrorData.Nodes.Add(node);
                _groupedNodes[group].Add(nodeName, nodeErrorData);
                node.ResetStyle();
                return;
            }

            // If the name already exists, add the node to the list of nodes with the same name (there will be a duplicate)
            NameErrorAmount++;

            List<DialogueNode> groupedNodesList = _groupedNodes[group][nodeName].Nodes;
            groupedNodesList.Add(node);
            Color errorColor = _groupedNodes[group][nodeName].ErrorData.Color;
            node.SetErrorStyle(errorColor);
            if (groupedNodesList.Count > 1)
            {
                foreach (DialogueNode n in _groupedNodes[group][nodeName].Nodes)
                {
                    n.SetErrorStyle(errorColor);
                }
            }
            else
            {
                node.ResetStyle();
            }
        }

        public void RemoveGroupedNode(DialogueNode node, Group group)
        {
            string nodeName = node.DialogueName.ToLower();
            node.Group = null;
            List<DialogueNode> groupedNodesList = _groupedNodes[group][nodeName].Nodes;
            groupedNodesList.Remove(node);

            node.ResetStyle();

            if (groupedNodesList.Count == 0)
            {
                // Node was not in an error state (no other nodes with the same name)
                _groupedNodes[group].Remove(nodeName);

                // Remove the group if there are no nodes in it
                if (_groupedNodes[group].Count == 0)
                {
                    _groupedNodes.Remove(group);
                }
            }
            else
            {
                // The node was in an error state, we need to decrement the repeated names amount
                NameErrorAmount--;

                if (groupedNodesList.Count == 1)
                {
                    // Reset the style of the remaining node if there is only one node with the same name
                    groupedNodesList[0].ResetStyle();
                }
            }
        }

        public void AddUngroupedNode(DialogueNode node)
        {
            string nodeName = node.DialogueName.ToLower();

            if (!_ungroupedNodes.ContainsKey(nodeName))
            {
                // If the name does not exist, create a new list with the node
                DialogueNodeErrorData errorData = new DialogueNodeErrorData();
                errorData.Nodes.Add(node);
                _ungroupedNodes.Add(nodeName, errorData);
                node.ResetStyle();
                return;
            }

            // If the name already exists, add the node to the list of nodes with the same name
            // This is used to show the error message when there are nodes with the same name
            NameErrorAmount++;

            List<DialogueNode> ungroupedNodesList = _ungroupedNodes[nodeName].Nodes;
            ungroupedNodesList.Add(node);
            node.SetErrorStyle(_ungroupedNodes[nodeName].ErrorData.Color);
            if (_ungroupedNodes[nodeName].Nodes.Count > 1)
            {
                foreach (DialogueNode n in _ungroupedNodes[nodeName].Nodes)
                {
                    n.SetErrorStyle(_ungroupedNodes[nodeName].ErrorData.Color);
                }
            }
            else
            {
                node.ResetStyle();
            }
        }

        public void RemoveUngroupedNode(DialogueNode node)
        {
            string nodeName = node.DialogueName.ToLower();

            if (!_ungroupedNodes.ContainsKey(nodeName))
            {
                Debug.LogError("Tried to remove a non-existing node.");
                return;
            }

            List<DialogueNode> ungroupedNodesList = _ungroupedNodes[nodeName].Nodes;
            ungroupedNodesList.Remove(node);
            if (ungroupedNodesList.Count == 0)
            {
                _ungroupedNodes.Remove(nodeName);
            }
            else
            {
                // The node was in an error state, we need to decrement the repeated names amount
                NameErrorAmount--;
                if (ungroupedNodesList.Count == 1)
                {
                    ungroupedNodesList[0].ResetStyle();
                }
            }
        }

        #endregion

        #region Groups
        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenu = new ContextualMenuManipulator((evt) =>
                {
                    evt.menu.AppendAction("Add Group", (e) => CreateGroup("DialogueGroup", e.eventInfo.localMousePosition));
                });
            return contextualMenu;
        }

        public DialogueGroup CreateGroup(string title, Vector2 position)
        {
            DialogueGroup group = new DialogueGroup(title, position);
            AddGroup(group);
            AddElement(group);

            foreach (GraphElement graphElement in selection)
            {
                if (graphElement is DialogueNode node)
                {
                    group.AddElement(node);
                }
            }

            return group;
        }

        private void AddGroup(DialogueGroup group)
        {
            string groupName = group.title.ToLower();
            if (!_groups.ContainsKey(groupName))
            {
                DialogueGroupErrorData groupErrorData = new();
                groupErrorData.Groups.Add(group);
                _groups.Add(groupName, groupErrorData);
                return;
            }

            // The name already exists
            NameErrorAmount++;

            List<DialogueGroup> groupList = _groups[groupName].Groups;
            groupList.Add(group);
            Color errorColor = _groups[groupName].ErrorData.Color;
            group.SetErrorStyle(errorColor);

            if (groupList.Count > 1)
            {
                foreach (DialogueGroup g in groupList)
                {
                    g.SetErrorStyle(errorColor);
                }
            }
            else
            {
                group.ResetStyle();
            }
        }

        private void RemoveGroup(DialogueGroup group)
        {
            string oldGroupName = group.OldTitle.ToLower();
            List<DialogueGroup> groupsList = _groups[oldGroupName].Groups;
            groupsList.Remove(group);
            group.ResetStyle();

            if (groupsList.Count > 0)
            {
                NameErrorAmount--;
            }

            if (groupsList.Count == 1)
            {
                groupsList[0].ResetStyle();
                return;
            }

            if (groupsList.Count == 0)
            {
                _groups.Remove(oldGroupName);
            }
        }
        #endregion

        #region Callbacks

        private void OnElementsDeleted()
        {
            deleteSelection = (operationName, askUser) =>
            {
                Type groupType = typeof(DialogueGroup);
                Type edgeType = typeof(Edge);

                List<DialogueNode> nodesToDelete = new();
                List<DialogueGroup> groupsToDelete = new();
                List<Edge> edgesToDelete = new();

                foreach (ISelectable selectedElement in selection)
                {
                    if (selectedElement is DialogueNode node)
                    {
                        nodesToDelete.Add(node);
                        continue;
                    }

                    if (selectedElement.GetType() == edgeType)
                    {
                        Edge edge = (Edge)selectedElement;
                        edgesToDelete.Add(edge);
                        continue;
                    }

                    if (selectedElement.GetType() == groupType)
                    {
                        DialogueGroup group = (DialogueGroup)selectedElement;
                        groupsToDelete.Add(group);
                    }
                }

                foreach (DialogueGroup group in groupsToDelete)
                {
                    List<DialogueNode> groupNodes = new();

                    foreach (GraphElement element in group.containedElements)
                    {
                        if (element is DialogueNode node)
                        {
                            groupNodes.Add(node);
                        }
                    }

                    group.RemoveElements(groupNodes);
                    RemoveGroup(group);
                    RemoveElement(group);
                }

                DeleteElements(edgesToDelete);

                foreach (DialogueNode node in nodesToDelete)
                {
                    // Remove the node from the group if it is in one
                    node.Group?.RemoveElement(node);

                    RemoveUngroupedNode(node);
                    node.DisconnectAllPorts();
                    RemoveElement(node);
                }
            };
        }

        private void OnGroupElementsAdded()
        {
            elementsAddedToGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (element is not DialogueNode)
                    {
                        continue;
                    }

                    DialogueGroup nodeGroup = (DialogueGroup)group;
                    DialogueNode node = (DialogueNode)element;

                    RemoveUngroupedNode(node);
                    AddGroupedNode(node, nodeGroup);
                }
            };
        }

        private void OnGroupElementsRemoved()
        {
            elementsRemovedFromGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (element is not DialogueNode)
                    {
                        continue;
                    }

                    DialogueNode node = (DialogueNode)element;
                    RemoveGroupedNode(node, group);
                    AddUngroupedNode(node);
                }
            };
        }

        private void OnGroupRenamed()
        {
            groupTitleChanged = (group, newTitle) =>
            {
                DialogueGroup dialogueGroup = (DialogueGroup)group;
                dialogueGroup.title = newTitle.RemoveWhitespaces().RemoveSpecialCharacters();

                if (string.IsNullOrEmpty(dialogueGroup.title))
                {
                    if (!string.IsNullOrEmpty(dialogueGroup.OldTitle))
                    {
                        NameErrorAmount++;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(dialogueGroup.OldTitle))
                    {
                        NameErrorAmount--;
                    }
                }

                RemoveGroup(dialogueGroup);
                dialogueGroup.OldTitle = dialogueGroup.title;
                AddGroup(dialogueGroup);
            };
        }

        private void OnGraphViewChanged()
        {
            graphViewChanged = (changes) =>
            {
                if (changes.edgesToCreate != null)
                {
                    foreach (Edge edge in changes.edgesToCreate)
                    {
                        DialogueNode outputNode = edge.output.node as DialogueNode;
                        DialogueNode inputNode = edge.input.node as DialogueNode;

                        if (outputNode is DialogueConditionalBranchNode conditionalBranchNode)
                        {
                            // Check if the edge is coming from the TRUE or FALSE port
                            if (edge.output.portName == "True")
                            {
                                conditionalBranchNode.NodeOnTrue = inputNode.ID;
                                //Debug.Log($"Connected TRUE branch of {conditionalBranchNode.ID} to {inputNode.ID}");
                            }
                            else if (edge.output.portName == "False")
                            {
                                conditionalBranchNode.NodeOnFalse = inputNode.ID;
                                //Debug.Log($"Connected FALSE branch of {conditionalBranchNode.ID} to {inputNode.ID}");
                            }
                        }
                        else if (edge.output.userData is DialogueChoiceSaveData choiceSaveData)
                        {
                            choiceSaveData.NodeID = inputNode.ID;
                        }
                        else
                        {
                            edge.output.userData = inputNode.ID;
                        }

                        edge.input.Connect(edge);
                    }
                }

                if (changes.elementsToRemove != null)
                {
                    Type edgeType = typeof(Edge);

                    foreach (GraphElement element in changes.elementsToRemove)
                    {
                        if (element is Edge edge)
                        {
                            DialogueNode outputNode = edge.output.node as DialogueNode;
                            DialogueNode inputNode = edge.input.node as DialogueNode;

                            if (outputNode is DialogueConditionalBranchNode conditionalBranchNode)
                            {
                                if (edge.output.portName == "True" && conditionalBranchNode.NodeOnTrue == inputNode.ID)
                                {
                                    conditionalBranchNode.NodeOnTrue = null;
                                    //Debug.Log($"Disconnected TRUE branch of {conditionalBranchNode.ID} from {inputNode.ID}");
                                }
                                else if (edge.output.portName == "False" && conditionalBranchNode.NodeOnFalse == inputNode.ID)
                                {
                                    conditionalBranchNode.NodeOnFalse = null;
                                    //Debug.Log($"Disconnected FALSE branch of {conditionalBranchNode.ID} from {inputNode.ID}");
                                }
                            }
                            else if (edge.output.userData is DialogueChoiceSaveData choiceData)
                            {
                                choiceData.NodeID = "";
                            }
                            else if (edge.output.userData is string)
                            {
                                edge.output.userData = "";
                            }

                            edge.input.Disconnect(edge);
                        }
                    }
                }

                return changes;
            };
        }

        #endregion

        #region Styles and Background
        private void AddStyles()
        {
            this.AddStyleSheets("DialogueGraphViewStyles", "DialogueNodeStyles");
        }

        private void AddGridBackground()
        {
            GridBackground grid = new GridBackground();
            grid.StretchToParentSize();
            Insert(0, grid);
        }
        #endregion

        #region Utility Methods

        public void ClearGraph()
        {
            graphElements.ForEach((element) => RemoveElement(element));

            _groups.Clear();
            _groupedNodes.Clear();
            _ungroupedNodes.Clear();

            NameErrorAmount = 0;
        }

        internal void Repaint()
        {
            _editorWindow.Repaint();
        }
        #endregion
    }
}