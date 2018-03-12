// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Enviro/SkyRendering"
{
	Properties
	{
		_Clouds ("Texture", 2D) = "white" {}
		_Satellites ("Texture", 2D) = "white" {}
		_Skybox ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		//Blend One OneMinusSrcAlpha
		Tags { "Queue"="Background" "RenderType"="Background" }
		Pass
		{
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _Clouds;
			half4 _Clouds_ST;
			sampler2D _Satellites;
			half4 _Satellites_ST;
			sampler2D _Skybox;
			half4 _Skybox_ST;
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 cloud = tex2D(_Clouds, UnityStereoScreenSpaceUVAdjust(i.uv,_Clouds_ST));
				fixed4 sat = tex2D(_Satellites, UnityStereoScreenSpaceUVAdjust(i.uv,_Satellites_ST));
				fixed4 sky = tex2D(_Skybox, UnityStereoScreenSpaceUVAdjust(i.uv,_Skybox_ST));
			
				fixed blending = lerp(0,10,cloud.a);
				blending = clamp(blending,0,1);

				cloud = lerp(sky,cloud,blending);

				fixed4 final = lerp(cloud + sat,cloud,blending);

				return final;
			}
			ENDCG
		}
	}
}
