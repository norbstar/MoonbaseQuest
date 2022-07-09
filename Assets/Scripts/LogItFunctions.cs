using UnityEngine;

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class LogItFunctions
{
    public enum LogSpecial
    {
        NEWLINE
    }

    public static string FloatFormat { get; } = "0.0000";

    private static string GetLastMessage(string context)
    {
        string lastMessage = null;

        if (File.Exists($"{context}.txt"))
        {
            IEnumerable<string> messages = File.ReadLines($"{context}.txt");

            int count = 0;

            using (IEnumerator<string> enumerator = messages.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    count++;
                }
            }

            if (count > 0)
            {
                lastMessage = File.ReadLines($"{context}.txt").Last();
            }
        }
        
        return lastMessage;
    }

    private static float? ExtractTimestamp(string message)
    {
        float? timestamp = null;

        if (message != null)
        {
            var match = Regex.Match(message, @"([-+]?[0-9]*\.?[0-9]+)");

            if (match.Success)
            {
                timestamp = Convert.ToSingle(match.Groups[1].Value);
            }
        }

        return timestamp;
    }

    public static void LogIt(string context, string message, Logging logging = null)
    {
        float? timestamp = null;
        bool logToFile = true;
        bool logToConsole = true;
        bool includeTimestamp = true;

        if (logging != null)
        {
            logToConsole = logging.logToConsole;
            logToFile = logging.logToFile;
            includeTimestamp = logging.includeTimestamp;
        }

        if (includeTimestamp)
        {
            timestamp = Time.time;
        }

        if ((logToConsole) && (Application.isEditor))
        {
            LogToConsole(timestamp, context, message);
        }

        if (logToFile)
        {
            LogToFile(timestamp, context, message);
        }
    }

    public static void LogToFile(string context, string message)
    {
        LogToFile(Time.time, context, message);
    }

    private static void LogToFile(float? timestamp, string context, string message)
    {
        float? lastTimestamp = ExtractTimestamp(GetLastMessage(context));
        string path = Path.Combine(Application.persistentDataPath, $"{context}.log");

        // var debugCanvas = GameObject.Find("Debug Canvas");
        // var manager = debugCanvas.GetComponent<DebugCanvasUIManager>() as DebugCanvasUIManager;
        // manager.textUI.text = $"Writing to {path}";
        
        StreamWriter sw = new StreamWriter(path, true);
        
        if (timestamp.HasValue)
        {
            if (lastTimestamp != null)
            {
                sw.WriteLine($"{((float) timestamp).ToString(LogItFunctions.FloatFormat)} [{((float) timestamp - (float) lastTimestamp).ToString(LogItFunctions.FloatFormat)}] {message}");
            }
            else
            {
                sw.WriteLine($"{((float) timestamp).ToString(LogItFunctions.FloatFormat)} {message}");
            }
        }
        else
        {
            sw.WriteLine($"{message}");
        }

        sw.Flush();
        sw.Close();
    }

    public static void LogToConsole(string context, string message)
    {
        LogToConsole(Time.time, context, message);
    }

    private static void LogToConsole(float? timestamp, string context, string message)
    {
        if (timestamp.HasValue)
        {
            Debug.Log($"{((float) timestamp).ToString(LogItFunctions.FloatFormat)} {context} {message}");
        }
        else
        {
            Debug.Log($"{context} {message}");
        }
    }
}