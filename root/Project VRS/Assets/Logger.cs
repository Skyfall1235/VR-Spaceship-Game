using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Logger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] bool _showLogs;

    public void Log(object message, LogLevel level, Object sender)
    {
        if(!_showLogs) { return; }

        switch(level)
        {
            case LogLevel.Error:
                Debug.LogError(message, sender);
                break;
            case LogLevel.Warning:
                Debug.LogWarning(message, sender);
                break;
            case LogLevel.Info:
                Debug.Log(message, sender);
                break;
        }
    }
    public enum LogLevel
    {
        Error,   //only errors
        Warning,  //only warnings
        Info   //only standard log info
    }
}




