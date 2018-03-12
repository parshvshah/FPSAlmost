// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Enviro/MoonShader"
{
    Properties
    {
        _MainTex("Texture (RGB)", 2D) = "black" {}
        _Color("Color", Color) = (0.8, 0.8, 0.8, 1)
        _Brightness("Brightness", Float) = 5

    }
 
	SubShader
    {
		Tags {"LightMode" = "ForwardBase"}
        Pass
        {
            Name "PlanetBase"
            Cull Back
// Offset 1,993000
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
 
                #pragma fragmentoption ARB_fog_exp2
                #pragma fragmentoption ARB_precision_hint_fastest
 
                #include "UnityCG.cginc"
 				uniform float4 _SunPosition;
 				uniform float4 _MoonPosition;
 
                uniform sampler2D _MainTex;
                uniform float4 _MainTex_ST;
                uniform float4 _Color;
                uniform float4 _AtmoColor;
                uniform float _FalloffPlanet;
                uniform float _TransparencyPlanet;
                uniform float _Brightness;
 
                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float3 normal : TEXCOORD0;
                    float3 worldvertpos : TEXCOORD1;
                    float2 texcoord : TEXCOORD2;
                   // float3 vertpos : TEXCOORD3;
                };
 
                v2f vert(appdata_base v)
                {
                    v2f o;
 
                    o.pos = UnityObjectToClipPos (v.vertex);
                    o.normal = mul((float3x3)unity_ObjectToWorld, v.normal);
                    o.worldvertpos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
 					//o.vertpos = v.vertex.xyz;
                    return o;
                }
 
                float4 frag(v2f i) : COLOR
                {

                float3 sunPos = _SunPosition;
                float3 moonPos = _MoonPosition;

                float3 lightVector = normalize(_SunPosition - moonPos);

                    i.normal = normalize(i.normal);
  
                    float4 color = tex2D(_MainTex, i.texcoord) *_Color;

                  //  float d =  pow(dot(i.normal,lightVector),5);
                    float d = saturate(max(0.0,dot(i.normal,lightVector)) * 2);
                 	color = color * d;

                    return color * _Brightness;
                }
            ENDCG
        }
 }}