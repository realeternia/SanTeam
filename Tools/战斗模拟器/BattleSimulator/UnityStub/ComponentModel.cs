// ============================================================
// 战斗模拟器 · UnityEngine 桩库 —— ComponentModel
// 组件模型：Object/GameObject/Transform/Component/MonoBehaviour/
// Renderer/Material/Collider/Camera/Canvas/RectTransform 等。
//
// 关键：组件工厂（AddComponent/GetComponent）创建实例后，通过反射
// 把类型为 Renderer/Material/Texture 的公开字段注入桩实例，
// 替代 Unity 预制体序列化字段（如 Chess.rend/rendFlag/material/materialFlag）。
// ============================================================
using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine
{
    // 活动对象注册表：跟踪战斗中的导弹（Missile 组件），供 GDI+ 主窗体绘制
    internal static class LiveRegistry
    {
        public static readonly List<Missile> Missiles = new List<Missile>();

        public static void NotifyCreated(Component comp)
        {
            if (comp is Missile m && !Missiles.Contains(m))
                Missiles.Add(m);
        }

        public static void NotifyDestroyed(GameObject go)
        {
            for (int i = Missiles.Count - 1; i >= 0; i--)
            {
                if (Missiles[i] == null || Missiles[i].gameObject == null || Missiles[i].gameObject == go)
                    Missiles.RemoveAt(i);
            }
        }

        public static void Clear()
        {
            Missiles.Clear();
        }
    }

    public class Object
    {
        public string name;

        public Object() { }

        // ---- Destroy ----
        public static void Destroy(Object obj) { DestroyInternal(obj); }
        public static void Destroy(Object obj, float t) { DestroyInternal(obj); }

        private static void DestroyInternal(Object obj)
        {
            var go = obj as GameObject;
            if (go != null)
            {
                CoroutineRunner.RemoveAllFor(go);
                go.SetDestroyed();
                return;
            }
            var comp = obj as Component;
            if (comp != null)
            {
                CoroutineRunner.RemoveAllFor(comp.gameObject);
                comp.gameObject.SetDestroyed();
            }
        }

        // ---- Instantiate ----
        public static GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation)
        {
            return Instantiate(original, position, rotation, null);
        }

        public static GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject go = new GameObject(original != null ? original.name : "Clone", position, rotation);
            if (parent != null)
                go.transform.SetParent(parent);
            if (original != null)
            {
                // 复制原对象上的所有组件（哑预制体上的 Chess/ChessHUD 等）
                foreach (var srcComp in original.GetAllComponents())
                {
                    var comp = go.AddComponent(srcComp.GetType());
                    CopyFields(srcComp, comp);
                }
            }
            return go;
        }

        public static GameObject Instantiate(GameObject original, Transform parent)
        {
            GameObject go = new GameObject(original != null ? original.name : "Clone", Vector3.zero, Quaternion.identity);
            if (parent != null)
                go.transform.SetParent(parent);
            if (original != null)
            {
                foreach (var srcComp in original.GetAllComponents())
                {
                    var comp = go.AddComponent(srcComp.GetType());
                    CopyFields(srcComp, comp);
                }
            }
            return go;
        }

        public static GameObject Instantiate(GameObject original)
        {
            return Instantiate(original, Vector3.zero, Quaternion.identity, null);
        }

        public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object
        {
            GameObject go = new GameObject(original != null ? original.name : "Clone", position, rotation);
            if (parent != null)
                go.transform.SetParent(parent);
            if (original is Component)
            {
                var comp = go.AddComponent(original.GetType());
                CopyFields(original, comp);
                return (T)(object)comp;
            }
            return (T)(object)go;
        }

        public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object
        {
            return Instantiate(original, position, rotation, null);
        }

        public static T Instantiate<T>(T original, Transform parent) where T : Object
        {
            return Instantiate(original, Vector3.zero, Quaternion.identity, parent);
        }

        public static T Instantiate<T>(T original) where T : Object
        {
            return Instantiate(original, Vector3.zero, Quaternion.identity, null);
        }

        // Unity Object.FindObjectOfType：桩实现返回类型默认实例（供 Chess.CreateHUD 的 FindObjectOfType<Canvas>()）
        public static T FindObjectOfType<T>() where T : Object
        {
            try
            {
                return (T)(object)Activator.CreateInstance(typeof(T));
            }
            catch
            {
                return null;
            }
        }

        internal static void CopyFields(object src, object dst)
        {
            if (src == null || dst == null)
                return;
            foreach (var field in src.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (field.IsInitOnly || field.IsStatic)
                    continue;
                if (field.Name == "_gameObject") // 桩内部宿主引用：新实例应指向自己的 GameObject
                    continue;
                try
                {
                    field.SetValue(dst, field.GetValue(src));
                }
                catch
                {
                    // 字段复制失败忽略（如只读/索引器）
                }
            }
        }

        public override string ToString()
        {
            return name ?? GetType().Name;
        }
    }

    public class Component : Object
    {
        internal GameObject _gameObject;

        public GameObject gameObject { get { return _gameObject; } }
        public Transform transform { get { return _gameObject != null ? _gameObject.transform : null; } }

        public T GetComponent<T>() where T : Component, new()
        {
            return _gameObject.GetComponent<T>();
        }

        public bool TryGetComponent<T>(out T component) where T : Component, new()
        {
            component = _gameObject.GetComponent<T>();
            return component != null;
        }

        public Component GetComponent(Type type)
        {
            return _gameObject.GetComponent(type);
        }
    }

    public class Behaviour : Component
    {
        public bool enabled = true;
        public bool isActiveAndEnabled { get { return enabled && _gameObject.activeSelf; } }
    }

    public class MonoBehaviour : Behaviour
    {
        public Coroutine StartCoroutine(System.Collections.IEnumerator routine)
        {
            return CoroutineRunner.StartCoroutine(this, routine);
        }

        public Coroutine StartCoroutine(string methodName)
        {
            return null;
        }

        public void StopCoroutine(Coroutine routine)
        {
            CoroutineRunner.StopCoroutine(routine);
        }

        public void StopCoroutine(System.Collections.IEnumerator routine)
        {
            CoroutineRunner.StopCoroutine(this, routine);
        }

        public void StopAllCoroutines()
        {
            CoroutineRunner.StopAllFor(this);
        }
    }

    // 组件工厂：创建组件实例并注入渲染字段
    internal static class ComponentFactory
    {
        private static readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();

        public static T Create<T>(GameObject go) where T : Component, new()
        {
            var comp = new T();
            comp._gameObject = go;
            InjectRenderFields(comp);
            LiveRegistry.NotifyCreated(comp);
            return comp;
        }

        public static Component Create(Type type, GameObject go)
        {
            var comp = (Component)Activator.CreateInstance(type);
            comp._gameObject = go;
            InjectRenderFields(comp);
            LiveRegistry.NotifyCreated(comp);
            return comp;
        }

        // 反射注入：Renderer/Material/Texture 类型的公开字段 → 桩实例（替代预制体序列化字段）
        private static void InjectRenderFields(object comp)
        {
            foreach (var field in comp.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (field.IsStatic || field.IsInitOnly)
                    continue;
                Type ft = field.FieldType;
                if (ft == typeof(Renderer))
                {
                    if (field.GetValue(comp) == null)
                        field.SetValue(comp, new Renderer());
                }
                else if (ft == typeof(Material))
                {
                    if (field.GetValue(comp) == null)
                        field.SetValue(comp, new Material(null));
                }
                else if (ft == typeof(Texture))
                {
                    if (field.GetValue(comp) == null)
                        field.SetValue(comp, new Texture());
                }
            }
        }
    }

    public class GameObject : Object
    {
        internal Transform _transform;
        internal List<Component> _components = new List<Component>();
        internal bool _destroyed;

        public GameObject()
        {
            _transform = ComponentFactory.Create<Transform>(this);
        }

        public GameObject(string name)
        {
            this.name = name;
            _transform = ComponentFactory.Create<Transform>(this);
        }

        public GameObject(string name, Vector3 position, Quaternion rotation)
        {
            this.name = name;
            _transform = ComponentFactory.Create<Transform>(this);
            _transform.position = position;
            _transform.rotation = rotation;
        }

        public Transform transform { get { return _transform; } }

        public bool activeSelf { get; private set; } = true;

        public void SetActive(bool value) { activeSelf = value; }

        internal void SetDestroyed()
        {
            _destroyed = true;
            LiveRegistry.NotifyDestroyed(this);
        }

        public T AddComponent<T>() where T : Component, new()
        {
            var comp = ComponentFactory.Create<T>(this);
            _components.Add(comp);
            return comp;
        }

        public Component AddComponent(Type type)
        {
            var comp = ComponentFactory.Create(type, this);
            _components.Add(comp);
            return comp;
        }

        public T GetComponent<T>() where T : Component, new()
        {
            foreach (var c in _components)
            {
                if (c is T)
                    return (T)c;
            }
            return null;
        }

        public Component GetComponent(Type type)
        {
            foreach (var c in _components)
            {
                if (type.IsAssignableFrom(c.GetType()))
                    return c;
            }
            return null;
        }

        public T GetComponentInChildren<T>() where T : Component, new()
        {
            return GetComponent<T>();
        }

        internal List<Component> GetAllComponents() { return _components; }

        public bool TryGetComponent<T>(out T component) where T : Component, new()
        {
            component = GetComponent<T>();
            return component != null;
        }

        public bool TryGetComponent(Type type, out Component component)
        {
            component = GetComponent(type);
            return component != null;
        }
    }

    public class Transform : Component
    {
        private Vector3 _position;
        private Quaternion _rotation = Quaternion.identity;
        private Vector3 _localScale = Vector3.one;
        private List<Transform> _children = new List<Transform>();
        private Transform _parent;

        public Vector3 position
        {
            get { return _parent != null ? _parent.position + _localPosition : _localPosition; }
            set { if (_parent != null) _localPosition = value - _parent.position; else _localPosition = value; }
        }

        private Vector3 _localPosition;
        public Vector3 localPosition { get { return _localPosition; } set { _localPosition = value; } }

        public Quaternion rotation { get { return _rotation; } set { _rotation = value; } }
        public Quaternion localRotation { get { return _rotation; } set { _rotation = value; } }

        public Vector3 localScale { get { return _localScale; } set { _localScale = value; } }

        public Transform parent
        {
            get { return _parent; }
            set { SetParent(value); }
        }

        public Vector3 forward
        {
            get { return QuaternionLookDir(_rotation); }
            set { _rotation = Quaternion.LookRotation(value); }
        }
        public Vector3 right { get { return QuaternionRightDir(_rotation); } }
        public Vector3 up { get { return Vector3.up; } }

        public int childCount { get { return _children.Count; } }

        private static Vector3 QuaternionLookDir(Quaternion q)
        {
            return new Vector3(2 * (q.x * q.z + q.w * q.y),
                               2 * (q.y * q.z - q.w * q.x),
                               1 - 2 * (q.x * q.x + q.y * q.y)).normalized;
        }

        private static Vector3 QuaternionRightDir(Quaternion q)
        {
            return new Vector3(1 - 2 * (q.y * q.y + q.z * q.z),
                               2 * (q.x * q.y + q.w * q.z),
                               2 * (q.x * q.z - q.w * q.y)).normalized;
        }

        public void SetParent(Transform p)
        {
            if (_parent != null)
                _parent._children.Remove(this);
            _parent = p;
            if (p != null)
                p._children.Add(this);
        }

        public Transform GetChild(int index)
        {
            return index >= 0 && index < _children.Count ? _children[index] : null;
        }

        public Transform Find(string name)
        {
            // 桩：找不到就返回一个虚拟子节点，供 GetComponent 拿哑组件
            var dummy = ComponentFactory.Create<Transform>(new GameObject("dummy"));
            return dummy;
        }

        public void Translate(Vector3 translation)
        {
            _localPosition += translation;
        }
    }

    public class RectTransform : Transform
    {
        private Vector2 _anchoredPosition;
        private Vector2 _sizeDelta = new Vector2(100f, 100f);

        public Vector2 anchoredPosition { get { return _anchoredPosition; } set { _anchoredPosition = value; } }
        public Vector2 sizeDelta { get { return _sizeDelta; } set { _sizeDelta = value; } }
    }

    public class Renderer : Component
    {
        private Material _sharedMaterial = new Material(null);

        public Material sharedMaterial { get { return _sharedMaterial; } set { _sharedMaterial = value; } }

        public Material material
        {
            get { return _sharedMaterial; }
            set { _sharedMaterial = value; }
        }
    }

    public class Material : Object
    {
        private Texture _mainTexture;

        public Material(Material source)
        {
            if (source != null)
                _mainTexture = source.mainTexture;
        }

        public Texture mainTexture
        {
            get { return _mainTexture; }
            set { _mainTexture = value; }
        }

        private readonly Dictionary<string, Color> _colors = new Dictionary<string, Color>();
        private readonly Dictionary<string, float> _floats = new Dictionary<string, float>();
        private readonly Dictionary<string, Texture> _textures = new Dictionary<string, Texture>();

        public void SetColor(string name, Color value) { _colors[name] = value; }
        public Color GetColor(string name) { return _colors.ContainsKey(name) ? _colors[name] : Color.white; }

        public void SetFloat(string name, float value) { _floats[name] = value; }
        public float GetFloat(string name) { return _floats.ContainsKey(name) ? _floats[name] : 0f; }

        public void SetTexture(string name, Texture value) { _textures[name] = value; }
        public Texture GetTexture(string name) { return _textures.ContainsKey(name) ? _textures[name] : null; }
    }

    public class Texture : Object
    {
    }

    public class Sprite : Object
    {
    }

    public class Collider : Component
    {
        private Bounds _bounds = new Bounds(Vector3.zero, new Vector3(6f, 6f, 6f));
        public Bounds bounds { get { return _bounds; } set { _bounds = value; } }
    }

    public class Camera : Behaviour
    {
        public static Camera main { get { return _main ?? (_main = new Camera()); } }
        private static Camera _main;

        public Vector3 WorldToScreenPoint(Vector3 position)
        {
            return new Vector3(position.x, position.y, position.z);
        }
    }

    public class Canvas : Behaviour
    {
    }

    public class Animator : Behaviour
    {
        public void Play(string stateName) { }
    }

    public class RectTransformUtility
    {
        public static bool ScreenPointToLocalPointInRectangle(RectTransform rect, Vector2 screenPoint, Camera cam, out Vector2 localPoint)
        {
            localPoint = Vector2.zero;
            return true;
        }
    }

    public static class Screen
    {
        public static int width { get { return 1920; } }
        public static int height { get { return 1080; } }
    }
}
