// A method to sample 2d tex in 3d space
// lerp xz direction when sample 2d tex
// so the faces around the y-axis can sample naturally
Shader "Unlit/Common_LerpXZ"
{
    Properties
    {
        _MainTex ("MainTex",2D) = "white"{}
        [HDR]_MainCol("MainColor",Color)=(1,1,1,1)
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
            };
            struct Varings
            {
                float4 positionCS : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            Varings vert(Attributes IN)
            {
                Varings OUT;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = positionInputs.positionCS;
                OUT.positionWS = positionInputs.positionWS;

                OUT.uv0=TRANSFORM_TEX(IN.uv0,_MainTex);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                return OUT;
            }

            float4 frag(Varings IN):SV_Target
            {
                half3 normalWS = (IN.normalWS);
                
                half x_d = round(abs(normalWS.x));
                half x_sign = sign(normalWS.x);

                //inverse partial uv to prevent symmetry
                half2 x_r = half2(lerp(1.0, -1.0, saturate(x_d * x_sign)), 1.0);
                
                half2 uvMain = lerp(IN.positionWS.xy, IN.positionWS.zy*x_r, x_d);
                
                half4 var_MainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvMain); 
                return var_MainTex * _MainCol;
            }
            ENDHLSL
        }
    }
}