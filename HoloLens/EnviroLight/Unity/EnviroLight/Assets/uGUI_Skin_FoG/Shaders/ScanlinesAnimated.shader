// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:33562,y:32702,varname:node_3138,prsc:2|emission-1137-OUT,alpha-1137-OUT;n:type:ShaderForge.SFN_Add,id:1137,x:33234,y:32851,varname:node_1137,prsc:2|A-2010-OUT,B-3837-OUT;n:type:ShaderForge.SFN_Multiply,id:2010,x:32970,y:32747,varname:node_2010,prsc:2|A-3380-OUT,B-5788-OUT;n:type:ShaderForge.SFN_Slider,id:5788,x:32609,y:32880,ptovrint:False,ptlb:Scanline Opacity,ptin:_ScanlineOpacity,varname:node_5788,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Slider,id:3837,x:32609,y:32990,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_3837,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5726496,max:1;n:type:ShaderForge.SFN_Power,id:3380,x:32766,y:32604,varname:node_3380,prsc:2|VAL-4498-OUT,EXP-5553-OUT;n:type:ShaderForge.SFN_Frac,id:4498,x:32596,y:32487,varname:node_4498,prsc:2|IN-2935-OUT;n:type:ShaderForge.SFN_ComponentMask,id:2935,x:32343,y:32541,varname:node_2935,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-2319-OUT;n:type:ShaderForge.SFN_OneMinus,id:2319,x:32148,y:32541,varname:node_2319,prsc:2|IN-6144-OUT;n:type:ShaderForge.SFN_Add,id:6144,x:31939,y:32541,varname:node_6144,prsc:2|A-3241-OUT,B-1154-OUT;n:type:ShaderForge.SFN_Multiply,id:3241,x:31724,y:32470,varname:node_3241,prsc:2|A-250-OUT,B-8578-OUT;n:type:ShaderForge.SFN_Append,id:250,x:31494,y:32418,varname:node_250,prsc:2|A-1212-OUT,B-1212-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:6833,x:30893,y:32354,varname:node_6833,prsc:2;n:type:ShaderForge.SFN_Append,id:8578,x:31265,y:32615,varname:node_8578,prsc:2|A-7815-OUT,B-5487-OUT;n:type:ShaderForge.SFN_Slider,id:5553,x:32435,y:32765,ptovrint:False,ptlb:Scanline Exponent,ptin:_ScanlineExponent,varname:node_5553,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:13.54701,max:50;n:type:ShaderForge.SFN_Slider,id:5487,x:30861,y:32724,ptovrint:False,ptlb:Scanline Density,ptin:_ScanlineDensity,varname:node_5487,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8034188,max:1;n:type:ShaderForge.SFN_Slider,id:3860,x:31164,y:32954,ptovrint:False,ptlb:Scanline Speed,ptin:_ScanlineSpeed,varname:node_3860,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:100;n:type:ShaderForge.SFN_Append,id:863,x:31546,y:32866,varname:node_863,prsc:2|A-3222-OUT,B-3860-OUT;n:type:ShaderForge.SFN_Time,id:8463,x:31546,y:33018,varname:node_8463,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1154,x:31775,y:32957,varname:node_1154,prsc:2|A-863-OUT,B-8463-TSL;n:type:ShaderForge.SFN_Vector1,id:3222,x:31196,y:32857,varname:node_3222,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:7815,x:31018,y:32615,varname:node_7815,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:1212,x:31064,y:32464,varname:node_1212,prsc:2|A-6833-Y,B-1748-OUT;n:type:ShaderForge.SFN_Slider,id:1748,x:30558,y:32555,ptovrint:False,ptlb:World Multiplier,ptin:_WorldMultiplier,varname:node_1748,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2,max:1;proporder:5788-3837-5553-5487-3860-1748;pass:END;sub:END;*/

Shader "UI/ScanlinesAnimated" {
    Properties {
        _ScanlineOpacity ("Scanline Opacity", Range(0, 1)) = 0.5
        _Opacity ("Opacity", Range(0, 1)) = 0.5726496
        _ScanlineExponent ("Scanline Exponent", Range(0, 50)) = 13.54701
        _ScanlineDensity ("Scanline Density", Range(0, 1)) = 0.8034188
        _ScanlineSpeed ("Scanline Speed", Range(0, 100)) = 2
        _WorldMultiplier ("World Multiplier", Range(0, 1)) = 0.2
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float _ScanlineOpacity;
            uniform float _Opacity;
            uniform float _ScanlineExponent;
            uniform float _ScanlineDensity;
            uniform float _ScanlineSpeed;
            uniform float _WorldMultiplier;
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float node_1212 = (i.posWorld.g*_WorldMultiplier);
                float4 node_8463 = _Time + _TimeEditor;
                float node_1137 = ((pow(frac((1.0 - ((float2(node_1212,node_1212)*float2(1.0,_ScanlineDensity))+(float2(0.0,_ScanlineSpeed)*node_8463.r))).g),_ScanlineExponent)*_ScanlineOpacity)+_Opacity);
                float3 emissive = float3(node_1137,node_1137,node_1137);
                float3 finalColor = emissive;
                return fixed4(finalColor,node_1137);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
