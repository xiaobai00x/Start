using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FindReferencesInParent : MonoBehaviour
{
    private static HashSet<string> whiteList = new HashSet<string>()
    {
        "Transform",
        "RectTransform",
        "UIWidget",
        "UIPanel",
        "UIScrollView",
        "UITable",
        "UIGrid",
        "UISprite",
        "RecyclePrefabPool"
    };

    [MenuItem("GameObject/Find Reference In Parent", false, 0)]
    static void FindReferenceInParent()
    {
        if (Selection.gameObjects.Length != 1)
        {
            Debug.LogWarning("<color=yellow>Please select one game object to find references in parent.</color>");
            return;
        }

        GameObject selectedObj = Selection.gameObjects[0];
        Component[] selectedComponents = selectedObj.GetComponents<Component>();
        Transform parent = selectedObj.transform.parent;
        while (parent != null)
        {
            Component[] components = parent.gameObject.GetComponents<Component>();
            foreach (Component component in components)
            {
                if (component == null) continue;
                if (whiteList.Contains(component.GetType().Name)) continue;
                SerializedObject so = new SerializedObject(component);
                SerializedProperty sp = so.GetIterator();
                while (sp.NextVisible(true))
                {
                    if (sp.propertyType != SerializedPropertyType.ObjectReference) continue;
                    if (sp.objectReferenceValue == null) continue;
                    if (sp.objectReferenceValue == selectedObj)
                    {
                        string message = string.Format("Reference found in <color=yellow>{0}</color> for <color=yellow>{1}</color> referencing <color=yellow>{2}</color> on <color=yellow>{3}</color> at field <color=yellow>{4}</color>",
                        parent.gameObject.name,
                        component.GetType().Name,
                        sp.objectReferenceValue.GetType().Name,
                        selectedObj.name,
                        sp.displayName);

                        Debug.LogError(message);
                        Selection.activeGameObject = parent.gameObject;
                        EditorGUIUtility.PingObject(component);
                        return;
                        Debug.LogError("Reference found in <color=yellow>" + parent.gameObject.name + "</color> for <color=yellow>" + component.GetType().Name + "</color> referencing <color=yellow>" + selectedObj.name +"     -->"+ sp.displayName + "</color>");
                        Selection.activeGameObject = parent.gameObject;
                        EditorGUIUtility.PingObject(component);
                        // EditorGUIUtility.ShowObjectPicker<Object>(component, false, "",
                        //     EditorGUIUtility.GetControlID(FocusType.Passive));
                        return;
                    }

                    foreach (Component selectedComponent in selectedComponents)
                    {
                        if (sp.objectReferenceValue == selectedComponent)
                        {
                            string message = string.Format("Reference found in <color=yellow>{0}</color> for <color=yellow>{1}</color> referencing <color=yellow>{2}</color> on <color=yellow>{3}</color> at field <color=yellow>{4}</color>",
                            parent.gameObject.name,
                            component.GetType().Name,
                            sp.objectReferenceValue.GetType().Name,
                            selectedObj.name,
                            sp.displayName);

                            Debug.LogError(message);
                            Selection.activeGameObject = parent.gameObject;
                            EditorGUIUtility.PingObject(component);
                            return;
                            Debug.LogError("Reference found in <color=yellow>" + parent.gameObject.name + "</color> for <color=yellow>" + component.GetType().Name + "</color> referencing <color=yellow>" + selectedComponent.GetType().Name + "</color> on <color=yellow>" + selectedObj.name +"     -->"+ sp.displayName + "</color>");
                            Selection.activeGameObject = parent.gameObject;
                            EditorGUIUtility.PingObject(component);
                            // EditorGUIUtility.ShowObjectPicker<Object>(component, false, "",
                            //     EditorGUIUtility.GetControlID(FocusType.Passive));
                            return;
                        }
                    }
                }
            }

            parent = parent.parent;
        }

        Debug.Log("No reference to <color=yellow>" + selectedObj.name + "</color> found in its parent objects.");
    }
}
