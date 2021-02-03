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
        #pragma surface surf Lambert decal:add

        #pragma target 4.0

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