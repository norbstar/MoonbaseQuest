using UnityEngine;

public class ProjectSettings : CachedObject<ProjectSettings>
{
    protected override void Awake()
    {
        base.Awake();
        
        #if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
        #else
            Debug.unityLogger.logEnabled = false;
        #endif
    }
}