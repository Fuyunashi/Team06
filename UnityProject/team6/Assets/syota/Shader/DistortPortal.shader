Shader "Custom/Portal"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white"{}
	}

		CGINCLUDE

#include "UnityCG.cginc"

		sampler2D _MainTex;
	sampler2D _SubTex;
	float _Aspect;
	float _Radius;
	float2 _Position;

	float4 frag(v2f_img i) : SV_Target
	{

		float width = 0.09;

	// 自身のピクセルからポータル中心までの距離
	float distance = length((_Position - i.uv) * float2(1, _Aspect));

	// 自身のピクセル位置での歪み具合
	float distortion = 1 - smoothstep(_Radius - width, _Radius, distance);

	// 自身のピクセル位置での歪み具合分だけ
	// ポータル中心の方へずらした uv を計算します
	float2 uv = i.uv + (_Position - i.uv) * distortion;

	return lerp(tex2D(_MainTex,uv),tex2D(_SubTex, i.uv),step(1, distortion));

	}
		ENDCG

		SubShader
	{
		Pass
		{
			CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
			ENDCG
		}
	}
}
