Shader "Unlit/Common_Normal"
{
    Properties
    {
        _MainTex ("MainTex",2D) = "white"{}
        [HDR]_MainCol("MainColor",Color)=(1,1,1,1)
        
        [Normal]_NormalMap("NormalMap",2D) = "bump"{}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Geometry"
        }
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_ST;
        half4 _MainCol;
        CBUFFER_END
        ENDHLSL

        Pass
        {
            Tags{"LightMode"="UniversalForward"}

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
            };
            struct Varings
            {
                float4 positionCS : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 tDirWS : TEXCOORD2;
                float3 bDirWS : TEXCOORD3;
            };

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);

            Varings vert(Attributes IN)
            {
                Varings OUT;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = positionInputs.positionCS;

                OUT.uv0=TRANSFORM_TEX(IN.uv0,_MainTex);

                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.tDirWS = normalize(TransformObjectToWorld(IN.tangentOS.xyz));
                OUT.bDirWS = cross(OUT.normalWS, OUT.tDirWS) * IN.tangentOS.w;
                
                return OUT;
            }

            float4 frag(Varings IN):SV_Target
            {
                float3 var_NormalMap = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, IN.uv0));
                float3x3 TBN = float3x3(IN.tDirWS, IN.bDirWS, IN.normalWS);
                float3 nDirWS = normalize(mul(var_NormalMap, TBN));

                float3 lDirWS = _MainLightPosition.xyz;

                float NoL = saturate(dot(nDirWS, lDirWS) * 0.5 + 0.5);
                
                half4 var_MainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv0); 
                return var_MainTex * _MainCol * NoL;
            }
            ENDHLSL
        }
    }
}