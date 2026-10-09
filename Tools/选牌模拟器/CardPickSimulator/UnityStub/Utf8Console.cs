// ============================================================
// 战斗模拟器 · UTF-8 控制台输出工具
// WinExe 子系统下 Console.OpenStandardOutput() 不可用（抛"文件无法使用"），
// Console.OutputEncoding / Console.SetOut 均无效（默认保持 GBK），
// 因此在 UTF-8 终端下中文会乱码。改为 P/Invoke 直接写 UTF-8 字节。
// 无 stdout 句柄（如 GUI 模式未重定向）时回退 System.Console。
// ============================================================
using System;
using System.Runtime.InteropServices;

public static class Utf8Console
{
    private static readonly System.Text.Encoding Utf8 = new System.Text.UTF8Encoding(false);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    private static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, IntPtr lpOverlapped);

    private const int StdOutputHandle = -11;

    public static void WriteLine(string text)
    {
        WriteBytes(Utf8.GetBytes(text + "\r\n"));
    }

    public static void Write(string text)
    {
        WriteBytes(Utf8.GetBytes(text));
    }

    private static void WriteBytes(byte[] bytes)
    {
        try
        {
            IntPtr handle = GetStdHandle(StdOutputHandle);
            if (handle != IntPtr.Zero && handle != new IntPtr(-1))
            {
                uint written;
                if (WriteFile(handle, bytes, (uint)bytes.Length, out written, IntPtr.Zero))
                    return;
            }
        }
        catch { }
        // 无句柄时回退（如 GUI 模式无重定向）
        try { System.Console.Write(new System.Text.UTF8Encoding(false).GetString(bytes)); } catch { }
    }
}
