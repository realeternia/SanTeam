// 冰冻材质效果：用于棋子冰冻 Buff 的换材质（BuffConfig「切材质」列）。
// 冰效与头像融合：头像去色染冰蓝得到"冰化头像"，再叠加由冰晶贴图高度场构造的扰动法线，
// 用扰动法线驱动漫反射/镜面高光/边缘反光，让平板棋子也有凹凸与镜面质感。
// _MainTex 由 Chess.AddMaterialEffect 运行时从原材质拷入；_FrostTex 由材质面板指定（Textures/Buffs/frozen_ice）；
// _FrostOffset 由 Chess 按棋子随机设置，避免所有棋子冰纹一致。
Shader "Custom/FrozenIce"
{
    Properties
    {
        _MainTex ("英雄头像", 2D) = "white" {}
        _TexAlpha ("冰化程度", Range(0, 1)) = 0.85

        // ---- 冰晶 / 凹凸 ----
        _FrostTex ("冰晶/高度贴图 (RGB)", 2D) = "white" {}
        _FrostTiling ("冰晶密度", Range(0.1, 8)) = 2
        _FrostOffset ("冰晶偏移", Vector) = (0, 0, 0, 0)
        _BumpScale ("凹凸强度", Range(0, 50)) = 12

        // ---- 冰体外观 ----
        _IceColor ("冰体色调", Color) = (0.62, 0.82, 1, 1)
        _FrostColor ("冰霜亮色", Color) = (0.9, 0.97, 1, 1)
        _Brightness ("整体亮度", Range(0.5, 2)) = 1.1

        // ---- 镜面 ----
        _IceSpecColor ("镜面高光颜色", Color) = (1, 1, 1, 1)
        _SpecIntensity ("镜面强度", Range(0, 4)) = 1.5
        _SpecPower ("镜面收敛(越大越锐)", Range(1, 256)) = 60
        _FresnelColor ("边缘反光颜色", Color) = (0.85, 0.95, 1, 1)
        _FresnelPower ("边缘反光收敛", Range(0.5, 8)) = 3
        _FresnelIntensity ("边缘反光强度", Range(0, 2)) = 0.6
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 150

        Pass
        {
            Name "BASE"
            Tags { "LightMode" = "ForwardBase" }
            Cull Back
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex; float4 _MainTex_ST;
            sampler2D _FrostTex; float4 _FrostTex_ST; float4 _FrostTex_TexelSize;
            float _TexAlpha;
            float _FrostTiling;
            float4 _FrostOffset;
            float _BumpScale;
            fixed4 _IceColor;
            fixed4 _FrostColor;
            float _Brightness;
            fixed4 _IceSpecColor;
            float _SpecIntensity;
            float _SpecPower;
            fixed4 _FresnelColor;
            float _FresnelPower;
            float _FresnelIntensity;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 英雄头像
                fixed4 portrait = tex2D(_MainTex, i.texcoord);

                float3 worldNormal = normalize(i.worldNormal);
                float3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                float3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

                // 冰晶高度场采样（中心 + 上下左右）
                float2 fuv = i.texcoord * _FrostTiling + _FrostOffset.xy;
                float2 texel = _FrostTex_TexelSize.xy * _FrostTiling;
                float hC = tex2D(_FrostTex, fuv).r;
                float hL = tex2D(_FrostTex, fuv - float2(texel.x, 0)).r;
                float hR = tex2D(_FrostTex, fuv + float2(texel.x, 0)).r;
                float hD = tex2D(_FrostTex, fuv - float2(0, texel.y)).r;
                float hU = tex2D(_FrostTex, fuv + float2(0, texel.y)).r;

                // 由高度梯度构造切空间法线（制造凹凸）
                float3 tn = normalize(float3((hL - hR) * _BumpScale, (hD - hU) * _BumpScale, 1.0));

                // 由屏幕导数构造切空间基（不依赖网格切线）
                float3 dpdx = ddx(i.worldPos);
                float3 dpdy = ddy(i.worldPos);
                float2 duvdx = ddx(i.texcoord);
                float2 duvdy = ddy(i.texcoord);
                float3 T = normalize(dpdx * duvdy.y - dpdy * duvdx.y);
                float3 B = normalize(cross(worldNormal, T));
                float3 N = normalize(T * tn.x + B * tn.y + worldNormal * tn.z);

                // 冰化头像：保留头像明暗结构、去色染冰蓝 + 冰晶亮斑
                float lum = dot(portrait.rgb, float3(0.299, 0.587, 0.114));
                fixed3 frozen = lum * _IceColor.rgb;
                frozen = lerp(frozen, _FrostColor.rgb, hC * 0.6);

                // 漫反射：用扰动法线，让冰面有明暗起伏
                float diff = max(0.0, dot(N, worldLightDir));
                frozen *= (0.7 + 0.3 * diff);

                // 镜面高光：用扰动法线 → 闪点/凹凸感
                float3 H = normalize(worldLightDir + viewDir);
                float spec = pow(saturate(dot(N, H)), _SpecPower) * _SpecIntensity;
                frozen += _IceSpecColor.rgb * spec;

                // 边缘反光（菲涅尔）→ 镜面感
                float fres = pow(1.0 - saturate(dot(N, viewDir)), _FresnelPower);
                frozen += _FresnelColor.rgb * fres * _FresnelIntensity;

                // 冰效与头像融合：_TexAlpha 控制冰化程度（0=纯头像 1=纯冰化）
                fixed3 col = lerp(portrait.rgb, frozen, _TexAlpha) * _Brightness;
                return fixed4(col, portrait.a);
            }
            ENDCG
        }
    }

    FallBack "Mobile/Diffuse"
}
