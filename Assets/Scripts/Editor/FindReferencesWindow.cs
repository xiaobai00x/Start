using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FindReferencesWindow : EditorWindow
{
    private GameObject selectedObject;
    private List<ReferenceItem> references = new List<ReferenceItem>();
    private Vector2 scrollPosition;

    [MenuItem("GameObject/Find References In Scene", false, 0)]
    static void FindReferences()
    {
        // Open the window
        var window = GetWindow<FindReferencesWindow>("References");
        window.selectedObject = Selection.activeGameObject;
        window.FindReferencesInScene();
    }

    void OnGUI()
    {
        if (selectedObject == null)
        {
            EditorGUILayout.HelpBox("No object selected.", MessageType.Info);
            return;
        }

        EditorGUILayout.LabelField("Finding references for: ", selectedObject.name);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUIStyle highlightStyle = new GUIStyle(GUI.skin.button);
        highlightStyle.richText = true;
        foreach (var reference in references)
        {
            if (GUILayout.Button(reference.disPlayName + "-----" + reference.disPlayomponentName + "---->" + 
                                 reference.componentName + " in " + reference.gameObjectName, highlightStyle))
            {
                if (reference.component == null)
                {
                    continue;
                }
                // Ping the object in the hierarchy to highlight it
                EditorGUIUtility.PingObject(reference.component.gameObject);

                // Select the object to show it in the Inspector
                Selection.activeObject = reference.component;

                // Focus the Inspector window
                EditorWindow inspectorWindow = EditorWindow.GetWindow(typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow"));
                if (inspectorWindow != null)
                {
                    inspectorWindow.Focus();
                }
            }
        }

        EditorGUILayout.EndScrollView();
    }

    void FindReferencesInScene()
    {
        references.Clear();
        references.Add(new ReferenceItem
        {
            disPlayName = "在引用脚本中的名字",
            disPlayomponentName = "引用类型",
            componentName = "引用脚本",
            gameObjectName = "引用节点",
        });
        var allComponents = FindObjectsOfType<Component>();
        foreach (var component in allComponents)
        {
            if (component == null) continue;

            SerializedObject so = new SerializedObject(component);
            SerializedProperty sp = so.GetIterator();

            while (sp.NextVisible(true))
            {
                if (sp.propertyType != SerializedPropertyType.ObjectReference) continue;
                if (sp.objectReferenceValue == null) continue;

                if (sp.objectReferenceValue == selectedObject || sp.objectReferenceValue is Component comp && comp.gameObject == selectedObject)
                {
                    references.Add(new ReferenceItem
                    {
                        disPlayName = "<color=yellow>" + sp.displayName + "</color>",
                        disPlayomponentName = "<color=yellow>" + sp.objectReferenceValue.GetType().Name + "</color>",
                        componentName = "<color=yellow>" + component.GetType().Name + "</color>",
                        gameObjectName = component.gameObject.name,
                        component = component
                    });
                }
            }
        }

        Repaint();
    }

    struct ReferenceItem
    {
        public string componentName;
        public string disPlayName;
        public string disPlayomponentName;
        public string gameObjectName;
        public Component component;
    }
}
