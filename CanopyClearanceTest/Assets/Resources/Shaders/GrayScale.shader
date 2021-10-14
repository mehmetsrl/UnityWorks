
Shader "GreyScale" {
	Properties
	{
		_MainTex("Diffuse Textures", 2D) = "white" {}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
#pragma surface surf Lambert

		sampler2D _MainTex;

	struct Input
	{
		float2 uv_MainTex;
		float2 uv_GreyMask;
	};

	void surf(Input IN, inout SurfaceOutput o)
	{
		half4 c = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = (c.r + c.b + c.g) / 3;
		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}