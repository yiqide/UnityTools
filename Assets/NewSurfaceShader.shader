Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Tex("图片",2D)="white" {}
        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _ColorMask("Color Mask",Float) = 15
        _HP("裁切半径",Range(0,1)) =0.5
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAltas" = "True"
        }
        LOD 200
        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest[unity_GUIZTeseMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]


        CGPROGRAM
        #pragma surface surf NoLighting alpha:auto
        #pragma target 3.0
        sampler2D _Tex;

        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
            fixed4 c;
            c.rgb = s.Albedo;
            c.a = s.Alpha;
            return c;
        }

        half _Metallic;
        float _HP;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input IN, inout SurfaceOutput o)
        {
            half3 color = tex2D(_Tex, IN.uv_MainTex).rgb;
            o.Emission = color;
            half2 pos = half2(IN.uv_MainTex.x, IN.uv_MainTex.y);
            o.Alpha=step(distance(pos, half2(0.5, 0.5)), _HP);
        }
        ENDCG
    }
    FallBack "Diffuse"
}