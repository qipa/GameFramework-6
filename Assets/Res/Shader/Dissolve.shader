Shader "Custom/Dissolve"
{
	Properties
	{
		_MainTex("Base (RGB)",2D) = "white"{}
		_NoiseTex("NoiseTex (R)",2D) = "white"{}
		_Speed("Speed",Float ) = 5
		_EdgeWidth("EdgeWidth",Range(0,0.5)) = 0.1
		_EdgeColor("EdgeColor",Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags{"RenderType" = "Opaque"}
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			uniform sampler2D _MainTex;
			uniform sampler2D _NoiseTex;
			uniform float _Speed;
			uniform float _EdgeWidth;
			uniform float4 _EdgeColor;
			
			float4 frag(v2f_img i) : COLOR
			{
				float factor = saturate(_Time.y / _Speed);
				float noiseValue = tex2D(_NoiseTex,i.uv).r;
				if(noiseValue <= factor)
				{
					discard;
				}
				float4 texColor = tex2D(_MainTex,i.uv);
				float EdgeFactor = saturate((noiseValue - factor)/(_EdgeWidth*factor));
				float4 BlendColor = texColor * _EdgeColor;
				
				return lerp(texColor,BlendColor,1 - EdgeFactor);
			}
			ENDCG
		}
	}
	FallBack Off
}

