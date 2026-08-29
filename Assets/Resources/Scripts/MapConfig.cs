using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapConfig : MonoBehaviour
{
    // 每个势力的布阵中心点（长度8，对应阵营1~8）：
    // 位置 = 5x5布阵图的中心；Y旋转 = 布阵图正上方(最上面一排)在地图中朝向的方向
    public Transform[] SideCenters;

    // 布阵格子间距(米)，按地图大小调整
    public float FormationCellSize = 13f;

    public GameObject WallNode;
    public GameObject RegionNode;

    public int TeamMode;
    public int Mapid;
    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform region in RegionNode.transform)
        {
            region.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        foreach(Transform wall in WallNode.transform)
        {
            wall.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
