using System;
using System.Collections.Generic;

using UnityEngine;

public class LogSettings : MonoBehaviour
{
    [Serializable]
    public class UnitLoggerSettings
    {
        public UnitLogger logger;
        public bool logEnabled = false;
    }

    [SerializeField] List<UnitLoggerSettings> catalogue;

    // Start is called before the first frame update
    void Start()
    {
        foreach (UnitLoggerSettings settings in catalogue)
        {
            settings.logger.logEnabled = settings.logEnabled;
        }
    }
}