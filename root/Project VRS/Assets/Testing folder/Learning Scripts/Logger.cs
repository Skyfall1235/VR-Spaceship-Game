using UnityEngine;
public class Logger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] bool _showLogs;

    public enum LogLevel
    {
        Error,   //only errors
        Warning,  //only warnings
        Info   //only standard log info
    }

    public void Log(string message)
    {
        Object sender = null;
        Log(message, LogLevel.Info, sender);
    }
    public void Log(string message, Object sender) { Log(message, LogLevel.Info, sender); }
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
    
    public static string ColorText(string text, Color color)
    {
        string output;
        output = $"<color={ToHex(color)}>{text}</color>";
        return output;
    }
    public static string ToHex(Color c)
    {
        return string.Format($"#{ToByte(c.r)}{ToByte(c.g)}{ToByte(c.b)}");
    }
    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }

    
}




