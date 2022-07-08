Shader "MyShader/DepthOnlyShader"
{
	SubShader
	{
        // DepthOnly使用時の深度バッファ用パス
        Pass
        {
            Name "DepthOnlyPass"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };


            v2f vert(appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v2f o;

                o.vertex = TransformObjectToHClip(v.vertex.xyz);

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {

                return 0.0;
            }

            ENDHLSL
        }
	}
}
