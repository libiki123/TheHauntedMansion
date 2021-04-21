// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/FX/Projector Light" {
    Properties{
       _Color("Main Color", Color) = (1,1,1,1)
       _ShadowTex("Cookie", 2D) = "" { TexGen ObjectLinear }
       _FalloffTex("FallOff", 2D) = "" { TexGen ObjectLinear }
    }
    SubShader{
        Pass {
        ZWrite off
        Fog { Color(0, 0, 0) }
        Color[_Color]
        ColorMask RGB
        Blend DstColor One
        Offset - 1, -1

        SetTexture[_ShadowTex] {
            combine texture * primary, ONE - texture
            Matrix[_Projector]
        }
        SetTexture[_FalloffTex] {
            constantColor(0,0,0,0)
            combine previous lerp(texture) constant
            Matrix[_ProjectorClip]
        }

    //Blend One One   // add color of _ShadowTex to the color in the framebuffer

    CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag

    // User-specified properties
    uniform fixed4 _Color;
    uniform sampler2D _ShadowTex;
    uniform sampler2D _FalloffTex;

    // Projector-specific uniforms
    uniform float4x4 unity_Projector; // transformation matrix from object space to projector space
    uniform float4x4 unity_ProjectorClip;

    struct vertexInput {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
    };

    struct vertexOutput {
        float4 pos : SV_POSITION;
        float4 posProj : TEXCOORD0; // position in projector space
        float4 uv : TEXCOORD1;
    };


    vertexOutput vert(vertexInput input)
    {
        vertexOutput output;

        output.posProj = mul(unity_Projector, input.vertex);
        output.pos = UnityObjectToClipPos(input.vertex);

        output.uv = mul(unity_ProjectorClip, input.vertex);

        return output;
    }

    float4 frag(vertexOutput input) : COLOR
    {
        if (input.posProj.w > 0.0) // in front of projector?
        {
            float4 shadowTex = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(input.posProj)) * _Color;
            float4 fallOff = tex2Dproj(_FalloffTex, UNITY_PROJ_COORD(input.uv));

            return lerp(shadowTex, float4(0), 1 - fallOff.a);
        }
        else // behind projector
        {
            return float4(0.0);
        }
    }

        ENDCG
    }
}
