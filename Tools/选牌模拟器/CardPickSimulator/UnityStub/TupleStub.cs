// ============================================================
// 战斗模拟器 · UnityEngine 桩库 —— ValueTuple 桩
// .NET Framework 4.6.2 无 System.ValueTuple，而游戏 Chess.cs 使用了
// C# 7 元组语法（List<(Chess chess, float distance)>、var (a,b) = ...）。
// 手写最小 System.ValueTuple<T1,T2> 与 TupleElementNamesAttribute 满足编译器。
// ============================================================
using System.Collections.Generic;

namespace System
{
    /// <summary>
    /// 二元值元组桩（对齐真实 System.ValueTuple 的字段与解构语义）
    /// </summary>
    public struct ValueTuple<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;

        public ValueTuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public void Deconstruct(out T1 item1, out T2 item2)
        {
            item1 = Item1;
            item2 = Item2;
        }
    }
}

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// 元组元素名 attribute（编译器为命名元组字面量生成，桩实现保留名字信息）
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property |
                    AttributeTargets.ReturnValue | AttributeTargets.Class | AttributeTargets.Struct |
                    AttributeTargets.Event, AllowMultiple = false)]
    public sealed class TupleElementNamesAttribute : Attribute
    {
        public TupleElementNamesAttribute(string[] transformNames)
        {
            TransformNames = transformNames;
        }

        public string[] TransformNames { get; }
    }
}
