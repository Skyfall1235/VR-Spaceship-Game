using System.IO;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Logger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] bool _showLogs;
    private byte _Hash = 0;

    public enum LogLevel
    {
        Error,   //only errors
        Warning,  //only warnings
        Info   //only standard log info
    }

    private void Awake()
    {
        _Hash = (byte)Mathf.FloorToInt(UnityEngine.Random.value * 256);
    }


    public void Log(string message)
    {
        UnityEngine.Object sender = null;
        Log(message, LogLevel.Info, sender);
    }
    public void Log(string message, LogLevel severity)
    {
        UnityEngine.Object sender = null;
        Log(message, severity, sender);
    }
    public void Log(string message, UnityEngine.Object sender) { Log(message, LogLevel.Info, sender); }
    public void Log(string message, LogLevel level, UnityEngine.Object sender)
    {
        if(!_showLogs) { return; }

        switch(level)
        {
            case LogLevel.Error:
                Debug.LogError(message, sender);
                LogErrorToFile(message, sender);
                break;
            case LogLevel.Warning:
                Debug.LogWarning(message, sender);
                LogErrorToFile(message, sender);
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

    public void LogErrorToFile(string message, UnityEngine.Object sender)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd");
        string filePath = Application.dataPath + $"/CustomLogs/error_log_{timestamp}_{_Hash}.txt";
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (!File.Exists(filePath))
        {
            // Create a new file if it doesn't exist
            using (StreamWriter writer = File.CreateText(filePath))
            {
                writer.WriteLine($"[{this.gameObject}]"); // Optional: Add a header for the new file
            }
        }

        // Now use File.AppendText to append to the existing or newly created file
        using (StreamWriter writer = File.AppendText(filePath))
        {
            writer.WriteLine($"[{timestamp}] Source: {sender}, Logger: {this.gameObject.name}, Scene: {currentSceneName}\n {message}");
            writer.Close();
        }

    }



}




