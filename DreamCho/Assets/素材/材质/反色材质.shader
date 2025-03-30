Shader "Custom/RenderingLayerColor"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _WhiteLayer("White Layer Mask", Float) = 1
        _BlackLayer("Black Layer Mask", Float) = 2
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                uint _WhiteLayer;
                uint _BlackLayer;
            CBUFFER_END

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                #ifdef PIXELSNAP_ON
                o.vertex = UnityPixelSnap(o.vertex);
                #endif
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // 获取当前物体的渲染层掩码
                uint renderingLayers = GetMeshRenderingLayer();
                
                half4 color = tex2D(_MainTex, i.uv) * _Color;
                
                // 检查是否包含White层
                if (renderingLayers & _WhiteLayer)
                {
                    color.rgb = 0; // 黑色
                    color.a = 1;
                }
                // 检查是否包含Black层
                else if (renderingLayers & _BlackLayer)
                {
                    color.rgb = 1; // 白色
                    color.a = 1;
                }
                else
                {
                    color.a = 0; // 透明
                }
                
                return color;
            }
            ENDHLSL
        }
    }
}