// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/Concrete"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Asphalt("Asphalt", 2D) = "white" {}
		_Mask("Mask", 2D) = "white" {}
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		_Asphalt_normal("Asphalt_normal", 2D) = "bump" {}
		_Albedocolor("Albedo color", Color) = (0,0,0,0)
		_Runwaymask("Runway mask", 2D) = "white" {}
		_TextureSample3("Texture Sample 3", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 texcoord_0;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
		};

		uniform sampler2D _Asphalt_normal;
		uniform sampler2D _Runwaymask;
		uniform float4 _Runwaymask_ST;
		uniform float4 _Albedocolor;
		uniform sampler2D _Asphalt;
		uniform float4 _Asphalt_ST;
		uniform sampler2D _TextureSample1;
		uniform float4 _TextureSample1_ST;
		uniform sampler2D _Mask;
		uniform float4 _Mask_ST;
		uniform sampler2D _TextureSample3;
		uniform float4 _TextureSample3_ST;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 temp_cast_0 = 100.0;
			o.texcoord_0.xy = v.texcoord.xy * temp_cast_0 + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 _Vector0 = float3(0,0,1);
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float2 uv_Runwaymask = i.uv_texcoord * _Runwaymask_ST.xy + _Runwaymask_ST.zw;
			o.Normal = lerp( lerp( UnpackNormal( tex2D( _Asphalt_normal,i.texcoord_0) ) , _Vector0 , dot( worldViewDir , WorldNormalVector( i, float3(0,0,1) ) ) ) , _Vector0 , tex2D( _Runwaymask,uv_Runwaymask).x );
			float2 uv_Asphalt = i.uv_texcoord * _Asphalt_ST.xy + _Asphalt_ST.zw;
			float4 tex2DNode1 = tex2D( _Asphalt,uv_Asphalt);
			float2 uv_TextureSample1 = i.uv_texcoord * _TextureSample1_ST.xy + _TextureSample1_ST.zw;
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			o.Albedo = ( saturate( ( tex2DNode1 > 0.5 ? ( 1.0 - ( 1.0 - 2.0 * ( tex2DNode1 - 0.5 ) ) * ( 1.0 - ( _Albedocolor * ( saturate( ( tex2D( _Mask,uv_Mask) > 0.5 ? ( 1.0 - ( 1.0 - 2.0 * ( tex2D( _Mask,uv_Mask) - 0.5 ) ) * ( 1.0 - lerp( tex2DNode1 , tex2Dlod( _TextureSample1,float4( uv_TextureSample1, 0, 1.0)) , dot( worldViewDir , WorldNormalVector( i, float3(0,0,1) ) ) ) ) ) : ( 2.0 * tex2D( _Mask,uv_Mask) * lerp( tex2DNode1 , tex2Dlod( _TextureSample1,float4( uv_TextureSample1, 0, 1.0)) , dot( worldViewDir , WorldNormalVector( i, float3(0,0,1) ) ) ) ) ) )) ) ) ) : ( 2.0 * tex2DNode1 * ( _Albedocolor * ( saturate( ( tex2D( _Mask,uv_Mask) > 0.5 ? ( 1.0 - ( 1.0 - 2.0 * ( tex2D( _Mask,uv_Mask) - 0.5 ) ) * ( 1.0 - lerp( tex2DNode1 , tex2Dlod( _TextureSample1,float4( uv_TextureSample1, 0, 1.0)) , dot( worldViewDir , WorldNormalVector( i, float3(0,0,1) ) ) ) ) ) : ( 2.0 * tex2D( _Mask,uv_Mask) * lerp( tex2DNode1 , tex2Dlod( _TextureSample1,float4( uv_TextureSample1, 0, 1.0)) , dot( worldViewDir , WorldNormalVector( i, float3(0,0,1) ) ) ) ) ) )) ) ) ) )).rgb;
			o.Metallic = 0.0;
			float4 tex2DNode40 = tex2D( _Runwaymask,uv_Runwaymask);
			float4 temp_cast_5 = 0.35;
			float2 uv_TextureSample3 = i.uv_texcoord * _TextureSample3_ST.xy + _TextureSample3_ST.zw;
			o.Smoothness = ( saturate( ( 0.5 - 2.0 * ( ( 1.0 * ( tex2DNode40 + temp_cast_5 ) ) - 0.5 ) * ( ( tex2D( _TextureSample3,uv_TextureSample3) * pow( ( 1.0 - tex2DNode40 ) , 2.0 ) ) - 0.5 ) ) )).r;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			# include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD6;
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
				float4 texcoords01 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.texcoords01 = float4( v.texcoord.xy, v.texcoord1.xy );
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.texcoords01.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=6001
2567;29;1666;974;1745.751;972.5399;3.1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;9;-468.5,507;Float;False;Constant;_Float1;Float 1;3;0;1;0;0;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;18;-452.4006,751.7999;Float;False;0;FLOAT3;0,0,0;False;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;20;-425.9028,613.9995;Float;False;World;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;34;-717.4023,1134.149;Float;False;Constant;_Float3;Float 3;4;0;100;0;0;FLOAT
Node;AmplifyShaderEditor.SamplerNode;40;227.5354,516.5762;Float;True;Property;_Runwaymask;Runway mask;5;0;Assets/Runway mask.tga;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;8;-233.5,473;Float;True;Property;_TextureSample1;Texture Sample 1;2;0;Assets/Concrete 2.tga;True;0;False;white;Auto;False;Object;-1;MipLevel;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DotProductOpNode;19;2.199407,727.6005;Float;True;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-508,-179;Float;True;Property;_Asphalt;Asphalt;0;0;Assets/Asphalt.tga;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;33;-489.6025,1146.05;Float;False;0;-1;2;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;48;1204.747,952.7598;Float;False;0;FLOAT4;0.0;False;FLOAT4
Node;AmplifyShaderEditor.LerpOp;24;48.49727,202.7;Float;False;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0;False;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;2;162.8003,352.3998;Float;True;Property;_Mask;Mask;1;0;Assets/Mask.tga;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;43;318.8358,653.076;Float;False;Constant;_Float5;Float 5;6;0;0.35;0;0;FLOAT
Node;AmplifyShaderEditor.Vector3Node;35;-208.595,837.7679;Float;True;Constant;_Vector0;Vector 0;4;0;0,0,1;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;51;1321.549,624.8599;Float;True;Property;_TextureSample3;Texture Sample 3;6;0;Assets/Grunge map.tga;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;25;454.9968,276.2997;Float;False;Constant;_Float0;Float 0;3;0;1;0;0;FLOAT
Node;AmplifyShaderEditor.BlendOpsNode;3;332.2,153;Float;False;Overlay;True;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;COLOR
Node;AmplifyShaderEditor.ColorNode;38;307.3358,-33.62388;Float;False;Property;_Albedocolor;Albedo color;4;0;0,0,0,0;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;42;904.2357,307.3755;Float;False;0;FLOAT4;0.0;False;1;FLOAT;0.0,0,0,0;False;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;26;-178.5029,1107.799;Float;True;Property;_Asphalt_normal;Asphalt_normal;3;0;Assets/Asphalt_normal.tga;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PowerNode;50;1360.548,976.4597;Float;True;0;FLOAT4;0.0;False;1;FLOAT;2.0;False;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;1709.549,857.7598;Float;True;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.0,0,0,0;False;FLOAT4
Node;AmplifyShaderEditor.LerpOp;27;446.628,819.3915;Float;False;0;FLOAT3;0.0;False;1;FLOAT3;0.0,0,0;False;2;FLOAT;0.0;False;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;551.7359,124.9761;Float;False;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;1237.335,190.276;Float;True;0;FLOAT;0.0,0,0,0;False;1;FLOAT4;0.0;False;FLOAT4
Node;AmplifyShaderEditor.BlendOpsNode;52;1671.248,199.2598;Float;False;Exclusion;True;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;32;-759.9027,1320.298;Float;False;Constant;_Float2;Float 2;4;0;10;0;0;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;2006.946,724.0591;Float;False;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.0;False;FLOAT4
Node;AmplifyShaderEditor.LerpOp;44;650.1403,826.8254;Float;False;0;FLOAT3;0.0;False;1;FLOAT3;0.0,0,0;False;2;FLOAT4;0.0;False;FLOAT3
Node;AmplifyShaderEditor.BlendOpsNode;39;809.0359,18.07613;Float;False;Overlay;True;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;COLOR
Node;AmplifyShaderEditor.SamplerNode;29;-181.9027,1313.498;Float;True;Property;_TextureSample2;Texture Sample 2;3;0;Assets/Asphalt_normal.tga;True;0;True;bump;Auto;True;Instance;26;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;45;-63.45071,-596.4164;Float;True;Property;_Grungemap;Grunge map;6;0;Assets/Grunge map.tga;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;31;-532.1029,1332.199;Float;False;0;-1;2;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;36;1094.797,488.8994;Float;False;Constant;_Float4;Float 4;3;0;0;0;0;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2005.923,177.3022;Float;False;True;2;Float;ASEMaterialInspector;Standard;Custom/Concrete;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;OBJECT;0.0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False
WireConnection;8;2;9;0
WireConnection;19;0;20;0
WireConnection;19;1;18;0
WireConnection;33;0;34;0
WireConnection;48;0;40;0
WireConnection;24;0;1;0
WireConnection;24;1;8;0
WireConnection;24;2;19;0
WireConnection;3;0;24;0
WireConnection;3;1;2;0
WireConnection;42;0;40;0
WireConnection;42;1;43;0
WireConnection;26;1;33;0
WireConnection;50;0;48;0
WireConnection;47;0;51;0
WireConnection;47;1;50;0
WireConnection;27;0;26;0
WireConnection;27;1;35;0
WireConnection;27;2;19;0
WireConnection;37;0;38;0
WireConnection;37;1;3;0
WireConnection;41;0;25;0
WireConnection;41;1;42;0
WireConnection;52;0;41;0
WireConnection;52;1;47;0
WireConnection;46;0;47;0
WireConnection;46;1;41;0
WireConnection;44;0;27;0
WireConnection;44;1;35;0
WireConnection;44;2;40;0
WireConnection;39;0;37;0
WireConnection;39;1;1;0
WireConnection;29;1;31;0
WireConnection;31;0;32;0
WireConnection;0;0;39;0
WireConnection;0;1;44;0
WireConnection;0;3;36;0
WireConnection;0;4;52;0
ASEEND*/
//CHKSM=FB72CD7B724E48A4C633A16C4585ED90CCB38B44