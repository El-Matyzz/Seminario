// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Barrel"
{
	Properties
	{
		_Opacity("Opacity", Range( 0 , 1)) = 0
		_Barrel_DefaultMaterial_AlbedoTR("Barrel_DefaultMaterial_AlbedoTR", 2D) = "white" {}
		_Barrel_DefaultMaterial_Normal("Barrel_DefaultMaterial_Normal", 2D) = "bump" {}
		_Barrel_DefaultMaterial_MetallicSmth("Barrel_DefaultMaterial_MetallicSmth", 2D) = "white" {}
		_Barrel_DefaultMaterial_AO("Barrel_DefaultMaterial_AO", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Barrel_DefaultMaterial_Normal;
		uniform float4 _Barrel_DefaultMaterial_Normal_ST;
		uniform sampler2D _Barrel_DefaultMaterial_AlbedoTR;
		uniform float4 _Barrel_DefaultMaterial_AlbedoTR_ST;
		uniform sampler2D _Barrel_DefaultMaterial_MetallicSmth;
		uniform float4 _Barrel_DefaultMaterial_MetallicSmth_ST;
		uniform sampler2D _Barrel_DefaultMaterial_AO;
		uniform float4 _Barrel_DefaultMaterial_AO_ST;
		uniform float _Opacity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Barrel_DefaultMaterial_Normal = i.uv_texcoord * _Barrel_DefaultMaterial_Normal_ST.xy + _Barrel_DefaultMaterial_Normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Barrel_DefaultMaterial_Normal, uv_Barrel_DefaultMaterial_Normal ) );
			float2 uv_Barrel_DefaultMaterial_AlbedoTR = i.uv_texcoord * _Barrel_DefaultMaterial_AlbedoTR_ST.xy + _Barrel_DefaultMaterial_AlbedoTR_ST.zw;
			o.Albedo = tex2D( _Barrel_DefaultMaterial_AlbedoTR, uv_Barrel_DefaultMaterial_AlbedoTR ).rgb;
			float2 uv_Barrel_DefaultMaterial_MetallicSmth = i.uv_texcoord * _Barrel_DefaultMaterial_MetallicSmth_ST.xy + _Barrel_DefaultMaterial_MetallicSmth_ST.zw;
			o.Metallic = tex2D( _Barrel_DefaultMaterial_MetallicSmth, uv_Barrel_DefaultMaterial_MetallicSmth ).r;
			float2 uv_Barrel_DefaultMaterial_AO = i.uv_texcoord * _Barrel_DefaultMaterial_AO_ST.xy + _Barrel_DefaultMaterial_AO_ST.zw;
			o.Occlusion = tex2D( _Barrel_DefaultMaterial_AO, uv_Barrel_DefaultMaterial_AO ).r;
			o.Alpha = _Opacity;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

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
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
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
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15301
1030;92;568;656;536.8629;342.6573;1.3;False;False
Node;AmplifyShaderEditor.RangedFloatNode;1;-303,337.5;Float;False;Property;_Opacity;Opacity;1;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-589.2233,-445.928;Float;True;Property;_Barrel_DefaultMaterial_AlbedoTR;Barrel_DefaultMaterial_AlbedoTR;2;0;Create;True;0;0;False;0;4d4133f269c80f1419bb3e195e6784c9;4d4133f269c80f1419bb3e195e6784c9;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;5;-542.0629,131.8426;Float;True;Property;_Barrel_DefaultMaterial_AO;Barrel_DefaultMaterial_AO;5;0;Create;True;0;0;False;0;971fdfdc45fae7c4b977f93873e1ec4e;971fdfdc45fae7c4b977f93873e1ec4e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-572.2233,-251.9281;Float;True;Property;_Barrel_DefaultMaterial_Normal;Barrel_DefaultMaterial_Normal;3;0;Create;True;0;0;False;0;53bf4de43fec37e4c87174f65a4c9563;53bf4de43fec37e4c87174f65a4c9563;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-607.2233,-62.92804;Float;True;Property;_Barrel_DefaultMaterial_MetallicSmth;Barrel_DefaultMaterial_MetallicSmth;4;0;Create;True;0;0;False;0;7dd27db9f5562e14bb9828c5f7356b7f;7dd27db9f5562e14bb9828c5f7356b7f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Barrel;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;0;False;0;Custom;0.5;True;True;0;True;Transparent;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;False;0;0;0;False;-1;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;0;0;2;0
WireConnection;0;1;3;0
WireConnection;0;3;4;1
WireConnection;0;5;5;1
WireConnection;0;9;1;0
ASEEND*/
//CHKSM=B8B29390484CBD04ABE97FED8B7DF820B7FEA418