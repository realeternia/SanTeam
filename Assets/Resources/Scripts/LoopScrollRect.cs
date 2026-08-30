using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 循环滚动列表：只实例化视口内可见的单元格（含缓冲区），回收不可见单元格到对象池，
/// 用于替代一次性实例化全部条目导致的卡顿。
/// </summary>
public class LoopScrollRect
{
    private readonly ScrollRect scrollRect;
    private GameObject itemPrefab;
    private float itemHeight;
    private int buffer = 3;

    private readonly List<object> dataSource = new List<object>();
    private readonly Dictionary<int, ILoopScrollItem> activeItems = new Dictionary<int, ILoopScrollItem>();
    private readonly Queue<ILoopScrollItem> itemPool = new Queue<ILoopScrollItem>();
    private Action<ILoopScrollItem> onItemCreated;
    private VerticalLayoutGroup cachedLayoutGroup;
    private bool cachedLayoutEnabled;
    private bool initialized;

    public bool IsInitialized { get { return initialized; } }

    public LoopScrollRect(ScrollRect scrollRect)
    {
        this.scrollRect = scrollRect;
    }

    public void Initialize(IList source, GameObject prefab, float cellHeight, Action<ILoopScrollItem> onItemCreated = null)
    {
        Clear();

        this.onItemCreated = onItemCreated;

        if (source != null)
        {
            foreach (var item in source)
            {
                dataSource.Add(item);
            }
        }
        this.itemPrefab = prefab;
        this.itemHeight = cellHeight;

        var content = scrollRect.content;
        cachedLayoutGroup = content.GetComponent<VerticalLayoutGroup>();
        if (cachedLayoutGroup != null)
        {
            cachedLayoutEnabled = cachedLayoutGroup.enabled;
            cachedLayoutGroup.enabled = false;
        }

        var rt = content as RectTransform;
        if (rt != null)
        {
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, itemHeight * dataSource.Count);
        }

        scrollRect.onValueChanged.AddListener(OnScroll);
        initialized = true;

        RefreshVisible();
    }

    public void Clear()
    {
        if (initialized)
        {
            scrollRect.onValueChanged.RemoveListener(OnScroll);
        }

        foreach (var kvp in activeItems)
        {
            if (kvp.Value is MonoBehaviour mb)
            {
                UnityEngine.Object.Destroy(mb.gameObject);
            }
        }
        activeItems.Clear();

        while (itemPool.Count > 0)
        {
            var item = itemPool.Dequeue();
            if (item is MonoBehaviour mb)
            {
                UnityEngine.Object.Destroy(mb.gameObject);
            }
        }

        if (cachedLayoutGroup != null)
        {
            cachedLayoutGroup.enabled = cachedLayoutEnabled;
            cachedLayoutGroup = null;
        }

        dataSource.Clear();
        initialized = false;
    }

    public int GetTotalCount()
    {
        return dataSource.Count;
    }

    public object GetData(int index)
    {
        if (index < 0 || index >= dataSource.Count)
        {
            GameLog.Warn($"LoopScrollRect.GetData: 索引越界 index={index} total={dataSource.Count}");
            return null;
        }
        return dataSource[index];
    }

    public void SortItems(Comparison<object> comparison)
    {
        if (!initialized || dataSource.Count == 0)
        {
            return;
        }

        dataSource.Sort(comparison);
        ForceRefresh();
    }

    public void ForceRefresh()
    {
        if (!initialized)
        {
            return;
        }

        foreach (var kvp in activeItems)
        {
            if (kvp.Value != null)
            {
                ReturnToPool(kvp.Value);
            }
        }
        activeItems.Clear();

        RefreshVisible();
    }

    public void OnScroll(Vector2 position)
    {
        RefreshVisible();
    }

    private void RefreshVisible()
    {
        if (!initialized || dataSource.Count == 0)
        {
            return;
        }

        var content = scrollRect.content;
        var viewport = scrollRect.viewport != null ? scrollRect.viewport : (scrollRect.transform as RectTransform);

        // Content 的 anchor/pivot 在左上角，向下滚动时 anchoredPosition.y 从 0 增大到正值
        // contentTop 表示视口顶部相对 Content 顶部的偏移（从 Content 顶部往下算）
        float contentTop = content.anchoredPosition.y;
        float viewportHeight = viewport.rect.height;

        int newFirst = Mathf.Max(0, Mathf.FloorToInt(contentTop / itemHeight) - buffer);
        int newLast = Mathf.Min(dataSource.Count - 1,
            Mathf.FloorToInt((contentTop + viewportHeight) / itemHeight) + buffer);

        var keysToRemove = new List<int>();
        foreach (var kvp in activeItems)
        {
            if (kvp.Key < newFirst || kvp.Key > newLast)
            {
                ReturnToPool(kvp.Value);
                keysToRemove.Add(kvp.Key);
            }
        }
        foreach (var k in keysToRemove)
        {
            activeItems.Remove(k);
        }

        for (int i = newFirst; i <= newLast; i++)
        {
            if (!activeItems.ContainsKey(i))
            {
                var cell = GetFromPool();
                PositionCell(cell, i);
                cell.BindData(dataSource[i]);
                activeItems[i] = cell;
            }
        }
    }

    private void PositionCell(ILoopScrollItem cell, int index)
    {
        var mb = cell as MonoBehaviour;
        if (mb == null) return;
        var rt = mb.transform as RectTransform;
        if (rt != null)
        {
            rt.anchoredPosition = new Vector2(0, -index * itemHeight);
        }
    }

    private ILoopScrollItem GetFromPool()
    {
        if (itemPool.Count > 0)
        {
            var pooledCell = itemPool.Dequeue();
            var mb = pooledCell as MonoBehaviour;
            if (mb != null)
            {
                mb.gameObject.SetActive(true);
            }
            return pooledCell;
        }

        var obj = UnityEngine.Object.Instantiate(itemPrefab, scrollRect.content);
        obj.transform.localScale = Vector3.one;
        var rt = obj.transform as RectTransform;
        if (rt != null)
        {
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
        }
        var cell = obj.GetComponent<ILoopScrollItem>();
        if (cell != null && onItemCreated != null)
        {
            onItemCreated(cell);
        }
        return cell;
    }

    private void ReturnToPool(ILoopScrollItem cell)
    {
        if (cell == null) return;
        cell.OnReturnToPool();
        var mb = cell as MonoBehaviour;
        if (mb != null)
        {
            mb.gameObject.SetActive(false);
        }
        itemPool.Enqueue(cell);
    }
}
