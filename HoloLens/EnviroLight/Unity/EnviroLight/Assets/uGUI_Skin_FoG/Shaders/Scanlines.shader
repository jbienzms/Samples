Shader "UI/Scanlines"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_LinesSize("LinesSize", Range(1,10)) = 1

		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
			Name "Default"
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				// half4 pos : POSITION;
				// fixed4 sPos : TEXCOORD;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			fixed4 _Color;
			half _LinesSize;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				// Original
				OUT.texcoord = IN.texcoord;

				// New
				// OUT.texcoord = ComputeScreenPos(OUT.vertex); // Or might be worldPosition
				
				// Original
				// OUT.color = IN.color * _Color;
				
				// New
				OUT.color = IN.color;

				// OUT.pos = mul(UNITY_MATRIX_MVP, IN.vertex);
				// OUT.sPos = ComputeScreenPos(OUT.pos);


				return OUT;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
				
				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				
				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				// Original Line
				// pos : POSITION   ->  vertex (SV_POSITION)
				// sPos : TEXCOORD  ->  worldPosition (TEXCOORD1)
				// fixed p = i.sPos.y / i.sPos.w;

				// Attempt 1
				// fixed p = IN.worldPosition.y; //  / IN.worldPosition.z;

				// Attempt 2
				// fixed p = IN.sPos.y / IN.sPos.w;

				// Attempt 3
				fixed4 sPos = ComputeScreenPos(IN.vertex);
				fixed p = sPos.y / sPos.w;

				// if ((int)(p*_ScreenParams.y / floor(_LinesSize)) % 2 == 0) discard;
				if ((int)(p*_ScreenParams.y / floor(_LinesSize)) % 2 == 0)
				{
					return color * _Color;
				}
				else
				{
					return color;
				}

				
			}
		ENDCG
		}
	}
}
