Shader "Custom/TerrainPSX"
{
    Properties
    {
        // Splat Map Control Texture
        [HideInInspector] _Control("Control 1 (RGBA)", 2D) = "red" {}
        //[HideInInspector] _MainTex("MainTex (RGBA)", 2D) = "white" {}

        // Textures
        [HideInInspector] _Splat3("Layer 3 (A)", 2D) = "white" {}
        [HideInInspector] _Splat2("Layer 2 (B)", 2D) = "white" {}
        [HideInInspector] _Splat1("Layer 1 (G)", 2D) = "white" {}
        [HideInInspector] _Splat0("Layer 0 (R)", 2D) = "white" {}

        // Normal Maps
        [HideInInspector] _Normal3("Normal 3 (A)", 2D) = "bump" {}
        [HideInInspector] _Normal2("Normal 2 (B)", 2D) = "bump" {}
        [HideInInspector] _Normal1("Normal 1 (G)", 2D) = "bump" {}
        [HideInInspector] _Normal0("Normal 0 (R)", 2D) = "bump" {}

        _MainTex("Base (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags {
            "SplatCount" = "4"
            "Queue" = "Geometry-100"
            "RenderType" = "Opaque"
        }
        LOD 200
        
        Pass {
            // PSX vert/frag shader
            Lighting On
            CGPROGRAM

            //#pragma debug
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f {
                fixed4 pos : SV_POSITION;
                half4 color : COLOR0;
                half4 colorFog : COLOR1;
                float2 uv_MainTex : TEXCOORD0;
                half3 normal : TEXCOORD1;
                float2 uv_Control : TEXCOORD2;
                float2 uv_Splat0 : TEXCOORD3;
            };

            float4 _MainTex_ST;
            uniform half4 unity_FogStart;
            uniform half4 unity_FogEnd;

            v2f vert(appdata_full v) {
                v2f o;
                //Vertex snapping
                float4 snapToPixel = UnityObjectToClipPos(v.vertex);
                float4 vertex = snapToPixel;
                vertex.xyz = snapToPixel.xyz / snapToPixel.w;
                vertex.x = floor(160 * vertex.x) / 160;
                vertex.y = floor(120 * vertex.y) / 120;
                vertex.xyz *= snapToPixel.w;
                o.pos = vertex;

                //Vertex lighting 
            //	o.color =  float4(ShadeVertexLights(v.vertex, v.normal), 1.0);
                o.color = float4(ShadeVertexLightsFull(v.vertex, v.normal, 4, true), 1.0);
                o.color *= v.color;

                float distance = length(mul(UNITY_MATRIX_MV,v.vertex));

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
                o.colorFog.a = clamp(fogDensity,0,1);

                //Cut out polygons
                if (distance > unity_FogEnd.z + unity_FogColor.a * 255) {
                    o.pos = 0;
                }

                //v.vertex.xyz = o.pos;

                return o;
            }

            //sampler2D _MainTex;
            sampler2D _Control;
            sampler2D _Splat0, _Splat1, _Splat2, _Splat3;

            float4 frag(v2f IN) : COLOR
            {
                fixed4 splat_control = tex2D(_Control, IN.uv_Control / IN.normal.r);
                fixed4 col;
                col.rgb = splat_control.r * tex2D(_Splat0, IN.uv_Splat0).rgb;
                col.rgb += splat_control.g * tex2D(_Splat1, IN.uv_Splat0).rgb;
                col.rgb += splat_control.b * tex2D(_Splat2, IN.uv_Splat0).rgb;
                col.rgb += splat_control.a * tex2D(_Splat3, IN.uv_Splat0).rgb;
                col.a = 1.0;
                return col;
            }

            ENDCG
        }
        /*
        Pass {
            CGPROGRAM

            //#pragma debug
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _Control;
            sampler2D _Splat0, _Splat1, _Splat2, _Splat3;

            struct Input {
                float2 uv_Control : TEXCOORD0;
                float2 uv_Splat0 : TEXCOORD1;
            };

            Input vert(Input IN) {
                return IN;
            }
        
            float3 frag (Input IN) : COLOR {
                // Albedo comes from a texture tinted by color
                fixed4 splat_control = tex2D(_Control, IN.uv_Control);
                fixed3 col;
                col = splat_control.r * tex2D(_Splat0, IN.uv_Splat0).rgb;
                col += splat_control.g * tex2D(_Splat1, IN.uv_Splat0).rgb;
                col += splat_control.b * tex2D(_Splat2, IN.uv_Splat0).rgb;
                col += splat_control.a * tex2D(_Splat3, IN.uv_Splat0).rgb;
                //col.a = 1.0;
                return col;
            }
            ENDCG
        }
            */
        /*
        // Terrain surface shader
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Lambert

        sampler2D _Control;
        sampler2D _Splat0, _Splat1, _Splat2, _Splat3;

        struct Input {
            float2 uv_Control : TEXCOORD0;
            float2 uv_Splat0 : TEXCOORD1;
        };

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input IN, inout SurfaceOutput o) {
            // Albedo comes from a texture tinted by color
            fixed4 splat_control = tex2D(_Control, IN.uv_Control);
            fixed3 col;
            col = splat_control.r * tex2D(_Splat0, IN.uv_Splat0).rgb;
            col += splat_control.g * tex2D(_Splat1, IN.uv_Splat0).rgb;
            col += splat_control.b * tex2D(_Splat2, IN.uv_Splat0).rgb;
            col += splat_control.a * tex2D(_Splat3, IN.uv_Splat0).rgb;
            o.Albedo = col;
            o.Alpha = 0.0;
        }
        ENDCG*/
    }
    Dependency "AddPassShader" = "Custom/TerrainPSXAddPass"

    FallBack "Diffuse"
}
