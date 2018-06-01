Shader "Custom/Glass" {
	Properties{
		//キューブテクスチャを受け付けるようにする
		_EnvMap("EnvMap",cube) = "white"{}
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			#pragma surface surf Lambert

			samplerCUBE  _EnvMap;

		struct Input {
			//視線ベクトルがポリゴンに反射したベクトルを取得(UnityのShaderhaデフォルトで計算してくれる)
			float3 worldRefl;
		};


		void surf(Input IN, inout SurfaceOutput o) {
			//キューブテクスチャもテクスチャなので画像からuvを取得
			o.Albedo = texCUBE(_EnvMap, IN.worldRefl).rgb;
			//簡易的なアルファを指定
			o.Alpha = 0.5;
		}
		ENDCG
	}
		FallBack "Diffuse"
}
