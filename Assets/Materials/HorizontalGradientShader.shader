Shader "UI/HorizontalGradientMasked"
{
    Properties
    {
        _ColorLeft ("Left Color", Color) = (1,1,1,1)
        _ColorRight ("Right Color", Color) = (0,0,0,1)
        _MainTex ("Mask (Source Image)", 2D) = "white" {}
        _ShadowColor ("Shadow Color", Color) = (0,0,0,0.5)
        _ShadowOffsetX ("Shadow Offset X", Float) = 0.02
        _ShadowOffsetY ("Shadow Offset Y", Float) = -0.02
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            fixed4 _ColorLeft;
            fixed4 _ColorRight;
            fixed4 _ShadowColor;
            float _ShadowOffsetX;
            float _ShadowOffsetY;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Shadow sampling
                float2 shadowUV = i.uv + float2(_ShadowOffsetX, _ShadowOffsetY);
                fixed4 shadowMask = tex2D(_MainTex, shadowUV);
                fixed4 shadow = _ShadowColor;
                shadow.a *= shadowMask.a;

                // Main image
                fixed4 mask = tex2D(_MainTex, i.uv);
                fixed4 grad = lerp(_ColorLeft, _ColorRight, i.uv.x);
                grad.a *= mask.a;

                // Blend shadow behind main image
                fixed4 result = grad;
                result.rgb = lerp(shadow.rgb, grad.rgb, mask.a);
                result.a = max(grad.a, shadow.a);

                return result;
            }
            ENDCG
        }
    }
}