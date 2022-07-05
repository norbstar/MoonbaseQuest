using System;

[Serializable]
public class Logging
{
    public bool featureEnabled = false;
    public bool logToConsole = false;
    public bool logToFile = false;
    public bool logVerbose = false;
    public bool includeTimestamp = true;

    public LogItType GetLogType()
    {
        LogItType logItType = LogItType.Outline;

        if (logVerbose)
        {
            logItType = LogItType.Verbose;
        }

        return logItType;
    }
}