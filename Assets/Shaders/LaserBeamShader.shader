// 技能激光专用 Shader（与好友连线的 Custom/GlowBeam 做出差异）：
// 1) 双层结构：白热核心(core) + 外层光晕(halo)，比 GlowBeam 的单一 sin 亮暗更有"束"的实体感
// 2) 能量脉动：_PulseSpeed/_PulseAmp 让核心粗细与整体强度随时间呼吸缩放
// 3) 能量流动：沿长度方向流动的亮带(_BandCount/_Speed) + 高频抖动(_Flicker)，形成颗粒/电流感
// 4) 末端能量堆积：_TipBoost 让激光命中端更亮，衔接命中粒子
// 使用加法混合(Blend SrcAlpha One)，强化能量发光观感。
Shader "Custom/LaserBeam"
{
    Properties
    {
        _MainTex ("Energy Texture", 2D) = "white" {}
        _Color ("Core Color", Color) = (1, 1, 1, 1)
        _GlowColor ("Glow Color", Color) = (0.0, 0.85, 1.0, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 10)) = 3
        _BeamWidth ("Beam Width", Range(0.01, 1)) = 0.3
        _CoreRatio ("Core Ratio", Range(0.01, 1)) = 0.35
        _CoreSharpness ("Core Sharpness", Range(0.5, 12)) = 5
        _HaloFalloff ("Halo Falloff", Range(0.5, 8)) = 1.6
        _Speed ("Energy Flow Speed", Range(-8, 8)) = 3
        _BandCount ("Energy Band Count", Range(0.1, 30)) = 7
        _FlowDepth ("Energy Flow Depth", Range(0, 1)) = 0.55
        _PulseSpeed ("Pulse Speed", Range(0, 20)) = 7
        _PulseAmp ("Pulse Amplitude", Range(0, 1)) = 0.3
        _Flicker ("Flicker", Range(0, 1)) = 0.22
        _NoiseScale ("Noise Scale", Range(0.1, 20)) = 6
        _TipBoost ("Tip Boost", Range(0, 4)) = 1.5
        _Opacity ("Opacity", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100

        Blend SrcAlpha One
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
            float _BeamWidth;
            float _CoreRatio;
            float _CoreSharpness;
            float _HaloFalloff;
            float _Speed;
            float _BandCount;
            float _FlowDepth;
            float _PulseSpeed;
            float _PulseAmp;
            float _Flicker;
            float _NoiseScale;
            float _TipBoost;
            float _Opacity;

            float hash(float2 st)
            {
                return frac(sin(dot(st, float2(12.9898, 78.233))) * 43758.5453123);
            }

            float noise(float2 st)
            {
                float2 i = floor(st);
                float2 f = frac(st);

                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));

                float2 u = f * f * (3.0 - 2.0 * f);

                return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 宽度方向：uv.y 0~1 横跨光束截面，d=0 为中轴、d=1 为最外侧
                float d = abs(i.uv.y * 2.0 - 1.0);

                // 能量脉动：核心粗细与强度随时间呼吸，做出"会缩放"的能量感
                float pulse = 1.0 + _PulseAmp * sin(_Time.y * _PulseSpeed);
                float dd = saturate(d / max(_BeamWidth * pulse, 0.001));

                // 双层结构：白热核心 + 外层光晕
                float core = pow(saturate(1.0 - dd / max(_CoreRatio, 0.001)), _CoreSharpness);
                float halo = pow(saturate(1.0 - dd), _HaloFalloff);

                // 沿长度方向流动的能量带（颗粒感来源）
                float band = 0.5 + 0.5 * sin((i.uv.x * _BandCount - _Time.y * _Speed) * 6.2831853);
                float energy = lerp(1.0 - _FlowDepth, 1.0, band);

                // 高频抖动，让激光有电流般的闪烁
                float flick = 1.0 - _Flicker * noise(float2(i.uv.x * _NoiseScale, _Time.y * 10.0));

                // 起点略淡、末端能量堆积（命中端更亮）
                float tailFade = smoothstep(0.0, 0.03, i.uv.x);
                float tipHot = 1.0 + _TipBoost * smoothstep(0.55, 1.0, i.uv.x);

                // 能量纹理（未指定时为白色，不改变观感）
                float2 flowUV = float2(i.uv.x - _Time.y * _Speed * 0.15, i.uv.y);
                float3 texMul = tex2D(_MainTex, flowUV).rgb;

                fixed3 rgb = _Color * core * 2.0 + _GlowColor * halo;
                rgb *= _GlowIntensity * energy * flick * tailFade * tipHot * texMul * i.color.rgb;

                float alpha = saturate(core + halo * 0.75) * _Opacity * i.color.a;

                fixed4 finalColor = fixed4(rgb, alpha);
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
                return finalColor;
            }
            ENDCG
        }
    }

    FallBack "Sprites/Default"
}