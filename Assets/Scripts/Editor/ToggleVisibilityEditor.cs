using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ToggleVisibilityEditor
{
    static ToggleVisibilityEditor()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
    }

    static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        if (Event.current != null && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space)
        {
            if (selectionRect.Contains(Event.current.mousePosition))
            {
                GameObject[] selectedObjects = Selection.gameObjects;

                // 切换选中的游戏对象的可见性
                foreach (var obj in selectedObjects)
                {
                    obj.SetActive(!obj.activeSelf);
                }

                Event.current.Use();
            }
        }
    }
}
