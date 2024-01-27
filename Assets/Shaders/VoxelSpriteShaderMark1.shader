Shader "VoxelSpriteShaderMark1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _Offset ("Offset", float) = 2.0
        _Count ("Count", int) = 2.0
    }
    SubShader
    {
        Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" "IgnoreProjector"="True" }
        Cull Off
        ZWrite On
        AlphaToMask On
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry GSMain
            #pragma fragment frag
            #pragma target 5.0
 
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            uniform float4 _Color;
            float _Offset;
            int _Count;
 
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = v.vertex;
                o.uv = v.uv;
                return o;
            }
 
            [maxvertexcount(60)]
            void GSMain(triangle appdata patch[3], inout TriangleStream<v2f> stream)
            {
                v2f GS;
 
                for(uint i = 0; i < 3; ++i)
                {
                    uint index = i;
                    GS.pos = UnityObjectToClipPos(patch[index].vertex);
                    GS.uv = patch[index].uv;
                    stream.Append(GS);
                }
                stream.RestartStrip();
 
                for(uint i = 0; i < 3; ++i)
                {
                    uint index = i;
                    GS.pos = UnityObjectToClipPos(patch[index].vertex + float4(0, 0, _Offset, 0));
                    GS.uv = patch[index].uv;
                    stream.Append(GS);
                }
                stream.RestartStrip();
 
                for (uint i = 0; i < 3; i++)
                {
                    uint index = i;
                    uint nextIndex = (index + 1) % 3;
 
                    // Triangle 1
 
                    GS.pos = UnityObjectToClipPos(patch[index].vertex);
                    GS.uv = patch[index].uv;
                    stream.Append(GS);
                    GS.pos = UnityObjectToClipPos(patch[index].vertex + float4(0, 0, _Offset, 0));
                    GS.uv = patch[index].uv;
                    stream.Append(GS);
 
                    GS.pos = UnityObjectToClipPos(patch[nextIndex].vertex + float4(0, 0, _Offset, 0));
                    GS.uv = patch[nextIndex].uv;
                    stream.Append(GS);
 
                    stream.RestartStrip();
 
                    // Triangle 2
 
                    GS.pos = UnityObjectToClipPos(patch[nextIndex].vertex + float4(0, 0, _Offset, 0));
                    GS.uv = patch[nextIndex].uv;
                    stream.Append(GS);
 
                    GS.pos = UnityObjectToClipPos(patch[nextIndex].vertex);
                    GS.uv = patch[nextIndex].uv;
                    stream.Append(GS);
 
                    GS.pos = UnityObjectToClipPos(patch[index].vertex);
                    GS.uv = patch[index].uv;
                    stream.Append(GS);
 
 
 
                    stream.RestartStrip();
                }
            }
 
            sampler2D _MainTex;
 
            fixed4 frag(v2f i) : COLOR
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                return col;
            }
            ENDCG
        }
    }
}Shader "VoxelSpriteShaderMark1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _Offset ("Offset", float) = 2.0
        _Count ("Count", int) = 2.0
    }
    SubShader
    {
        Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" "IgnoreProjector"="True" }
        Cull Off
        ZWrite On
        AlphaToMask On
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry GSMain
            #pragma fragment frag
            #pragma target 5.0
 
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            uniform float4 _Color;
            float _Offset;
            int _Count;
 
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = v.vertex;
                o.uv = v.uv;
                return o;
            }
 
            [maxvertexcount(60)]
            void GSMain(triangle appdata patch[3], inout TriangleStream<v2f> stream)
            {
                v2f GS;
 
                for(uint i = 0; i < 3; ++i)
                {
                    uint index = i;
                    GS.pos = UnityObjectToClipPos(patch[index].vertex);
                    GS.uv = patch[index].uv;
                    stream.Append(GS);
                }
                stream.RestartStrip();
 
                for(uint i = 0; i < 3; ++i)
                {
                    uint index = i;
                    GS.pos = UnityObjectToClipPos(patch[index].vertex + float4(0, 0, _Offset, 0));
                    GS.uv = patch[index].uv;
                    stream.Append(GS);
                }
                stream.RestartStrip();
 
                for (uint i = 0; i < 3; i++)
                {
                    uint index = i;
                    uint nextIndex = (index + 1) % 3;
 
                    // Triangle 1
 
                    GS.pos = UnityObjectToClipPos(patch[index].vertex);
                    GS.uv = patch[index].uv;
                    stream.Append(GS);
                    GS.pos = UnityObjectToClipPos(patch[index].vertex + float4(0, 0, _Offset, 0));
                    GS.uv = patch[index].uv;
                    stream.Append(GS);
 
                    GS.pos = UnityObjectToClipPos(patch[nextIndex].vertex + float4(0, 0, _Offset, 0));
                    GS.uv = patch[nextIndex].uv;
                    stream.Append(GS);
 
                    stream.RestartStrip();
 
                    // Triangle 2
 
                    GS.pos = UnityObjectToClipPos(patch[nextIndex].vertex + float4(0, 0, _Offset, 0));
                    GS.uv = patch[nextIndex].uv;
                    stream.Append(GS);
 
                    GS.pos = UnityObjectToClipPos(patch[nextIndex].vertex);
                    GS.uv = patch[nextIndex].uv;
                    stream.Append(GS);
 
                    GS.pos = UnityObjectToClipPos(patch[index].vertex);
                    GS.uv = patch[index].uv;
                    stream.Append(GS);
 
 
 
                    stream.RestartStrip();
                }
            }
 
            sampler2D _MainTex;
 
            fixed4 frag(v2f i) : COLOR
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                return col;
            }
            ENDCG
        }
    }
}