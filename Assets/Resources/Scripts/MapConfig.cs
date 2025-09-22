using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapConfig : MonoBehaviour
{
    
    public GameObject[] RegionSide1; // 阵营1的出生点数组
    public GameObject[] RegionSide2; // 阵营2的出生点数组

    public GameObject[] RegionSide3; // 阵营3的出生点数组
    public GameObject[] RegionSide4; // 阵营4的出生点数组    

    public GameObject[] RegionSide5; // 阵营5的出生点数组
    public GameObject[] RegionSide6; // 阵营6的出生点数组   

    public GameObject[] RegionSide7; // 阵营7的出生点数组
    public GameObject[] RegionSide8; // 阵营8的出生点数组       


    
    public GameObject[] RegionHeroSide1; // 阵营1的出生点数组
    public GameObject[] RegionHeroSide2; // 阵营2的出生点数组
      
    public GameObject[] RegionHeroSide3; // 阵营3的出生点数组
    public GameObject[] RegionHeroSide4; // 阵营4的出生点数组

    public GameObject[] RegionHeroSide5; // 阵营5的出生点数组
    public GameObject[] RegionHeroSide6; // 阵营6的出生点数组 

    public GameObject[] RegionHeroSide7; // 阵营7的出生点数组
    public GameObject[] RegionHeroSide8; // 阵营8的出生点数组

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
