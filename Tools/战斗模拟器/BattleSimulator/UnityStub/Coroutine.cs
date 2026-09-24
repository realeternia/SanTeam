// ============================================================
// 战斗模拟器 · UnityEngine 桩库 —— Coroutine
// 最小协程运行时：按 Time.time 推进 IEnumerator。
// ============================================================
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine
{
    public class WaitForSeconds
    {
        public float seconds;

        public WaitForSeconds(float seconds)
        {
            this.seconds = seconds;
        }
    }

    public class Coroutine
    {
        internal IEnumerator routine;
        internal object owner;
        internal bool running = true;
        internal float resumeTime;
    }

    internal static class CoroutineRunner
    {
        private static readonly List<Coroutine> _routines = new List<Coroutine>();

        public static Coroutine StartCoroutine(Component owner, IEnumerator routine)
        {
            var coroutine = new Coroutine();
            coroutine.owner = owner;
            coroutine.routine = routine;
            coroutine.resumeTime = Time.time;
            _routines.Add(coroutine);
            return coroutine;
        }

        public static void StopCoroutine(Coroutine coroutine)
        {
            if (coroutine != null)
            {
                coroutine.running = false;
                _routines.Remove(coroutine);
            }
        }

        public static void StopCoroutine(Component owner, IEnumerator routine)
        {
            for (int i = _routines.Count - 1; i >= 0; i--)
            {
                if (_routines[i].owner == owner && _routines[i].routine == routine)
                {
                    _routines[i].running = false;
                    _routines.RemoveAt(i);
                }
            }
        }

        public static void StopAllFor(Component owner)
        {
            for (int i = _routines.Count - 1; i >= 0; i--)
            {
                if (_routines[i].owner == owner)
                {
                    _routines[i].running = false;
                    _routines.RemoveAt(i);
                }
            }
        }

        public static void RemoveAllFor(GameObject go)
        {
            for (int i = _routines.Count - 1; i >= 0; i--)
            {
                var comp = _routines[i].owner as Component;
                if (comp != null && comp.gameObject == go)
                {
                    _routines[i].running = false;
                    _routines.RemoveAt(i);
                }
            }
        }

        public static void Clear()
        {
            _routines.Clear();
        }

        // 按当前 Time.time 推进所有协程；Time.time 由 Sim 在调用前推进
        // 注意：MoveNext 内协程体可能调用 Destroy → RemoveAllFor 修改同一列表，
        // 因此必须用快照迭代 + 按引用移除，避免索引越界。
        public static void Step()
        {
            var snapshot = _routines.ToArray();
            foreach (var coroutine in snapshot)
            {
                if (!coroutine.running)
                {
                    _routines.Remove(coroutine);
                    continue;
                }
                if (coroutine.resumeTime > Time.time)
                    continue;

                bool finished = !coroutine.routine.MoveNext();
                if (finished)
                {
                    _routines.Remove(coroutine);
                    continue;
                }

                object yielded = coroutine.routine.Current;
                if (yielded is WaitForSeconds)
                {
                    coroutine.resumeTime = Time.time + ((WaitForSeconds)yielded).seconds;
                }
                else
                {
                    // null / 自定义 yield：下一帧立即继续
                    coroutine.resumeTime = Time.time;
                }
            }
        }

        public static int Count { get { return _routines.Count; } }
    }
}
