// 城堡消融材质：按世界坐标噪声逐步剔除表面，边缘发亮（战败后移除城堡用）
Shader "SanTeam/CastleDissolve"
{
    Properties
    {
        _Color ("Tint", Color) = (1,1,1,1)
        _MainTex ("Albedo", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 0
        _DissolveEdgeWidth ("Dissolve Edge Width", Range(0.001,0.5)) = 0.08
        _DissolveEdgeColor ("Dissolve Edge Color", Color) = (1, 0.35, 0.05, 1)
        _NoiseScale ("Noise Scale", Float) = 0.35
        _NoiseStrength ("Noise Strength", Range(0,1)) = 0.6
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert addshadow
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BumpMap;
        fixed4 _Color;
        fixed4 _DissolveEdgeColor;
        float _DissolveAmount;
        float _DissolveEdgeWidth;
        float _NoiseScale;
        float _NoiseStrength;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        // 简易 3D 值噪声：消融边界在世界空间连续，避免 UV 拉伸导致同一位置割裂
        float hash13(float3 p)
        {
            p = frac(p * 0.1031);
            p += dot(p, p.yzx + 33.33);
            return frac((p.x + p.y) * p.z);
        }

        float noise3(float3 p)
        {
            float3 i = floor(p);
            float3 f = frac(p);
            f = f * f * (3.0 - 2.0 * f);

            float n000 = hash13(i + float3(0, 0, 0));
            float n100 = hash13(i + float3(1, 0, 0));
            float n010 = hash13(i + float3(0, 1, 0));
            float n110 = hash13(i + float3(1, 1, 0));
            float n001 = hash13(i + float3(0, 0, 1));
            float n101 = hash13(i + float3(1, 0, 1));
            float n011 = hash13(i + float3(0, 1, 1));
            float n111 = hash13(i + float3(1, 1, 1));

            float nx00 = lerp(n000, n100, f.x);
            float nx10 = lerp(n010, n110, f.x);
            float nx01 = lerp(n001, n101, f.x);
            float nx11 = lerp(n011, n111, f.x);
            float nxy0 = lerp(nx00, nx10, f.y);
            float nxy1 = lerp(nx01, nx11, f.y);
            return lerp(nxy0, nxy1, f.z);
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));

            float n = noise3(IN.worldPos * _NoiseScale) * _NoiseStrength + (1.0 - _NoiseStrength) * 0.5;
            float edge = n - _DissolveAmount;
            clip(edge);

            o.Albedo = tex.rgb;
            o.Alpha = tex.a;

            float glow = 1.0 - saturate(edge / _DissolveEdgeWidth);
            o.Emission = _DissolveEdgeColor.rgb * glow * 2.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
