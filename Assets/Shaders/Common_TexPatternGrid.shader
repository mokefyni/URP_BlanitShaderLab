Shader "URPCustom/Common_TexPatternGrid"
{
    Properties
    {
        _MainTex ("MainTex",2D) = "white"{}
        [HDR]_MainCol("MainColor",Color)=(1,1,1,1)
        
        _GridTilingSpeed("GridTilingSpeed", vector) = (1, 1, 0, 0)
        _GridWidth("GridWidth", range(-1, 1)) = 0.25
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
        half4 _GridTilingSpeed;
        half _GridWidth;
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


            // =========================== Grid Calculation ===========================
            float GetTexVal(float2 coord, float gridWidth)
            {
                float signWidth = step(0.0, gridWidth); //根据gridWidth进行随后的计算
                return floor(1.0-signWidth + gridWidth +
                            lerp(max(frac(coord.x), frac(coord.y)),
                                min(frac(coord.x), frac(coord.y)), signWidth));
                
                // return smoothstep(gridWidth-softEdge, gridWidth+softEdge,frac(coord.y));
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
                
                float2 uvTex = IN.uv0 * _GridTilingSpeed.xy + frac(_GridTilingSpeed.zw * _Time.y);
                
                half texVal = GetTexVal(uvTex, _GridWidth);
                
                return _MainCol * texVal;
            }
            ENDHLSL
        }
    }
}