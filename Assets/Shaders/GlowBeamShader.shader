Shader "Custom/GlowBeam"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _GlowColor ("Glow Color", Color) = (0,1,1,1)
        _GlowIntensity ("Glow Intensity", Range(0, 10)) = 2
        _GlowPower ("Glow Power", Range(0.1, 5)) = 1
        _BeamWidth ("Beam Width", Range(0.01, 2)) = 0.5
        _Speed ("Scroll Speed", Range(0, 5)) = 1
        _NoiseScale ("Noise Scale", Range(0.1, 10)) = 1
        _Opacity ("Opacity", Range(0, 1)) = 1
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _GlowColor;
            float _GlowIntensity;
            float _GlowPower;
            float _BeamWidth;
            float _Speed;
            float _NoiseScale;
            float _Opacity;
            
            // 简单的噪声函数
            float random(float2 st)
            {
                return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }
            
            float noise(float2 st)
            {
                float2 i = floor(st);
                float2 f = frac(st);
                
                float a = random(i);
                float b = random(i + float2(1.0, 0.0));
                float c = random(i + float2(0.0, 1.0));
                float d = random(i + float2(1.0, 1.0));
                
                float2 u = f * f * (3.0 - 2.0 * f);
                
                return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 滚动UV
                float2 scrolledUV = i.uv;
                scrolledUV.x += _Time.y * _Speed;
                scrolledUV.y += _Time.y * _Speed * 0.5;
                
                // 采样主纹理
                fixed4 col = tex2D(_MainTex, scrolledUV) * i.color;
                
                // 创建光束形状 - 使用sin函数创建中间亮两边暗的效果
                float beamShape = pow(abs(sin(i.uv.y * 3.14159)), _GlowPower);
                
                // 添加噪声变化
                float noiseVal = noise(i.uv * _NoiseScale + _Time.y * 0.5) * 0.3 + 0.7;
                
                // 计算发光强度
                float glow = beamShape * noiseVal * _GlowIntensity;
                
                // 创建内外发光
                float innerGlow = pow(beamShape, 2.0) * _GlowIntensity * 2;
                float outerGlow = pow(beamShape, 0.5) * _GlowIntensity * 0.5;
                
                // 组合发光颜色
                fixed4 glowColor = _GlowColor * (innerGlow + outerGlow);
                
                // 最终颜色 = 纹理颜色 + 发光颜色
                fixed4 finalColor = col + glowColor;
                
                // 应用透明度
                finalColor.a = col.a * _Opacity;
                
                // 边缘淡出
                float edgeFade = 1.0 - abs(i.uv.x - 0.5) * 2.0 / _BeamWidth;
                edgeFade = saturate(edgeFade);
                finalColor.a *= edgeFade;
                
                // 垂直淡出
                float verticalFade = smoothstep(0.0, 0.1, i.uv.y) * smoothstep(1.0, 0.9, i.uv.y);
                finalColor.a *= verticalFade;
                
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
                return finalColor;
            }
            ENDCG
        }
        
        // 额外的高斯模糊发光pass
        Pass
        {
            Name "Glow"
            Tags { "LightMode"="ForwardBase" }
            
            Blend One One
            ZWrite Off
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _GlowColor;
            float _GlowIntensity;
            float _BeamWidth;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 创建更宽的发光效果
                float glowShape = pow(abs(sin(i.uv.y * 3.14159)), 0.5);
                float width = _BeamWidth * 2;
                float edgeFade = 1.0 - abs(i.uv.x - 0.5) * 2.0 / width;
                edgeFade = saturate(edgeFade);
                
                fixed4 glow = _GlowColor * _GlowIntensity * glowShape * edgeFade * 0.5;
                return glow;
            }
            ENDCG
        }
    }
    
    FallBack "Sprites/Default"
}