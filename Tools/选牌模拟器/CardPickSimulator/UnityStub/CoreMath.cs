// ============================================================
// 战斗模拟器 · UnityEngine 桩库 —— CoreMath
// 提供游戏战斗代码依赖的最小数学/颜色/四元数类型。
// 与真实 Unity 语义保持一致（仅覆盖战斗逻辑用到的成员）。
// ============================================================
using System;

namespace UnityEngine
{
    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2 zero { get { return new Vector2(0f, 0f); } }
        public static Vector2 one { get { return new Vector2(1f, 1f); } }

        public float magnitude { get { return (float)Math.Sqrt(x * x + y * y); } }
        public float sqrMagnitude { get { return x * x + y * y; } }

        public Vector2 normalized
        {
            get
            {
                float m = magnitude;
                if (m < 1e-6f)
                    return zero;
                return new Vector2(x / m, y / m);
            }
        }

        public static Vector2 operator +(Vector2 a, Vector2 b) { return new Vector2(a.x + b.x, a.y + b.y); }
        public static Vector2 operator -(Vector2 a, Vector2 b) { return new Vector2(a.x - b.x, a.y - b.y); }
        public static Vector2 operator *(Vector2 a, float d) { return new Vector2(a.x * d, a.y * d); }
        public static Vector2 operator *(float d, Vector2 a) { return new Vector2(a.x * d, a.y * d); }
        public static Vector2 operator /(Vector2 a, float d) { return new Vector2(a.x / d, a.y / d); }
        public static bool operator ==(Vector2 a, Vector2 b) { return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y); }
        public static bool operator !=(Vector2 a, Vector2 b) { return !(a == b); }

        public override bool Equals(object obj) { return obj is Vector2 && this == (Vector2)obj; }
        public override int GetHashCode() { return x.GetHashCode() ^ (y.GetHashCode() << 2); }
        public override string ToString() { return string.Format("({0:F2}, {1:F2})", x, y); }
    }

    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0f;
        }

        public static Vector3 zero { get { return new Vector3(0f, 0f, 0f); } }
        public static Vector3 one { get { return new Vector3(1f, 1f, 1f); } }
        public static Vector3 up { get { return new Vector3(0f, 1f, 0f); } }
        public static Vector3 forward { get { return new Vector3(0f, 0f, 1f); } }
        public static Vector3 right { get { return new Vector3(1f, 0f, 0f); } }

        public float magnitude { get { return (float)Math.Sqrt(x * x + y * y + z * z); } }
        public float sqrMagnitude { get { return x * x + y * y + z * z; } }

        public Vector3 normalized
        {
            get
            {
                float m = magnitude;
                if (m < 1e-6f)
                    return zero;
                return new Vector3(x / m, y / m, z / m);
            }
        }

        public static Vector3 operator +(Vector3 a, Vector3 b) { return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z); }
        public static Vector3 operator -(Vector3 a, Vector3 b) { return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z); }
        public static Vector3 operator -(Vector3 a) { return new Vector3(-a.x, -a.y, -a.z); }
        public static Vector3 operator *(Vector3 a, float d) { return new Vector3(a.x * d, a.y * d, a.z * d); }
        public static Vector3 operator *(float d, Vector3 a) { return new Vector3(a.x * d, a.y * d, a.z * d); }
        public static Vector3 operator /(Vector3 a, float d) { return new Vector3(a.x / d, a.y / d, a.z / d); }
        public static bool operator ==(Vector3 a, Vector3 b) { return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z); }
        public static bool operator !=(Vector3 a, Vector3 b) { return !(a == b); }

        public override bool Equals(object obj) { return obj is Vector3 && this == (Vector3)obj; }
        public override int GetHashCode() { return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2); }
        public override string ToString() { return string.Format("({0:F2}, {1:F2}, {2:F2})", x, y, z); }

        public static float Dot(Vector3 a, Vector3 b) { return a.x * b.x + a.y * b.y + a.z * b.z; }
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            return new Vector3(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }

        public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal)
        {
            float d = Dot(vector, planeNormal);
            return vector - planeNormal * d;
        }

        public static float Distance(Vector3 a, Vector3 b) { return (a - b).magnitude; }

        public static float Angle(Vector3 from, Vector3 to)
        {
            float dot = Mathf.Clamp(Dot(from.normalized, to.normalized), -1f, 1f);
            return (float)Math.Acos(dot) * Mathf.Rad2Deg;
        }
    }

    public struct Vector4
    {
        public float x, y, z, w;
        public Vector4(float x, float y, float z, float w) { this.x = x; this.y = y; this.z = z; this.w = w; }
        public static bool operator ==(Vector4 a, Vector4 b) { return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w; }
        public static bool operator !=(Vector4 a, Vector4 b) { return !(a == b); }
        public override bool Equals(object obj) { return obj is Vector4 && this == (Vector4)obj; }
        public override int GetHashCode() { return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() << 4) ^ (w.GetHashCode() << 6); }
    }

    // 格子坐标（战斗距离计算用）：Distance = 曼哈顿距离
    public struct Vector2Int
    {
        public int x;
        public int y;

        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2Int zero { get { return new Vector2Int(0, 0); } }

        public static Vector2Int operator +(Vector2Int a, Vector2Int b) { return new Vector2Int(a.x + b.x, a.y + b.y); }
        public static Vector2Int operator -(Vector2Int a, Vector2Int b) { return new Vector2Int(a.x - b.x, a.y - b.y); }
        public static bool operator ==(Vector2Int a, Vector2Int b) { return a.x == b.x && a.y == b.y; }
        public static bool operator !=(Vector2Int a, Vector2Int b) { return !(a == b); }

        public static int Distance(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        public override bool Equals(object obj) { return obj is Vector2Int && this == (Vector2Int)obj; }
        public override int GetHashCode() { return (x * 397) ^ y; }
        public override string ToString() { return string.Format("({0}, {1})", x, y); }
    }

    public struct Color
    {
        public float r, g, b, a;

        public Color(float r, float g, float b, float a)
        {
            this.r = r; this.g = g; this.b = b; this.a = a;
        }

        public Color(float r, float g, float b)
        {
            this.r = r; this.g = g; this.b = b; this.a = 1f;
        }

        public static Color white { get { return new Color(1f, 1f, 1f, 1f); } }
        public static Color black { get { return new Color(0f, 0f, 0f, 1f); } }
        public static Color red { get { return new Color(1f, 0f, 0f, 1f); } }
        public static Color green { get { return new Color(0f, 1f, 0f, 1f); } }
        public static Color yellow { get { return new Color(1f, 0.92f, 0.016f, 1f); } }
        public static Color blue { get { return new Color(0f, 0f, 1f, 1f); } }
        public static Color gray { get { return new Color(0.5f, 0.5f, 0.5f, 1f); } }
        public static Color grey { get { return gray; } }
        public static Color clear { get { return new Color(0f, 0f, 0f, 0f); } }

        public static Color Lerp(Color a, Color b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Color(a.r + (b.r - a.r) * t, a.g + (b.g - a.g) * t, a.b + (b.b - a.b) * t, a.a + (b.a - a.a) * t);
        }

        public static bool operator ==(Color a, Color b)
        {
            return Mathf.Approximately(a.r, b.r) && Mathf.Approximately(a.g, b.g) && Mathf.Approximately(a.b, b.b) && Mathf.Approximately(a.a, b.a);
        }
        public static bool operator !=(Color a, Color b) { return !(a == b); }
        public override bool Equals(object obj) { return obj is Color && this == (Color)obj; }
        public override int GetHashCode() { return r.GetHashCode() ^ (g.GetHashCode() << 2) ^ (b.GetHashCode() << 4) ^ (a.GetHashCode() << 6); }
        public override string ToString() { return string.Format("RGBA({0:F2}, {1:F2}, {2:F2}, {3:F2})", r, g, b, a); }
    }

    public struct Color32
    {
        public byte r, g, b, a;
        public Color32(byte r, byte g, byte b, byte a) { this.r = r; this.g = g; this.b = b; this.a = a; }
        public static implicit operator Color32(Color c)
        {
            return new Color32((byte)(c.r * 255f), (byte)(c.g * 255f), (byte)(c.b * 255f), (byte)(c.a * 255f));
        }
    }

    public struct Quaternion
    {
        public float x, y, z, w;

        public Quaternion(float x, float y, float z, float w)
        {
            this.x = x; this.y = y; this.z = z; this.w = w;
        }

        public static Quaternion identity { get { return new Quaternion(0f, 0f, 0f, 1f); } }

        // 近似欧拉角四元数（仅用于绕 Y 轴 180° 旋转等战斗场景，足够近似）
        public static Quaternion Euler(float x, float y, float z)
        {
            float radX = x * Mathf.Deg2Rad * 0.5f;
            float radY = y * Mathf.Deg2Rad * 0.5f;
            float radZ = z * Mathf.Deg2Rad * 0.5f;
            float cx = (float)Math.Cos(radX), sx = (float)Math.Sin(radX);
            float cy = (float)Math.Cos(radY), sy = (float)Math.Sin(radY);
            float cz = (float)Math.Cos(radZ), sz = (float)Math.Sin(radZ);
            return new Quaternion(
                sx * cy * cz - cx * sy * sz,
                cx * sy * cz + sx * cy * sz,
                cx * cy * sz - sx * sy * cz,
                cx * cy * cz + sx * sy * sz);
        }

        // 朝向 forward 方向的四元数（Y 轴旋转近似）
        public static Quaternion LookRotation(Vector3 forward)
        {
            forward = forward.normalized;
            if (forward.sqrMagnitude < 1e-6f)
                return identity;
            float angle = (float)Math.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
            return Euler(0f, angle, 0f);
        }

        public static bool operator ==(Quaternion a, Quaternion b) { return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w; }
        public static bool operator !=(Quaternion a, Quaternion b) { return !(a == b); }

        // 四元数旋转向量（战斗场景用于朝向/位移计算，标准公式）
        public static Vector3 operator *(Quaternion q, Vector3 v)
        {
            var qv = new Vector3(q.x, q.y, q.z);
            var t = 2f * Vector3.Cross(qv, v);
            return v + q.w * t + Vector3.Cross(qv, t);
        }
        public override bool Equals(object obj) { return obj is Quaternion && this == (Quaternion)obj; }
        public override int GetHashCode() { return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() << 4) ^ (w.GetHashCode() << 6); }
        public override string ToString() { return string.Format("({0:F2}, {1:F2}, {2:F2}, {3:F2})", x, y, z, w); }
    }

    public struct Bounds
    {
        public Vector3 center;
        public Vector3 size;

        public Bounds(Vector3 center, Vector3 size)
        {
            this.center = center;
            this.size = size;
        }
    }

    public static class Mathf
    {
        public const float PI = 3.14159274f;
        public const float Deg2Rad = PI * 2f / 360f;
        public const float Rad2Deg = 360f / (PI * 2f);
        public const float Infinity = float.PositiveInfinity;

        public static float Abs(float f) { return Math.Abs(f); }
        public static int Abs(int value) { return Math.Abs(value); }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        public static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        public static float Clamp01(float value) { return Clamp(value, 0f, 1f); }

        public static float Max(float a, float b) { return a > b ? a : b; }
        public static float Max(params float[] values) { float m = values[0]; for (int i = 1; i < values.Length; i++) m = Max(m, values[i]); return m; }
        public static int Max(int a, int b) { return a > b ? a : b; }
        public static float Min(float a, float b) { return a < b ? a : b; }
        public static int Min(int a, int b) { return a < b ? a : b; }

        public static float Sqrt(float f) { return (float)Math.Sqrt(f); }
        public static float Pow(float f, float p) { return (float)Math.Pow(f, p); }
        public static float Sin(float f) { return (float)Math.Sin(f); }
        public static float Cos(float f) { return (float)Math.Cos(f); }

        public static float Lerp(float a, float b, float t) { return a + (b - a) * Clamp01(t); }
        public static float MoveTowards(float current, float target, float maxDelta)
        {
            if (Math.Abs(target - current) <= maxDelta) return target;
            return current + Math.Sign(target - current) * maxDelta;
        }

        public static bool Approximately(float a, float b)
        {
            return Math.Abs(b - a) < Math.Max(1e-6f, Math.Max(Math.Abs(a), Math.Abs(b)) * 1e-6f);
        }

        public static int FloorToInt(float f) { return (int)Math.Floor(f); }
        public static int CeilToInt(float f) { return (int)Math.Ceiling(f); }
        public static int RoundToInt(float f) { return (int)Math.Round(f); }
        public static float Floor(float f) { return (float)Math.Floor(f); }
        public static float Ceil(float f) { return (float)Math.Ceiling(f); }
    }

    public static class ColorUtility
    {
        // 支持 "#RRGGBB" 与 "#RRGGBBAA"
        public static bool TryParseHtmlString(string htmlString, out Color color)
        {
            color = Color.white;
            if (string.IsNullOrEmpty(htmlString))
                return false;
            string s = htmlString.Trim().TrimStart('#');
            if (s.Length != 6 && s.Length != 8)
                return false;
            try
            {
                int idx = 0;
                float r = ParseByte(s, ref idx);
                float g = ParseByte(s, ref idx);
                float b = ParseByte(s, ref idx);
                float a = s.Length == 8 ? ParseByte(s, ref idx) : 255f;
                color = new Color(r / 255f, g / 255f, b / 255f, a / 255f);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static float ParseByte(string s, ref int idx)
        {
            string hex = s.Substring(idx, 2);
            idx += 2;
            return (float)Convert.ToInt32(hex, 16);
        }

        public static string ToHtmlStringRGB(Color color)
        {
            int r = (int)Math.Round(Mathf.Clamp01(color.r) * 255f);
            int g = (int)Math.Round(Mathf.Clamp01(color.g) * 255f);
            int b = (int)Math.Round(Mathf.Clamp01(color.b) * 255f);
            return string.Format("{0:X2}{1:X2}{2:X2}", r, g, b);
        }
    }
}
