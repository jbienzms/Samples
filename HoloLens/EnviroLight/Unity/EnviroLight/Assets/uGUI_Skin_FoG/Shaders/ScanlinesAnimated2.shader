// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:,iptp:1,cusa:True,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:True,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:1873,x:33393,y:32352,varname:node_1873,prsc:2|emission-1749-OUT,alpha-603-OUT;n:type:ShaderForge.SFN_Tex2d,id:4805,x:32551,y:32729,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:True,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1086,x:32812,y:32818,cmnt:RGB,varname:node_1086,prsc:2|A-4805-RGB,B-5376-RGB;n:type:ShaderForge.SFN_Color,id:5983,x:32540,y:32916,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_VertexColor,id:5376,x:32551,y:33079,varname:node_5376,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1749,x:33025,y:32818,cmnt:Premultiply Alpha,varname:node_1749,prsc:2|A-1086-OUT,B-603-OUT;n:type:ShaderForge.SFN_Multiply,id:603,x:32812,y:32992,cmnt:A,varname:node_603,prsc:2|A-4805-A,B-5376-A;n:type:ShaderForge.SFN_Add,id:1991,x:32874,y:32450,varname:node_1991,prsc:2|A-3667-OUT,B-1284-OUT;n:type:ShaderForge.SFN_Multiply,id:3667,x:32610,y:32346,varname:node_3667,prsc:2|A-4849-OUT,B-1917-OUT;n:type:ShaderForge.SFN_Slider,id:1917,x:32249,y:32479,ptovrint:False,ptlb:Scanline Opacity,ptin:_ScanlineOpacity,varname:node_5788,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Slider,id:1284,x:32249,y:32589,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_3837,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.7264957,max:1;n:type:ShaderForge.SFN_Power,id:4849,x:32406,y:32203,varname:node_4849,prsc:2|VAL-8512-OUT,EXP-8386-OUT;n:type:ShaderForge.SFN_Slider,id:8386,x:32075,y:32364,ptovrint:False,ptlb:Scanline Exponent,ptin:_ScanlineExponent,varname:node_5553,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:15.25641,max:50;n:type:ShaderForge.SFN_Frac,id:8512,x:32236,y:32086,varname:node_8512,prsc:2|IN-4669-OUT;n:type:ShaderForge.SFN_ComponentMask,id:4669,x:31983,y:32140,varname:node_4669,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-6032-OUT;n:type:ShaderForge.SFN_OneMinus,id:6032,x:31788,y:32140,varname:node_6032,prsc:2|IN-6475-OUT;n:type:ShaderForge.SFN_Add,id:6475,x:31579,y:32140,varname:node_6475,prsc:2|A-2686-OUT,B-8893-OUT;n:type:ShaderForge.SFN_Multiply,id:8893,x:31415,y:32556,varname:node_8893,prsc:2|A-7141-OUT,B-8293-TSL;n:type:ShaderForge.SFN_Multiply,id:2686,x:31364,y:32069,varname:node_2686,prsc:2|A-7652-OUT,B-6168-OUT;n:type:ShaderForge.SFN_Append,id:7652,x:31134,y:32017,varname:node_7652,prsc:2|A-6384-Y,B-6384-Y;n:type:ShaderForge.SFN_Append,id:7141,x:31186,y:32465,varname:node_7141,prsc:2|A-7441-OUT,B-9051-OUT;n:type:ShaderForge.SFN_Time,id:8293,x:31186,y:32617,varname:node_8293,prsc:2;n:type:ShaderForge.SFN_Slider,id:9051,x:30804,y:32553,ptovrint:False,ptlb:Scanline Speed 2,ptin:_ScanlineSpeed2,varname:node_3860,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:5.299145,max:10;n:type:ShaderForge.SFN_Slider,id:7441,x:30804,y:32444,ptovrint:False,ptlb:Scanline Speed 1,ptin:_ScanlineSpeed1,varname:node_1558,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:7.179487,max:10;n:type:ShaderForge.SFN_Append,id:6168,x:30905,y:32214,varname:node_6168,prsc:2|A-9571-OUT,B-7641-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:6384,x:30905,y:31963,varname:node_6384,prsc:2;n:type:ShaderForge.SFN_Slider,id:7641,x:30501,y:32323,ptovrint:False,ptlb:Scanline Density,ptin:_ScanlineDensity,varname:node_5487,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:9571,x:30501,y:32207,ptovrint:False,ptlb:Value,ptin:_Value,varname:node_9282,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;proporder:4805-5983-1917-1284-8386-9051-7441-7641-9571;pass:END;sub:END;*/

Shader "UI/ScanlinesAnimated2" {
    Properties {
        [PerRendererData]_MainTex ("MainTex", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _ScanlineOpacity ("Scanline Opacity", Range(0, 1)) = 0.5
        _Opacity ("Opacity", Range(0, 1)) = 0.7264957
        _ScanlineExponent ("Scanline Exponent", Range(0, 50)) = 15.25641
        _ScanlineSpeed2 ("Scanline Speed 2", Range(0, 10)) = 5.299145
        _ScanlineSpeed1 ("Scanline Speed 1", Range(0, 10)) = 7.179487
        _ScanlineDensity ("Scanline Density", Range(0, 1)) = 1
        _Value ("Value", Range(0, 1)) = 1
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
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float node_603 = (_MainTex_var.a*i.vertexColor.a); // A
                float3 emissive = ((_MainTex_var.rgb*i.vertexColor.rgb)*node_603);
                float3 finalColor = emissive;
                return fixed4(finalColor,node_603);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
