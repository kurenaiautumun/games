﻿Shader "Custom/Outline"
{
    Properties
    {
        [HideInInspector] _MainTex  ("-", 2D) = ""{}
        _Distance ("Outline Width", Float) = 1
        _Color    ("Outline Color", Color) = (1, 0, 0, 0)
    }

    CGINCLUDE

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;
    float _Distance;
    half4 _Color;

    struct appdata {
        float4 vertex : POSITION;
        float2 texcoord : TEXCOORD0;
        float4 color : COLOR0;
    };

    struct v2f {
        float4 pos : SV_POSITION;
        fixed4 color : COLOR0;
        float2 uv : TEXCOORD0;
    };

    v2f vert( appdata v )
    {
        v2f o;
        o.pos = UnityObjectToClipPos (v.vertex);
        o.uv = v.texcoord;
        o.color = v.color;
        return o;
    }

    half4 frag(v2f i) : SV_Target 
    {
        // Simple sobel filter for the alpha channel.
        float d = _MainTex_TexelSize.xy * _Distance;

        half a1 = tex2D(_MainTex, i.uv + d * float2(-1, -1)).a;
        half a2 = tex2D(_MainTex, i.uv + d * float2( 0, -1)).a;
        half a3 = tex2D(_MainTex, i.uv + d * float2(+1, -1)).a;

        half a4 = tex2D(_MainTex, i.uv + d * float2(-1,  0)).a;
        half a6 = tex2D(_MainTex, i.uv + d * float2(+1,  0)).a;

        half a7 = tex2D(_MainTex, i.uv + d * float2(-1, +1)).a;
        half a8 = tex2D(_MainTex, i.uv + d * float2( 0, +1)).a;
        half a9 = tex2D(_MainTex, i.uv + d * float2(+1, +1)).a;

        float gx = - a1 - a2*2 - a3 + a7 + a8*2 + a9;
        float gy = - a1 - a4*2 - a7 + a3 + a6*2 + a9;

        float w = sqrt(gx * gx + gy * gy) / 4;

        // Mix the contour color.
        half4 source = tex2D(_MainTex, i.uv) * i.color;
        return half4(lerp(source.rgb, _Color.rgb, w), source.a);
    }

    ENDCG 

    Subshader
    {
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            ZTest Always Cull Off ZWrite Off
            Fog { Mode off }      
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
}