﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Enviro/Clouds" {
    Properties {
        _BaseColor ("BaseColor", Color) = (1,1,1,0.5)
        _SkyColor ("SkyColor", Color) = (1,1,1,0.5)
        _Density ("Density", Float ) = 0.5
        _CloudsMap ("Clouds Map", 2D) = "white" {}
         [HideInInspector]_Scale ("Scaling", Float ) = 4000
        _CloudCover ("CloudCover", Float ) = -0.25
        _CloudAlpha ("CloudAlpha", Float ) = 5
        _CloudAlphaCut ("CloudAlphaCut", Float ) = 0.0
        _HorizonBlend ("Horizon Blending", Range (0, 10)) = 1
        _direct ("direct light intensity", Float ) = 1
        _lightIntensity ("global light intensity", Float ) = 1
        [HideInInspector]_Offset ("Offset", Float ) = 1
        [HideInInspector]_CloudNormalsDirection ("_CloudNormalsDirection", Vector) = (1, -2, -1, 0)
    }
    SubShader {
        Tags {
        	"IgnoreProjector"="True"
        	"Queue" = "Transparent"
        	"RenderType"="Transparent"
        }
        LOD 200
        Pass {
          //  Name "ForwardBase"
            Tags {
          //      "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
          
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #define UNITY_PASS_FORWARDBASE
            
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile __ UNITY_COLORSPACE_GAMMA
            #pragma target 3.0
            #pragma glsl

           // #include "AutoLight.cginc" 
            //#include "Lighting.cginc"

            uniform float4 _timeScale;
            uniform fixed _Scale;
            uniform fixed _Offset;
            uniform fixed _CloudCover;
            uniform fixed4 _BaseColor;
            uniform fixed4 _SkyColor;
            uniform fixed4 _MoonColor;
            uniform fixed4 _SunColor;
            uniform fixed _CloudAlpha;
            uniform fixed _CloudAlphaCut;
            uniform fixed _Exposure;
            uniform fixed _Density;
            uniform sampler2D _CloudsMap;
            uniform float4 _CloudsMap_ST;
			uniform fixed _HorizonBlend;
            uniform fixed4 _CloudNormalsDirection;
          // uniform float4 _EnviroLighting;
            uniform float _direct;
            uniform float _lightIntensity;

            uniform float4 _SunDirection;
            uniform float4 _MoonDirection;

            struct VertexInput {
                half4 vertex : POSITION;
                half4 vertexColor : COLOR;
                half3 normal : NORMAL;
            	float2 texcoord0 : TEXCOORD0;
            };

            struct VertexOutput {
                half4 pos : SV_POSITION;
                half4 vertexColor : COLOR;
                half4 posWorld : TEXCOORD0;
                half3 normalDir : TEXCOORD1;
            	float2 uv0 : TEXCOORD2;
            	half3 view: TEXCOORD3;
            };

           	VertexOutput vert (VertexInput v) {
                VertexOutput o;
                UNITY_INITIALIZE_OUTPUT(VertexOutput, o);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.normalDir = normalize( mul( unity_ObjectToWorld, float4(v.normal, 2) ) );
                o.vertexColor = v.vertexColor;
                o.uv0 = v.texcoord0;
                o.view = normalize(WorldSpaceViewDir(v.vertex));
                return o;
            }

            fixed4 frag(VertexOutput i) : COLOR {
                float2 baseAnimation = _CloudsMap_ST.ba;
                baseAnimation += _timeScale.rg;

                fixed2 wUV = lerp(i.posWorld.xz / _Scale * _CloudsMap_ST.rg, i.uv0/(_CloudsMap_ST.rg * _Scale * 0.0005), 0.0);
                
                fixed2 nUV = wUV + (baseAnimation * 1) + fixed2(_Offset,_Offset);
                fixed2 nUV2 = wUV + (baseAnimation * 4) + fixed2(0.0, 0.5) + fixed2(_Offset,_Offset);
                fixed4 cloudTexture = tex2D(_CloudsMap, nUV);
                fixed4 cloudTexture2 = tex2D(_CloudsMap, nUV2);

                fixed baseMorph = ((saturate(cloudTexture.a + _CloudCover) * i.vertexColor.a) - cloudTexture2.a);
                //fixed3 baseMorphNormals = ((cloudTexture.rgb + _CloudCover) * i.vertexColor.a) - (cloudTexture2.rgb);

				fixed cloudMorph = baseMorph * _CloudAlpha;
				fixed horizonBlend = pow(saturate(1 - length(i.uv0 * 2.0 + -1.0)), _HorizonBlend);
        		cloudMorph *= horizonBlend;
                fixed cloudAlphaCut = cloudMorph -_CloudAlphaCut;
                 
                clip(saturate(ceil(cloudAlphaCut)) - 0.5);
                fixed fakeDepth = saturate(-_Density + (i.vertexColor.b * _CloudNormalsDirection.g + 1) / 2);
               
                cloudMorph = saturate(cloudMorph);

                fixed3 sunDir = normalize(_SunDirection);
                fixed3 moonDir = normalize(_MoonDirection);

                fixed lightIntensity = _lightIntensity;
             //   fixed3 lightDirection = pow(saturate(dot(-sunDir,i.normalDir)),1) + pow(saturate(dot(-moonDir,i.normalDir)),1);

               // #if defined(UNITY_COLORSPACE_GAMMA)
			   //  lightDirection = clamp(lightDirection,0.01,1);
			   //  #else
			   //  lightDirection = clamp(lightDirection,0.2,1);
			  //#endif

                fixed3 highlightsSun = pow(saturate(dot(-sunDir,i.view)),2);
                fixed3 highlightsMoon = (pow(saturate(dot(-moonDir,i.view)),15) * 20) * fakeDepth;
               
                 // fixed3 light = pow(saturate(dot(sunDir,baseMorphNormals)),0.5) * fakeDepth;

			    fixed3 highlightClrSun = (_SunColor * (lightIntensity * _direct)) * fakeDepth;
			    fixed3 highlightClrMoon = (_MoonColor * (lightIntensity * _direct)) * fakeDepth;

			    fixed3 baseClr = _BaseColor.rgb;
			    fixed3 baseClr2 = _BaseColor.rgb * 0.25;

			    fixed3 finalColor = (lerp(baseClr,baseClr2,1-fakeDepth) * _SkyColor);

			   //finalColor = lerp(finalColor * 0.1,lerp(finalColor,lightClr * fakeDepth,fakeDepth), lightDirection);
			   finalColor = lerp(finalColor,lerp((finalColor * highlightClrSun),finalColor, fakeDepth),highlightsSun);
			   finalColor = lerp(finalColor,lerp((finalColor * highlightClrMoon),finalColor, fakeDepth),highlightsMoon);
			   finalColor = lerp((finalColor * 0.75 * fakeDepth),finalColor,horizonBlend);
			  	#if defined(UNITY_COLORSPACE_GAMMA)
			 finalColor = pow(finalColor,0.454545) * _SkyColor;
				#endif
			   fixed alpha = clamp( (cloudMorph ) * (fakeDepth * 1.25),0,1);

               return fixed4(finalColor, alpha);
            }
            ENDCG
		}

    }
    FallBack "Diffuse"
}