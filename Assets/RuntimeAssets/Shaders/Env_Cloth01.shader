Shader "Unlit/Env_Cloth01"
{
    Properties
    {
        _MainTex ("MainTex",2D) = "white"{}
        [HDR]_MainCol("MainColor",Color)=(1,1,1,1)
        
        [Cloth]
        _RimScale("RimScale", range(0, 8)) = 1
        _RimExp("RimExp", range(0, 120)) = 60
        _InnerScale("InnerScale", range(0, 8)) = 1
        _InnerExp("InnerExp", range(0, 120)) = 60
        _LambertScale("LambertScale", range(0, 2)) = 1
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

        half _RimScale;
        half _RimExp;
        half _InnerScale;
        half _InnerExp;
        half _LambertScale;
        
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
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };
            struct Varings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
            };

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);

            Varings vert(Attributes IN)
            {
                Varings OUT;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = positionInputs.positionCS;
                OUT.positionWS = positionInputs.positionWS;

                OUT.uv=TRANSFORM_TEX(IN.uv,_MainTex);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                return OUT;
            }

            float4 frag(Varings IN):SV_Target
            {
                half3 vDirWS = GetWorldSpaceNormalizeViewDir(IN.positionWS);
                half3 normalWS = normalize(IN.normalWS);
                half VoN = saturate(dot(vDirWS, normalWS));
                half rim = _RimScale * pow(1 - VoN, _RimExp);
                half inner = _InnerScale * pow(VoN, _InnerExp);
                half lambert = _LambertScale;

                half clothMultiplier = rim + inner + lambert;
                
                half4 var_MainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                half4 finalCol = var_MainTex * _MainCol;
                finalCol *= clothMultiplier;
                
                return finalCol;
            }
            ENDHLSL
        }
    }
}