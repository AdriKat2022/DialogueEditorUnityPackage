using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace AdriKat.DialogueSystem.Utility
{
    public static class DialogueStyleUtility
    {
        public static readonly IReadOnlyDictionary<string, string> STYLE_GUIDS = new Dictionary<string, string>()
        {
            { "DialogueToolBarStyles", "56e71c5ed8a693f4293ce0ee78db317d" },
            { "DialogueVariables", "aba5c975eccbb86409b93f173fbcf6ab" },
            { "DialogueGraphViewStyles", "5c98439f5b29b8b41bd4e0e844c7387c" },
            { "DialogueNodeStyles", "e8be37637e95f6347a19881e9f06499c" }
        };

        public static VisualElement AddStyleSheets(this VisualElement element, params string[] styleSheetNames)
        {
            foreach (string styleSheetName in styleSheetNames)
            {
                if (!STYLE_GUIDS.TryGetValue(styleSheetName, out string guid))
                {
                    Debug.LogError($"Failed to find style sheet GUID: {styleSheetName}");
                    continue;
                }

                string path = AssetDatabase.GUIDToAssetPath(guid);
                StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
                if (styleSheet == null)
                {
                    Debug.LogError($"Failed to load style sheet: {styleSheetName}");
                    continue;
                }
                element.styleSheets.Add(styleSheet);
            }

            return element;
        }

        public static VisualElement AddClasses(this VisualElement element, params string[] classNames)
        {
            foreach (string className in classNames)
            {
                element.AddToClassList(className);
            }

            return element;
        }
    }
}