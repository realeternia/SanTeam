Shader "Custom/DiffuseWithOutline"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0.0, 1)) = 0.01
        _SecondTex ("Second Texture (RGB)", 2D) = "white" {}
        _SecondTexSize ("Second Texture Size", Range(0.1, 2.0)) = 0.3
        _SecondTexOffsetX ("Second Texture Offset X", Range(-1.0, 1.0)) = -0.8
        _SecondTexOffsetY ("Second Texture Offset Y", Range(-1.0, 1.0)) = -0.8
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 150

        // 描边Pass
        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }
            Cull Front
            ZWrite On
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float4 _OutlineColor;
            float _OutlineWidth;

            v2f vert (appdata_t v)
            {
     v2f o;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                worldPos.xyz += normalize(worldNormal) * _OutlineWidth;
                o.pos = mul(UNITY_MATRIX_VP, worldPos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }

        // 基础漫反射Pass
        Pass
        {
            Name "BASE"
            Tags { "LightMode" = "ForwardBase" }
            Cull Back
            ZWrite On
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _SecondTex;
            float4 _SecondTex_ST;
            float4 _SecondTex_TexelSize;
            float4 _Color;
            float _SecondTexSize;
            float _SecondTexOffsetX;
            float _SecondTexOffsetY;

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
                fixed4 col = tex2D(_MainTex, i.texcoord) * _Color;

                // 计算漫反射光照
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed diff = max(0, dot(worldNormal, worldLightDir));
                col.rgb *= diff * _LightColor0.rgb + unity_AmbientSky.rgb;

                // 处理第二张图片（显示在左下方）
                // 检查_SecondTex是否存在（非默认白色纹理）
                if (_SecondTex_TexelSize.z > 0.01) // 如果纹理宽度大于0.01（表示有实际纹理）
                {
                    float2 secondTexCoord = (i.texcoord - float2(0.5, 0.5)) * _SecondTexSize + float2(0.5 + _SecondTexOffsetX, 0.5 + _SecondTexOffsetY);
                    if (secondTexCoord.x >= 0 && secondTexCoord.x <= 1 && secondTexCoord.y >= 0 && secondTexCoord.y <= 1)
                    {
                        fixed4 secondCol = tex2D(_SecondTex, secondTexCoord);
                        // 使用alpha混合第二张图片
                        col = lerp(col, secondCol, secondCol.a);
                    }
                }

                return col;
            }
            ENDCG
        }
    }

    FallBack "Mobile/Diffuse"
}