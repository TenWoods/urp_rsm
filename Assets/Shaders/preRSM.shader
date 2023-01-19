Shader "Hidden/preRSM"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"RenderType"="Opaque"}
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            struct output
            {
                float4 worldFlux : COLOR0;
                float4 worldNormal : COLOR1;
                float4 worldPosition : COLOR2;
                float4 depth : COLOR3;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            output frag (v2f i) : SV_Target
            {
                output o;
                /*fixed4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = 1 - col.rgb;*/
                o.depth = float4(1.0, 1.0, 1.0, 1.0);
                o.worldFlux = float4(1.0, 0.0, 0.0, 1.0);
                o.worldNormal = float4(0.0, 1.0, 0.0, 1.0);
                o.worldPosition = float4(0.0, 0.0, 1.0, 1.0);
                return o;
            }
            ENDCG
        }
    }
}
