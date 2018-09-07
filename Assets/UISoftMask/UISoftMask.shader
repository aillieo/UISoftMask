// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "AillieoUtils/UISoftMask"     
{     
    Properties     
    {     
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)


		// masking texture
		_SoftMaskTex ("Soft Mask Texture", 2D) = "white" {}


        // required for UI.Mask
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15

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

        Blend SrcAlpha OneMinusSrcAlpha  
        ZWrite Off
        
        // required for UI.Mask
        Stencil
        {
             Ref [_Stencil]
             Comp [_StencilComp]
             Pass [_StencilOp] 
             ReadMask [_StencilReadMask]
             WriteMask [_StencilWriteMask]
        }
        ColorMask [_ColorMask]

        Pass     
        {     
            CGPROGRAM     
            #pragma vertex vert     
            #pragma fragment frag     
            #include "UnityCG.cginc"     
                 
            struct appdata_t     
            {     
                float4 vertex   : POSITION;     
                float4 color    : COLOR;     
                float2 texcoord : TEXCOORD0;
            };     
     
            struct v2f     
            {     
                float4 vertex   : SV_POSITION;     
                fixed4 color    : COLOR;     
                half2 texcoord  : TEXCOORD0;
				half2 texcoord_uv1 : TEXCOORD1;
            };     
               
            sampler2D _MainTex;
            fixed4 _Color;


			// masking parameters
            sampler2D _SoftMaskTex;
			float4 _SoftMaskRect;
			float4x4 _SoftMaskTrans;
	
	
			half2 get_uv1(float4 vert)							// get uv in soft mask texture
			{
				float4 pos = float4(0,0,0,0);					// pivot  local space
				pos = UnityObjectToClipPos(pos);				// pivot  world space
				pos = mul(_SoftMaskTrans, pos);					// pivot  maskObj space
				pos = vert + pos;								// vert   maskObj space
				pos.x /= _SoftMaskRect.x;
				pos.y /= _SoftMaskRect.y;						// uv form pivot
				pos.x += _SoftMaskRect.z;
				pos.y += _SoftMaskRect.w;						// uv from LB
				return pos.xy;
			}
            
            v2f vert(appdata_t IN)     
            {
                v2f OUT;     
                OUT.vertex = UnityObjectToClipPos(IN.vertex);     
                OUT.texcoord = IN.texcoord;   
                OUT.texcoord_uv1 = get_uv1(IN.vertex);		// uv in mask texture
#ifdef UNITY_HALF_TEXEL_OFFSET     
                OUT.vertex.xy -= (_ScreenParams.zw-1.0);     
#endif     
                OUT.color = IN.color * _Color;     
                return OUT;  
            }  
     
            fixed4 frag(v2f IN) : SV_Target     
            {     
				fixed4 finalTex = tex2D(_MainTex, IN.texcoord);
                fixed4 alphaTex = tex2D(_SoftMaskTex,IN.texcoord_uv1);		// alpha from soft mask texture

                return alphaTex.a * finalTex;

            }     
            ENDCG     
        }     
    }     
}