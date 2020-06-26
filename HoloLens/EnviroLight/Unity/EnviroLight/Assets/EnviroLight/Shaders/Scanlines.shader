// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:,iptp:1,cusa:True,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:True,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:1873,x:33229,y:32719,varname:node_1873,prsc:2|emission-1749-OUT,alpha-603-OUT;n:type:ShaderForge.SFN_Tex2d,id:4805,x:32551,y:32729,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:True,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1086,x:32812,y:32818,cmnt:RGB,varname:node_1086,prsc:2|A-4938-OUT,B-5376-RGB;n:type:ShaderForge.SFN_Color,id:5983,x:32551,y:32915,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_VertexColor,id:5376,x:32551,y:33079,varname:node_5376,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1749,x:33025,y:32818,cmnt:Premultiply Alpha,varname:node_1749,prsc:2|A-1086-OUT,B-603-OUT;n:type:ShaderForge.SFN_Multiply,id:603,x:32812,y:32992,cmnt:A,varname:node_603,prsc:2|A-4805-A,B-4938-OUT,C-5376-A;n:type:ShaderForge.SFN_Add,id:4938,x:32377,y:32926,varname:node_4938,prsc:2|A-5299-OUT,B-1442-OUT;n:type:ShaderForge.SFN_Multiply,id:5299,x:32113,y:32822,varname:node_5299,prsc:2|A-3128-OUT,B-7027-OUT;n:type:ShaderForge.SFN_Slider,id:7027,x:31752,y:32955,ptovrint:False,ptlb:Scanline Opacity,ptin:_ScanlineOpacity,varname:node_5788,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.25,max:1;n:type:ShaderForge.SFN_Slider,id:1442,x:31752,y:33065,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_3837,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.95,max:1;n:type:ShaderForge.SFN_Power,id:3128,x:31909,y:32679,varname:node_3128,prsc:2|VAL-258-OUT,EXP-3701-OUT;n:type:ShaderForge.SFN_Frac,id:258,x:31739,y:32562,varname:node_258,prsc:2|IN-3713-OUT;n:type:ShaderForge.SFN_ComponentMask,id:3713,x:31486,y:32616,varname:node_3713,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-1034-OUT;n:type:ShaderForge.SFN_OneMinus,id:1034,x:31291,y:32616,varname:node_1034,prsc:2|IN-9889-OUT;n:type:ShaderForge.SFN_Add,id:9889,x:31082,y:32616,varname:node_9889,prsc:2|A-1797-OUT,B-4899-OUT;n:type:ShaderForge.SFN_Multiply,id:1797,x:30867,y:32545,varname:node_1797,prsc:2|A-7573-OUT,B-1108-OUT;n:type:ShaderForge.SFN_Append,id:7573,x:30637,y:32493,varname:node_7573,prsc:2|A-4829-OUT,B-4829-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:1530,x:30161,y:32389,varname:node_1530,prsc:2;n:type:ShaderForge.SFN_Append,id:1108,x:30408,y:32690,varname:node_1108,prsc:2|A-6264-OUT,B-8629-OUT;n:type:ShaderForge.SFN_Slider,id:3701,x:31578,y:32840,ptovrint:False,ptlb:Scanline Exponent,ptin:_ScanlineExponent,varname:node_5553,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:14,max:50;n:type:ShaderForge.SFN_Slider,id:8629,x:30004,y:32799,ptovrint:False,ptlb:Scanline Density,ptin:_ScanlineDensity,varname:node_5487,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8,max:1;n:type:ShaderForge.SFN_Slider,id:8532,x:30307,y:33029,ptovrint:False,ptlb:Scanline Speed,ptin:_ScanlineSpeed,varname:node_3860,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:10;n:type:ShaderForge.SFN_Append,id:4364,x:30689,y:32941,varname:node_4364,prsc:2|A-1133-OUT,B-8532-OUT;n:type:ShaderForge.SFN_Time,id:290,x:30689,y:33093,varname:node_290,prsc:2;n:type:ShaderForge.SFN_Multiply,id:4899,x:30918,y:33032,varname:node_4899,prsc:2|A-4364-OUT,B-290-TSL;n:type:ShaderForge.SFN_Slider,id:564,x:30004,y:32534,ptovrint:False,ptlb:World Multiplier,ptin:_WorldMultiplier,varname:node_564,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.0012,max:0.5;n:type:ShaderForge.SFN_Multiply,id:4829,x:30381,y:32477,varname:node_4829,prsc:2|A-1530-Y,B-564-OUT;n:type:ShaderForge.SFN_Vector1,id:6264,x:30161,y:32690,varname:node_6264,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:1133,x:30464,y:32951,varname:node_1133,prsc:2,v1:0;proporder:4805-5983-7027-1442-3701-8629-8532-564;pass:END;sub:END;*/

Shader "UI/Scanlines" {
    Properties {
        [PerRendererData]_MainTex ("MainTex", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _ScanlineOpacity ("Scanline Opacity", Range(0, 1)) = 0.25
        _Opacity ("Opacity", Range(0, 1)) = 0.95
        _ScanlineExponent ("Scanline Exponent", Range(0, 50)) = 14
        _ScanlineDensity ("Scanline Density", Range(0, 1)) = 0.8
        _ScanlineSpeed ("Scanline Speed", Range(0, 10)) = 2
        _WorldMultiplier ("World Multiplier", Range(0, 0.5)) = 0.0012
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _ScanlineOpacity;
            uniform float _Opacity;
            uniform float _ScanlineExponent;
            uniform float _ScanlineDensity;
            uniform float _ScanlineSpeed;
            uniform float _WorldMultiplier;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float4 vertexColor : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
				
				UNITY_SETUP_INSTANCE_ID(i);
                
				float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float node_4829 = (i.posWorld.g*_WorldMultiplier);
                float4 node_290 = _Time + _TimeEditor;
                float node_4938 = ((pow(frac((1.0 - ((float2(node_4829,node_4829)*float2(1.0,_ScanlineDensity))+(float2(0.0,_ScanlineSpeed)*node_290.r))).g),_ScanlineExponent)*_ScanlineOpacity)+_Opacity);
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float node_603 = (_MainTex_var.a*node_4938*i.vertexColor.a); // A
                float3 emissive = ((node_4938*i.vertexColor.rgb)*node_603);
                float3 finalColor = emissive;
                return fixed4(finalColor,node_603);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
