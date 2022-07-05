using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class LogItType
{
    public static LogItType Outline { get; } = new LogItType(1, "Outline");
    public static LogItType Verbose { get; } = new LogItType(2, "Verbose");

    public int Ordinal { get; private set; }
    public string Name { get; private set; }

    public LogItType(int ordinal, string name)
    {
        Ordinal = ordinal;
        Name = name;
    }

    public static IEnumerable<LogItType> List()
    {
        return new[] { Outline, Verbose };
    }

    public static LogItType FromString(string name)
    {
        return List().Single(lt => String.Equals(lt.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    public static LogItType FromIndex(int ordinal)
    {
        return List().Single(lt => lt.Ordinal == ordinal);
    }

    public bool Featured(LogItType logType)
    {
        return (logType.Ordinal <= Ordinal);
    }
}