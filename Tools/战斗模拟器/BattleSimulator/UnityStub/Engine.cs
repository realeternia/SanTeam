// ============================================================
// 战斗模拟器 · UnityEngine 桩库 —— Engine
// Time/Debug/Application/Screen/Resources/JsonUtility
// ============================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace UnityEngine
{
    public enum LogType
    {
        Error = 0,
        Assert = 1,
        Warning = 2,
        Log = 3,
        Exception = 4
    }

    public static class Time
    {
        public static float time = 0f;
        public static float deltaTime = 0f;
        public static float realtimeSinceStartup = 0f;

        internal static void Advance(float dt)
        {
            time += dt;
            deltaTime = dt;
            realtimeSinceStartup = time;
        }

        public static void Reset()
        {
            time = 0f;
            deltaTime = 0f;
            realtimeSinceStartup = 0f;
        }
    }

    public static class Debug
    {
        public static void Log(object message)
        {
            Utf8Console.WriteLine("[Debug] " + message);
        }

        public static void LogWarning(object message)
        {
            Utf8Console.WriteLine("[Warn] " + message);
        }

        public static void LogError(object message)
        {
            Utf8Console.WriteLine("[Error] " + message);
        }
    }

    public static class Application
    {
        private static string _persistentDataPath;

        public static string persistentDataPath
        {
            get
            {
                if (_persistentDataPath == null)
                {
                    // 数据根目录 = 模拟器目录（BattleSimulator）；GameLog 在其下生成 Logs/ 战斗日志
                    _persistentDataPath = Path.GetFullPath(
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\.."));
                    if (!Directory.Exists(_persistentDataPath))
                        Directory.CreateDirectory(_persistentDataPath);
                }
                return _persistentDataPath;
            }
        }

        public static event Action<string, string, LogType> logMessageReceived;
    }

    public static class Resources
    {
        // 桩资源加载：Prefabs/Hud* → 带 ChessHUD 的哑对象；其余 Prefabs/ → 普通哑对象；
        // Texture/Sprite → null（贴图非关键）；Missile 等组件 → new 实例
        public static T Load<T>(string path) where T : Object
        {
            if (typeof(T) == typeof(GameObject))
            {
                GameObject go = new GameObject(path);
                if (path != null && path.StartsWith("Prefabs/Hud"))
                {
                    if (go.GetComponent(typeof(ChessHUD)) == null)
                        go.AddComponent(typeof(ChessHUD));
                }
                return (T)(object)go;
            }
            if (typeof(T) == typeof(Texture) || typeof(T) == typeof(Sprite))
            {
                return null;
            }
            if (typeof(Component).IsAssignableFrom(typeof(T)))
            {
                try
                {
                    // 组件实例必须挂到哑 GameObject 上（Missile 等用 transform/gameObject 驱动）
                    var comp = (Component)Activator.CreateInstance(typeof(T));
                    var go = new GameObject(typeof(T).Name);
                    comp._gameObject = go;
                    return (T)(object)comp;
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }
    }

    // 最小 JsonUtility：仅支持公开字段的简单对象/数组/基本类型（配置/存档类用）
    public static class JsonUtility
    {
        public static T FromJson<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
                return default(T);
            object obj = Activator.CreateInstance(typeof(T));
            var dict = ParseObject(json);
            Populate(obj, dict);
            return (T)obj;
        }

        public static string ToJson(object obj)
        {
            var sb = new System.Text.StringBuilder();
            WriteObject(sb, obj);
            return sb.ToString();
        }

        public static string ToJson(object obj, bool prettyPrint)
        {
            return ToJson(obj);
        }

        private static void Populate(object target, Dictionary<string, object> dict)
        {
            foreach (var field in target.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                object val;
                if (!dict.TryGetValue(field.Name, out val))
                    continue;
                try
                {
                    if (val == null) continue;
                    Type ft = field.FieldType;
                    if (ft.IsAssignableFrom(val.GetType()))
                        field.SetValue(target, val);
                    else if (ft == typeof(string))
                        field.SetValue(target, Convert.ToString(val));
                    else if (ft.IsEnum)
                        field.SetValue(target, Enum.ToObject(ft, Convert.ToInt32(val)));
                    else if (ft == typeof(int))
                        field.SetValue(target, Convert.ToInt32(val));
                    else if (ft == typeof(float))
                        field.SetValue(target, Convert.ToSingle(val));
                    else if (ft == typeof(double))
                        field.SetValue(target, Convert.ToDouble(val));
                    else if (ft == typeof(bool))
                        field.SetValue(target, Convert.ToBoolean(val));
                    else if (ft.IsArray)
                    {
                        var list = val as List<object>;
                        if (list != null)
                        {
                            var elemType = ft.GetElementType();
                            var arr = Array.CreateInstance(elemType, list.Count);
                            for (int i = 0; i < list.Count; i++)
                            {
                                if (elemType.IsAssignableFrom(list[i].GetType()))
                                    arr.SetValue(list[i], i);
                                else if (list[i] is Dictionary<string, object>)
                                {
                                    var elem = Activator.CreateInstance(elemType);
                                    Populate(elem, (Dictionary<string, object>)list[i]);
                                    arr.SetValue(elem, i);
                                }
                                else if (elemType == typeof(int))
                                    arr.SetValue(Convert.ToInt32(list[i]), i);
                            }
                            field.SetValue(target, arr);
                        }
                    }
                    else if (val is Dictionary<string, object>)
                    {
                        var elem = Activator.CreateInstance(ft);
                        Populate(elem, (Dictionary<string, object>)val);
                        field.SetValue(target, elem);
                    }
                }
                catch
                {
                    // 字段反序列化失败忽略
                }
            }
        }

        private static Dictionary<string, object> ParseObject(string json)
        {
            var tokenizer = new JsonTokenizer(json);
            return tokenizer.ReadObject();
        }

        private static void WriteObject(System.Text.StringBuilder sb, object obj)
        {
            sb.Append('{');
            bool first = true;
            foreach (var field in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!first) sb.Append(',');
                first = false;
                sb.Append('"').Append(field.Name).Append("\":");
                WriteValue(sb, field.GetValue(obj), field.FieldType);
            }
            sb.Append('}');
        }

        private static void WriteValue(System.Text.StringBuilder sb, object val, Type type)
        {
            if (val == null)
            {
                sb.Append("null");
                return;
            }
            if (type == typeof(string))
                sb.Append('"').Append(((string)val).Replace("\\", "\\\\").Replace("\"", "\\\"")).Append('"');
            else if (type == typeof(bool))
                sb.Append((bool)val ? "true" : "false");
            else if (type.IsArray)
            {
                sb.Append('[');
                var arr = (Array)val;
                for (int i = 0; i < arr.Length; i++)
                {
                    if (i > 0) sb.Append(',');
                    WriteValue(sb, arr.GetValue(i), arr.GetValue(i) != null ? arr.GetValue(i).GetType() : typeof(object));
                }
                sb.Append(']');
            }
            else if (type.IsClass)
            {
                WriteObject(sb, val);
            }
            else
            {
                sb.Append(val.ToString().ToLowerInvariant());
            }
        }

        private class JsonTokenizer
        {
            private readonly string s;
            private int i;

            public JsonTokenizer(string s)
            {
                this.s = s;
                i = 0;
            }

            public Dictionary<string, object> ReadObject()
            {
                var dict = new Dictionary<string, object>();
                SkipWs();
                if (Peek() != '{') return dict;
                i++;
                SkipWs();
                while (Peek() != '}' && i < s.Length)
                {
                    SkipWs();
                    string key = ReadString();
                    SkipWs();
                    if (Peek() == ':') i++;
                    SkipWs();
                    dict[key] = ReadValue();
                    SkipWs();
                    if (Peek() == ',') i++;
                }
                if (Peek() == '}') i++;
                return dict;
            }

            private object ReadValue()
            {
                SkipWs();
                char c = Peek();
                if (c == '{')
                    return ReadObject();
                if (c == '[')
                {
                    i++;
                    var list = new List<object>();
                    SkipWs();
                    while (Peek() != ']' && i < s.Length)
                    {
                        list.Add(ReadValue());
                        SkipWs();
                        if (Peek() == ',') i++;
                    }
                    if (Peek() == ']') i++;
                    return list;
                }
                if (c == '"')
                    return ReadString();
                // 数字/bool/null
                int start = i;
                while (i < s.Length && "0123456789.-eE".IndexOf(s[i]) >= 0) i++;
                string num = s.Substring(start, i - start);
                if (num == "true") return true;
                if (num == "false") return false;
                if (num == "null") return null;
                double d;
                if (double.TryParse(num, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out d))
                    return d;
                return num;
            }

            private string ReadString()
            {
                if (Peek() == '"') i++;
                var sb = new System.Text.StringBuilder();
                while (i < s.Length)
                {
                    char c = s[i++];
                    if (c == '"') break;
                    if (c == '\\' && i < s.Length)
                    {
                        char n = s[i++];
                        if (n == 'n') sb.Append('\n');
                        else if (n == 't') sb.Append('\t');
                        else sb.Append(n);
                    }
                    else
                        sb.Append(c);
                }
                return sb.ToString();
            }

            private void SkipWs()
            {
                while (i < s.Length && (s[i] == ' ' || s[i] == '\t' || s[i] == '\n' || s[i] == '\r'))
                    i++;
            }

            private char Peek()
            {
                return i < s.Length ? s[i] : '\0';
            }
        }
    }
}
