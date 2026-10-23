Shader "Custom/WaterMoveUV"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}//水的纹理材质
		_Color("Tint", Color) = (1,1,1,1)//整体染色
		_ScrollX("Flow Offset X", Float) = 0//水流UV在X方向的初始偏移
		_ScrollY("Flow Offset Y", Float) = 0//水流UV在Y方向的初始偏移
		_SpeedX("Flow Speed X", Float) = 0.03//水流UV在X方向的流动速度
		_SpeedY("Flow Speed Y", Float) = 0.05//水流UV在Y方向的流动速度
		_LayerRate("Layer Rate", Range(0, 1)) = 0.35//第二层水纹的混合比例，越大水纹越碎
		_SpecColor("Specular Color", Color) = (1,1,1,1)//星点高光颜色
		_Shininess("Shininess", Range(1, 256)) = 64//高光锐利度，越大高光越集中
		_SpecStrength("Specular Strength", Range(0, 20)) = 10//星点强光强度
		_FresnelColor("Fresnel Color", Color) = (0.6,0.8,1,1)//边缘反光颜色
		_FresnelPower("Fresnel Power", Range(0.5, 8)) = 3//边缘反光范围，越大越贴边
		_FresnelStrength("Fresnel Strength", Range(0, 2)) = 0.35//边缘反光强度
		_SparkTiling("Sparkle Tiling", Float) = 14//星点密度，每单位UV的格子数
		_SparkSize("Sparkle Size", Range(0.001, 0.5)) = 0.005//单个星点大小（格内半径）
		_SparkThreshold("Sparkle Threshold", Range(0, 1)) = 0.55//越高星点越稀疏
		_SparkTwinkle("Sparkle Twinkle Speed", Float) = 6//星点闪烁速度
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			Tags { "LightMode"="ForwardBase" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase

			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			float _ScrollX;
			float _ScrollY;
			float _SpeedX;
			float _SpeedY;
			float _LayerRate;
			float _Shininess;
			float _SpecStrength;
			fixed4 _FresnelColor;
			float _FresnelPower;
			float _FresnelStrength;
			float _SparkTiling;
			float _SparkSize;
			float _SparkThreshold;
			float _SparkTwinkle;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
			};

			//格子随机数（用于让每颗星点的闪烁相位不同）
			float hash21(float2 p)
			{
				p = frac(p * float2(123.34, 456.21));
				p += dot(p, p + 45.32);
				return frac(p.x * p.y);
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float t = _Time.y;

				//水流主UV：按设定速度持续滚动，形成水流方向
				float2 flowUV = i.uv + float2(_ScrollX + t * _SpeedX, _ScrollY + t * _SpeedY);

				//第二层水纹：不同平铺与反向偏移，让水面有错位流动的层次感
				float2 layerUV = i.uv * 1.37 - float2(t * _SpeedX * 0.6, t * _SpeedY * 0.6) + float2(0.37, 0.21);

				fixed4 col = lerp(tex2D(_MainTex, flowUV), tex2D(_MainTex, layerUV), _LayerRate) * _Color;

				//油亮光照：Blinn-Phong 高光 + 边缘 Fresnel 反光
				float3 N = normalize(i.worldNormal);
				float3 V = normalize(UnityWorldSpaceViewDir(i.worldPos));
				float3 L = normalize(UnityWorldSpaceLightDir(i.worldPos));
				float3 H = normalize(L + V);

				float spec = pow(saturate(dot(N, H)), _Shininess);
				float fresnel = pow(1.0 - saturate(dot(N, V)), _FresnelPower) * _FresnelStrength;

				//星星点：按格子切分，每格随机决定是否闪、闪烁相位不同；格子随水流一起平移
				float2 sparkUV = i.uv * _SparkTiling + float2(t * _SpeedX * 2.0, -t * _SpeedY * 2.0);
				float2 cell = floor(sparkUV);
				float2 f = frac(sparkUV) - 0.5;
				float rnd = hash21(cell);
				//闪烁：高次幂让亮峰更尖锐（大部分时间暗、瞬间爆亮）
				float twinkle = pow(0.5 + 0.5 * sin(t * _SparkTwinkle + rnd * 6.2831), 3.0);
				//用 fwidth 做抗锯齿，极小星点也不会因亚像素采样而完全消失
				float d = length(f);
				float aa = max(fwidth(d), 1e-5);
				float dotMask = 1.0 - smoothstep(_SparkSize - aa, _SparkSize, d);
				float spark = dotMask * smoothstep(_SparkThreshold, _SparkThreshold + 0.2, rnd) * twinkle;

				//强光只打在星星点上，形成一点一点的反射光斑
				col.rgb += _SpecColor.rgb * _LightColor0.rgb * spark * (1.0 + spec * 4.0) * _SpecStrength;
				col.rgb += _FresnelColor.rgb * fresnel * lerp(0.4, 1.0, spark);

				return col;
			}
			ENDCG
		}
	}
	Fallback Off
}
