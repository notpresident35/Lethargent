Shader "Custom/TerrainPSXAddPass" {
    Properties{

        // Control Texture ("Splat Map")
        [HideInInspector] _Control("Control (RGBA)", 2D) = "red" {}

        // Terrain textures - each weighted according to the corresponding colour
        // channel in the control texture
        [HideInInspector] _Splat3("Layer 3 (A)", 2D) = "white" {}
        [HideInInspector] _Splat2("Layer 2 (B)", 2D) = "white" {}
        [HideInInspector] _Splat1("Layer 1 (G)", 2D) = "white" {}
        [HideInInspector] _Splat0("Layer 0 (R)", 2D) = "white" {}
    }

        SubShader{
            Tags {
                "SplatCount" = "4"
                "Queue" = "Geometry-99"
                "RenderType" = "Opaque"
                "IgnoreProjector" = "True"
            }

        // TERRAIN PASS 
        CGPROGRAM
        #pragma surface surf Lambert decal:add vertex:vert

        #pragma target 4.0

        #include "UnityCG.cginc"

        struct v2f {
            fixed4 pos : SV_POSITION;
            half4 color : COLOR0;
            half4 colorFog : COLOR1;
            float2 uv_MainTex : TEXCOORD0;
            half3 normal : TEXCOORD1;
        };

        float4 _MainTex_ST;
        uniform half4 unity_FogStart;
        uniform half4 unity_FogEnd;

        v2f vert(inout appdata_full v) {
            v2f o;
            //Vertex snapping
            //float4 snapToPixel = v.vertex;
            float4 vertex = 0;
            vertex.xyz = v.vertex.xyz / v.vertex.w;
            vertex.xyz = floor(1600 * vertex.xyz) / 1600;
            vertex.xyz *= v.vertex.w;
            o.pos = vertex;

            //Vertex lighting 
        //	o.color =  float4(ShadeVertexLights(v.vertex, v.normal), 1.0);
            o.color = float4(ShadeVertexLightsFull(v.vertex, v.normal, 4, true), 1.0);
            o.color *= v.color;

            float distance = length(UnityObjectToClipPos(v.vertex));

            //Affine Texture Mapping
            float4 affinePos = vertex; //vertex;				
            o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
            o.uv_MainTex *= distance + (vertex.w * (UNITY_LIGHTMODEL_AMBIENT.a * 8)) / distance / 2;
            o.normal = distance + (vertex.w * (UNITY_LIGHTMODEL_AMBIENT.a * 8)) / distance / 2;

            //Fog
            float4 fogColor = unity_FogColor;

            float fogDensity = (unity_FogEnd - distance) / (unity_FogEnd - unity_FogStart);
            o.normal.g = fogDensity;
            o.normal.b = 1;

            o.colorFog = fogColor;
            o.colorFog.a = clamp(fogDensity, 0, 1);

            //Cut out polygons
            if (distance > unity_FogEnd.z + unity_FogColor.a * 255) {
                o.pos = 0;
            }

            v.vertex = o.pos;
            //v.normal = o.normal;
            //v.color = o.color * o.colorFog;

            return o;
        }


        // Access the Shaderlab properties
        uniform sampler2D _Control;
        uniform sampler2D _Splat0,_Splat1,_Splat2,_Splat3;

        // Surface shader input structure
        struct Input {
            float2 uv_Control : TEXCOORD0;
            float2 uv_Splat0 : TEXCOORD1;
            float2 uv_Splat1 : TEXCOORD2;
            float2 uv_Splat2 : TEXCOORD3;
            float2 uv_Splat3 : TEXCOORD4;
        };

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        // Surface Shader function
        void surf(Input IN, inout SurfaceOutput o) {
            fixed4 splat_control = tex2D(_Control, IN.uv_Control);
            fixed3 col;
            col = splat_control.r * tex2D(_Splat0, IN.uv_Splat0).rgb;
            col += splat_control.g * tex2D(_Splat1, IN.uv_Splat1).rgb;
            col += splat_control.b * tex2D(_Splat2, IN.uv_Splat2).rgb;
            col += splat_control.a * tex2D(_Splat3, IN.uv_Splat3).rgb;
            o.Albedo = col;
            o.Alpha = 0.0;
        }

        
        ENDCG
    } // End SubShader
    // Fallback to Diffuse
    Fallback "Diffuse"
} // End Shader