using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
 
public static class ComponentTools
{
    [MenuItem("CONTEXT/Component/Collapse All")]
    public static void CollapseAll(MenuCommand command)
    {
        GameObject gameObject = (command.context as Component).gameObject;
        Component[] components = gameObject.GetComponents<Component>();
        
        foreach (Component component in components)
        {    
            InternalEditorUtility.SetIsInspectorExpanded(component, false);
        }

        ActiveEditorTracker.sharedTracker.ForceRebuild();
    }
 
    [MenuItem("CONTEXT/Component/Expand All")]
    public static void ExpandAll(MenuCommand command)
    {
        GameObject gameObject = (command.context as Component).gameObject;
        Component[] components = gameObject.GetComponents<Component>();
        
        foreach (Component component in components)
        {
            InternalEditorUtility.SetIsInspectorExpanded(component, true);
        }
        
        ActiveEditorTracker.sharedTracker.ForceRebuild();
    }
}