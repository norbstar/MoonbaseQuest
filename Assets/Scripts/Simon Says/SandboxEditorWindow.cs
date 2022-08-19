#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;

using UnityEditor;

// https://www.youtube.com/watch?v=yxg_kjc2xuc&ab_channel=JasonWeimann

public class SandboxEditorWindow : EditorWindow
{
    [MenuItem("Editor Window/Sandbox")]
    private static void OpenWindow()
    {
        var window = GetWindow<SandboxEditorWindow>();
        window.trackedResources = Extensions.GetAllInstances().ToList();
    }

    public static class Extensions
    {
        public static ResourceType[] GetAllInstances() => new ResourceType[] { ResourceType.Food, ResourceType.Water };
    }

    public enum ResourceType
    {
        Food,
        Oxygen,
        Water
    }
    
    public List<ResourceType> trackedResources;   

    void OnGUI()
    {
        // TODO
    }
}
#endif