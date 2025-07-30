using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;
    public int gridCellSize = 3; // 每个格子的实际大小(米)

    private Dictionary<int, List<Vector2Int>> occupiedGrids = new Dictionary<int, List<Vector2Int>>(); // 所有被占据的格子，键为chess.id

    void Start()
    {
        Instance = this;
    }

    // 世界坐标转格子坐标
    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / gridCellSize);
        int z = Mathf.FloorToInt(worldPosition.z / gridCellSize);
        return new Vector2Int(x, z);
    }

    // 尝试锁定目标位置的格子
    public bool TryLockGridPositions(Chess unit, Vector3 targetPosition)
    {
        // 获取单位包围盒
        Collider collider = unit.GetComponent<Collider>();
        if (collider == null)
        {
            UnityEngine.Debug.LogError("Unit missing collider for grid calculation");
            return false;
        }

        // 使用GetOccupiedGrids方法获取需要锁定的格子列表
        List<Vector2Int> requiredGrids = GetOccupiedGrids(targetPosition, collider);

        // 检查所有格子是否可用
        foreach (var gridPos in requiredGrids)
        {
            foreach (var entry in occupiedGrids)
            {
                if (entry.Key != unit.id && entry.Value.Contains(gridPos))
                {
                    UnityEngine.Debug.Log("Grid " + gridPos + " is already occupied by unit: " + entry.Key);
                    return false; // 格子不可用
                }
            }
        }

        ReleaseGridPositions(unit);
        // 锁定新格子
        List<Vector2Int> unitGrids = new List<Vector2Int>();
        foreach (var gridPos in requiredGrids)
        {
            unitGrids.Add(gridPos);
            UnityEngine.Debug.Log("Lock " + gridPos + " for unit: " + unit.id);
        }

        // 存储单位占据的格子
        occupiedGrids[unit.id] = unitGrids;

        return true;
    }

    // 获取指定位置和碰撞体占据的所有格子
    public List<Vector2Int> GetOccupiedGrids(Vector3 position, Collider collider)
    {
        List<Vector2Int> occupiedGrids = new List<Vector2Int>();
        
        if (collider == null)
        {
            UnityEngine.Debug.LogError("Collider is null for grid calculation");
            return occupiedGrids;
        }

        // 获取碰撞体边界
        Vector3 boundsSize = collider.bounds.size;
        Vector3 halfBounds = boundsSize / 2f;

        // 计算边界的最小和最大世界坐标
        Vector3 minWorldPos = position - halfBounds;
        Vector3 maxWorldPos = position + halfBounds;

        // 将世界坐标转换为格子坐标
        Vector2Int minGridPos = WorldToGridPosition(minWorldPos);
        Vector2Int maxGridPos = WorldToGridPosition(maxWorldPos);

        // 遍历从最小到最大格子坐标的所有格子
        for (int x = minGridPos.x; x <= maxGridPos.x; x++)
        {
            for (int z = minGridPos.y; z <= maxGridPos.y; z++)
            {
                Vector2Int currentGrid = new Vector2Int(x*gridCellSize, z*gridCellSize);
                occupiedGrids.Add(currentGrid);
            }
        }
        
        return occupiedGrids;
    }

    // 释放指定位置的格子
    // 释放指定单位占据的格子
    public void ReleaseGridPositions(Chess unit)
    {
        if (unit == null)
        {
            UnityEngine.Debug.LogError("Unit is null for grid release");
            return;
        }

        // 检查单位是否有占据的格子
        if (occupiedGrids.ContainsKey(unit.id))
        {
            occupiedGrids[unit.id].Clear();
            UnityEngine.Debug.Log("Released all grids for unit: " + unit.id);
        }
    }

    // 管理器销毁时释放所有格子
    private void OnDestroy()
    {
        occupiedGrids.Clear();
    }
}
