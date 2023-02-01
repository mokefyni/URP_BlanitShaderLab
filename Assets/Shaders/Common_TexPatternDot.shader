Shader "URPCustom/Common_TexPatternDot"
{
    Properties
    {
        _MainTex ("MainTex",2D) = "white"{}
        [HDR]_MainCol("MainColor",Color)=(1,1,1,1)
        
        _DotTilingSpeed("DotTilingSpeed", vector) = (1, 1, 0, 0)
        _DotRadius("DotRadius", range(-1, 1)) = 0.25
        _DotSoftEdge("DotSoftEdge", range(0, 1)) = 0
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
        half4 _DotTilingSpeed;
        half _DotRadius;
        half _DotSoftEdge;
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
            };
            struct Varings
            {
                float4 positionCS : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);


            // =========================== Dot Calculation ===========================
            float GetTexVal(float2 coord, float radius, float softEdge)
            {
                float dist = distance(frac(coord), float2(0.5, 0.5));
                float stepVal = smoothstep(radius - softEdge - 0.001, radius + softEdge, dist * sign(radius));
                
                return stepVal;
            }

            Varings vert(Attributes IN)
            {
                Varings OUT;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = positionInputs.positionCS;

                OUT.uv0=TRANSFORM_TEX(IN.uv0,_MainTex);
                return OUT;
            }

            float4 frag(Varings IN):SV_Target
            {
                // half4 var_MainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv0);
                
                float2 uvTex = IN.uv0 * _DotTilingSpeed.xy + _DotTilingSpeed.zw * frac(_Time.y);
                
                half texVal = GetTexVal(uvTex, _DotRadius, _DotSoftEdge);
                
                return _MainCol * texVal;
            }
            ENDHLSL
        }
    }
}