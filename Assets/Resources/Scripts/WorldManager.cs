using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;
    public GameObject Units;
    public int gridCellSize = 3; // 每个格子的实际大小(米)

    private Dictionary<int, List<Vector2Int>> occupiedGrids = new Dictionary<int, List<Vector2Int>>(); // 所有被占据的格子，键为chess.id

    private bool showDebugCube = false;
    private Dictionary<Vector2Int, GameObject> debugGridCubes = new Dictionary<Vector2Int, GameObject>(); // 格子与调试cube的映射

    public GameObject[] RegionSide1; // 阵营1的出生点数组
    public GameObject[] RegionSide2; // 阵营2的出生点数组

    
    public GameObject[] RegionHeroSide1; // 阵营1的出生点数组
    public GameObject[] RegionHeroSide2; // 阵营2的出生点数组

    public HeroInfoGroup heroInfoGroup;
    public Button buttonRestart;
    public TMP_Text textRestart;


    void Start()
    {
        Instance = this;

        HeroConfig.Load();
        buttonRestart.onClick.AddListener(RestartGame);

        buttonRestart.gameObject.SetActive(false);
        textRestart.gameObject.SetActive(false);
        SpawnUnitsInRegions();
    }

    private void RestartGame()
    {     
        buttonRestart.gameObject.SetActive(false);
        textRestart.gameObject.SetActive(false);
        SpawnUnitsInRegions();
    }

    private void SpawnUnitsInRegions()
    {
        // 清空之前的单位
        foreach (Transform child in Units.transform)
        {
            Destroy(child.gameObject);
        }
        occupiedGrids.Clear();
        heroInfoGroup.Reset();

        // 加载UnitBing预制体
        GameObject unitPrefab = Resources.Load<GameObject>("Prefabs/UnitBing");

        int unitId = 100;

        // 在RegionSide1生成单位 (阵营1)
        SpawnUnitsForRegion(RegionSide1, unitPrefab, 1, "tree", ref unitId);

        // 在RegionSide2生成单位 (阵营2)
        SpawnUnitsForRegion(RegionSide2, unitPrefab, 2, "bottle", ref unitId);

        SpawnHerosForRegion(RegionHeroSide1[0], 100001, 1, ref unitId);
        SpawnHerosForRegion(RegionHeroSide1[1], 100002, 1, ref unitId);
        SpawnHerosForRegion(RegionHeroSide1[2], 100003, 1, ref unitId);
        SpawnHerosForRegion(RegionHeroSide2[0], 100004, 2, ref unitId);
        SpawnHerosForRegion(RegionHeroSide2[1], 100005, 2, ref unitId);
        SpawnHerosForRegion(RegionHeroSide2[2], 100006, 2, ref unitId);
    }

    private void SpawnUnitsForRegion(GameObject[] region, GameObject prefab, int side, string chessName, ref int idCounter)
    {
        foreach (GameObject spawnPoint in region)
        {
            if (spawnPoint != null)
            {
                // 实例化单位
                GameObject unitInstance = Instantiate(prefab, spawnPoint.transform.position, Quaternion.identity, Units.transform);
                unitInstance.name = $"UnitBing_{side}_{idCounter}";
                unitInstance.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

                // 获取并初始化Chess组件
                Chess chessComponent = unitInstance.GetComponent<Chess>();
                if (chessComponent != null)
                {
                    chessComponent.id = idCounter;
                    chessComponent.side = side;
                    chessComponent.chessName = chessName;
                    chessComponent.maxHp = 100;
                    chessComponent.moveSpeed = 10;
                    chessComponent.attackRange = 12;
                    chessComponent.attackDamage = 20;

                    // 可以在这里设置其他必要的初始化参数
                }
                else
                {
                    Debug.LogError("Chess component not found on UnitBing prefab");
                }

                idCounter++;
            }
        }
    }

    private void SpawnHerosForRegion(GameObject spawnPoint, int heroId, int side, ref int idCounter)
    {
        var heroConfig = HeroConfig.GetConfig((uint)heroId);
        GameObject heroPrefab = Resources.Load<GameObject>("Prefabs/UnitHero");
        if (spawnPoint != null)
        {
            // 实例化单位
            GameObject unitInstance = Instantiate(heroPrefab, spawnPoint.transform.position, Quaternion.identity, Units.transform);
            unitInstance.name = $"Hero_{side}_{idCounter}";
            unitInstance.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

            var heroInfo = heroInfoGroup.AddHero(side, heroId, 1);
            // 获取并初始化Chess组件
            Chess chessComponent = unitInstance.GetComponent<Chess>();
            if (chessComponent != null)
            {
                chessComponent.id = idCounter;
                chessComponent.side = side;
                chessComponent.chessName = heroConfig.Icon;
                chessComponent.maxHp = heroConfig.Hp;
                chessComponent.moveSpeed = heroConfig.MoveSpeed;
                chessComponent.attackRange = heroConfig.Range;
                chessComponent.attackDamage = heroConfig.Atk;
                chessComponent.heroInfo = heroInfo;

                // 可以在这里设置其他必要的初始化参数
            }
            else
            {
                Debug.LogError("Chess component not found on UnitBing prefab");
            }

            idCounter++;
        }
    }    

    // 世界坐标转格子坐标
    public Vector2Int WorldToGridPosition(Vector3 worldPosition, bool FloorToInt)
    {
        int x = 0;
        int z = 0;
        if (FloorToInt)
        {
            x = Mathf.FloorToInt(worldPosition.x / gridCellSize) * gridCellSize;
            z = Mathf.FloorToInt(worldPosition.z / gridCellSize) * gridCellSize;
        }
        else
        {
            x = Mathf.CeilToInt(worldPosition.x / gridCellSize) * gridCellSize;
            z = Mathf.CeilToInt(worldPosition.z / gridCellSize) * gridCellSize;
        }
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
        // UnityEngine.Debug.Log($"id:{unit.id} requiredGrids: Target Position = {targetPosition}, Collider Size = {collider.bounds.size}");
        // string gridPositions = string.Join(", ", requiredGrids);
        // UnityEngine.Debug.Log($"Grids: {gridPositions}");

        // 检查所有格子是否可用
        foreach (var gridPos in requiredGrids)
        {
            foreach (var entry in occupiedGrids)
            {
                if (entry.Key != unit.id && entry.Value.Contains(gridPos))
                {
              //      UnityEngine.Debug.Log("Grid " + gridPos + " is already occupied by unit: " + entry.Key);
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
            CreateDebugCube(unit.id, gridPos);
         //   UnityEngine.Debug.Log("Lock " + gridPos + " for unit: " + unit.id);
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
        Vector2Int minGridPos = WorldToGridPosition(minWorldPos, true);
        Vector2Int maxGridPos = WorldToGridPosition(maxWorldPos, false);

        // 遍历从最小到最大格子坐标的所有格子
        for (int x = minGridPos.x; x <= maxGridPos.x; x+= gridCellSize)
        {
            for (int z = minGridPos.y; z <= maxGridPos.y; z+= gridCellSize)
            {
                Vector2Int currentGrid = new Vector2Int(x, z);
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
            // 删除该单位占据的所有格子的调试cube
            foreach (var gridPos in occupiedGrids[unit.id])
            {
                DestroyDebugCube(gridPos);
            }
            occupiedGrids[unit.id].Clear();
          //  UnityEngine.Debug.Log("Released all grids for unit: " + unit.id);
        }
    }

    // 创建调试用的cube
    private void CreateDebugCube(int oid, Vector2Int gridPos)
    {
        if(!showDebugCube)
            return;

        if (debugGridCubes.ContainsKey(gridPos))
            return; // 已存在则不再创建

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(gridPos.x, 0.5f, gridPos.y);
        cube.transform.localScale = new Vector3(gridCellSize * 0.9f, 1f, gridCellSize * 0.9f);
        cube.GetComponent<Renderer>().material.color = Color.red;
        cube.name = "GridCube_" + oid;
        cube.transform.parent = Units.transform;
        cube.transform.localPosition += new Vector3(0, 10f, 0);

        debugGridCubes[gridPos] = cube;
    }

    // 销毁调试用的cube
    private void DestroyDebugCube(Vector2Int gridPos)
    {
        if(!showDebugCube)
            return;

        if (debugGridCubes.TryGetValue(gridPos, out GameObject cube))
        {
            Destroy(cube);
            debugGridCubes.Remove(gridPos);
        }
    }

    public void OnUnitDie(Chess dieUnit)
    {
        // 检查阵营1和阵营2是否还有存活单位
        bool side1HasUnits = false;
        bool side2HasUnits = false;

        foreach (Transform child in Units.transform)
        {
            Chess chessComponent = child.GetComponent<Chess>();
            if (chessComponent != null && chessComponent.hp > 0)
            {
                if (chessComponent.side == 1)
                {
                    side1HasUnits = true;
                }
                else if (chessComponent.side == 2)
                {
                    side2HasUnits = true;
                }

                // 如果两个阵营都有存活单位，提前结束检查
                if (side1HasUnits && side2HasUnits)
                {
                    break;
                }
            }
        }
        UnityEngine.Debug.Log($"id:{dieUnit.id} dieUnit.side:{dieUnit.side} side1HasUnits:{side1HasUnits} side2HasUnits:{side2HasUnits}");
        // 如果任一阵营没有存活单位，显示重启按钮
        if (!side1HasUnits || !side2HasUnits)
        {
            buttonRestart.gameObject.SetActive(true);
            textRestart.gameObject.SetActive(true);
            textRestart.text = side1HasUnits ? "蜀国胜利!!!" : "魏国胜利!!!";
        }
    }

    // 管理器销毁时释放所有格子
    private void OnDestroy()
    {
        occupiedGrids.Clear();

        // 销毁所有调试cube
        foreach (var cube in debugGridCubes.Values)
        {
            Destroy(cube);
        }
        debugGridCubes.Clear();
    }
}
