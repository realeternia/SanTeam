using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class GlowBeamController : MonoBehaviour
{
    [Header("Beam Settings")]
    public Color beamColor = Color.white;
    public Color glowColor = Color.cyan;
    [Range(0.1f, 5f)]
    public float beamWidth = 1f;
    [Range(0f, 10f)]
    public float glowIntensity = 3f;
    [Range(0f, 5f)]
    public float scrollSpeed = 1f;
    [Range(0f, 1f)]
    public float opacity = 0.8f;
    
    [Header("Texture Settings")]
    public Texture2D beamTexture;
    [Range(0.1f, 10f)]
    public float noiseScale = 3f;
    
    private Material material;
    private Renderer beamRenderer;

    public Transform parentTransform;

    public Chess source;
    public Chess target;
    
    void Start()
    {
        beamRenderer = GetComponent<Renderer>();
        material = beamRenderer.material;
        
        UpdateBeamProperties();
        StartBreathing();
    }
    
    void Update()
    {
        if (source == null || target == null)
        {
            Destroy(gameObject);
            return;
        }

        UpdateBeamPosition();

    }
    
    void UpdateBeamProperties()
    {
        if (material != null)
        {
            material.SetColor("_Color", beamColor);
            material.SetColor("_GlowColor", glowColor);
            material.SetFloat("_BeamWidth", beamWidth);
            material.SetFloat("_GlowIntensity", glowIntensity);
            material.SetFloat("_Speed", scrollSpeed);
            material.SetFloat("_Opacity", opacity);
            material.SetFloat("_NoiseScale", noiseScale);
            
            if (beamTexture != null)
            {
                material.SetTexture("_MainTex", beamTexture);
            }
        }
    }

    void UpdateBeamPosition()
    {
        // 计算方向和距离
        Vector3 direction = target.transform.position - source.transform.position;
        float distance = direction.magnitude;

        if (distance > 0)
        {
            // 设置光束位置和旋转 - 定位到source的中心
            parentTransform.position = (source.transform.position + target.transform.position) / 2;
            parentTransform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 90, 0);
            
            // 设置光束长度（假设光束的本地Z轴为长度方向）
            parentTransform.localScale = new Vector3(distance/10, 0.3f, 0.3f);
        }
    }

    // 动态改变发光颜色
    public void SetGlowColor(Color newColor)
    {
        UnityEngine.Debug.Log("SetGlowColor " + newColor);
        glowColor = newColor;
        UpdateBeamProperties();
    }
    
    // 循环呼吸效果
    public IEnumerator FadeBeam(float minOpacity, float maxOpacity, float speed)
    {
        while (true)
        {
            float sinValue = (Mathf.Sin(Time.time * speed) + 1) * 0.5f;
            opacity = Mathf.Lerp(minOpacity, maxOpacity, sinValue);
            UpdateBeamProperties();
            yield return null;
        }
    }

    // 启动呼吸效果
    public void StartBreathing(float minOpacity = 0.1f, float maxOpacity = 1f, float speed = 2f)
    {
        StartCoroutine(FadeBeam(minOpacity, maxOpacity, speed));
    }

    // 停止呼吸效果
    public void StopBreathing()
    {
        StopAllCoroutines();
    }

    // 设置固定透明度
    public void SetOpacity(float targetOpacity)
    {
        StopAllCoroutines();
        opacity = targetOpacity;
        UpdateBeamProperties();
    }

    public void SetSourceAndTarget(Chess source, Chess target)
    {
        this.source = source;
        this.target = target;
    }

}