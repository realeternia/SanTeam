using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public enum LogLevel
{
    Debug = 0,
    Info = 1,
    Warn = 2,
    Error = 3
}

public class GameLog
{
    private static GameLog _instance;
    private static readonly object _lock = new object();

    private string _logDirectory;
    private string _currentDate;
    private LogLevel _minLogLevel = LogLevel.Debug;
    private readonly Dictionary<string, StreamWriter> _tagWriters = new Dictionary<string, StreamWriter>();
    private StreamWriter _mainWriter;
    private readonly object _fileLock = new object();

    public static GameLog Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new GameLog();
                    }
                }
            }
            return _instance;
        }
    }

    private GameLog()
    {
        Initialize();
    }

    private void Initialize()
    {
        _logDirectory = Path.Combine(UnityEngine.Application.persistentDataPath, "Logs");
        if (!Directory.Exists(_logDirectory))
        {
            Directory.CreateDirectory(_logDirectory);
        }
        CheckDateRotation();
        UnityEngine.Application.logMessageReceived += HandleUnityLog;
    }

    private void HandleUnityLog(string condition, string stackTrace, UnityEngine.LogType type)
    {
        if (type == UnityEngine.LogType.Exception)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string formattedMessage = $"[{timestamp}][EXCEPTION] {condition}\n{stackTrace}";
            WriteToFile(formattedMessage, "Exception");
        }
    }

    private void CheckDateRotation()
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        if (_currentDate != today)
        {
            lock (_fileLock)
            {
                if (_currentDate != today)
                {
                    CloseAllWriters();
                    _currentDate = today;
                    string mainLogFile = Path.Combine(_logDirectory, $"game_{today}.txt");
                    _mainWriter = new StreamWriter(mainLogFile, true, Encoding.UTF8);
                    _mainWriter.AutoFlush = true;
                }
            }
        }
    }

    private void CloseAllWriters()
    {
        if (_mainWriter != null)
        {
            _mainWriter.Close();
            _mainWriter = null;
        }
        foreach (var writer in _tagWriters.Values)
        {
            writer.Close();
        }
        _tagWriters.Clear();
    }

    public static void SetMinLogLevel(LogLevel level)
    {
        Instance._minLogLevel = level;
    }

    public static TaggedLogger SetTag(string tag)
    {
        return new TaggedLogger(tag);
    }

    public static void Debug(object message)
    {
        Instance.Log(LogLevel.Debug, null, message);
    }

    public static void Info(object message)
    {
        Instance.Log(LogLevel.Info, null, message);
    }

    public static void Warn(object message)
    {
        Instance.Log(LogLevel.Warn, null, message);
    }

    public static void Error(object message)
    {
        Instance.Log(LogLevel.Error, null, message);
    }

    internal void Log(LogLevel level, string tag, object message)
    {
        // if (level < _minLogLevel)
        // {
        //     return;
        // }

        CheckDateRotation();

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string levelStr = level.ToString().ToUpper();
        string tagStr = string.IsNullOrEmpty(tag) ? "" : $"[{tag}]";
        string formattedMessage = $"[{timestamp}][{levelStr}]{tagStr} {message}";

        switch (level)
        {
            case LogLevel.Debug:
            case LogLevel.Info:
                UnityEngine.Debug.Log(formattedMessage);
                break;
            case LogLevel.Warn:
                UnityEngine.Debug.LogWarning(formattedMessage);
                break;
            case LogLevel.Error:
                UnityEngine.Debug.LogError(formattedMessage);
                break;
        }

        WriteToFile(formattedMessage, tag);
    }

    private void WriteToFile(string message, string tag)
    {
        lock (_fileLock)
        {
            try
            {
                if (_mainWriter != null)
                {
                    _mainWriter.WriteLine(message);
                }

                if (!string.IsNullOrEmpty(tag))
                {
                    StreamWriter tagWriter = GetTagWriter(tag);
                    if (tagWriter != null)
                    {
                        tagWriter.WriteLine(message);
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"GameLog write file error: {e.Message}");
            }
        }
    }

    private StreamWriter GetTagWriter(string tag)
    {
        string tagKey = $"{_currentDate}_{tag}";
        if (!_tagWriters.ContainsKey(tagKey))
        {
            string tagFile = Path.Combine(_logDirectory, $"game_{_currentDate}.{tag.ToLower()}.txt");
            var writer = new StreamWriter(tagFile, true, Encoding.UTF8);
            writer.AutoFlush = true;
            _tagWriters[tagKey] = writer;
        }
        return _tagWriters[tagKey];
    }

    public static void Shutdown()
    {
        UnityEngine.Application.logMessageReceived -= Instance.HandleUnityLog;
        Instance.CloseAllWriters();
    }
}

public class TaggedLogger
{
    private readonly string _tag;

    public TaggedLogger(string tag)
    {
        _tag = tag;
    }

    public void Debug(object message)
    {
        GameLog.Instance.Log(LogLevel.Debug, _tag, message);
    }

    public void Info(object message)
    {
        GameLog.Instance.Log(LogLevel.Info, _tag, message);
    }

    public void Warn(object message)
    {
        GameLog.Instance.Log(LogLevel.Warn, _tag, message);
    }

    public void Error(object message)
    {
        GameLog.Instance.Log(LogLevel.Error, _tag, message);
    }
}
