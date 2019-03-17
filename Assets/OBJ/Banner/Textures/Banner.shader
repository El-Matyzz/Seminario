// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "New AmplifyShader"
{
	Properties
	{
		_EdgeLength ( "Edge length", Range( 2, 50 ) ) = 40
		_IconTranslation("Icon Translation", Range( 0 , 3)) = 0
		_Apple("Apple", 2D) = "white" {}
		_Castle("Castle", 2D) = "white" {}
		_Lion("Lion", 2D) = "white" {}
		_WaveNoise("Wave Noise", 2D) = "white" {}
		_Border("Border", 2D) = "white" {}
		_InsideColor("Inside Color", Color) = (0,0.751724,1,0)
		_OutsideColor("Outside Color", Color) = (1,0,0,0)
		_BorderColor("Border Color", Color) = (0,0,0,0)
		_WaveSpeed("Wave Speed", Float) = 0
		_WaveDirectionMultiplier("Wave Direction Multiplier", Range( 0 , 2)) = 0
		_NAbywindPosition("NA by wind Position", Range( 0 , 1)) = 0
		_NAbyWindGradient("NA by Wind Gradient", Range( 0 , 1)) = 0.81
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _NAbywindPosition;
		uniform float _NAbyWindGradient;
		uniform sampler2D _WaveNoise;
		uniform float _WaveSpeed;
		uniform float _WaveDirectionMultiplier;
		uniform sampler2D _Border;
		uniform float4 _Border_ST;
		uniform float4 _BorderColor;
		uniform float4 _OutsideColor;
		uniform sampler2D _Apple;
		uniform float4 _Apple_ST;
		uniform float _IconTranslation;
		uniform sampler2D _Castle;
		uniform float4 _Castle_ST;
		uniform sampler2D _Lion;
		uniform float4 _Lion_ST;
		uniform float4 _InsideColor;
		uniform float _EdgeLength;

		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float3 ase_vertex3Pos = v.vertex.xyz;
			float2 appendResult82 = (float2(0.0 , _WaveSpeed));
			float2 panner78 = ( 1.0 * _Time.y * appendResult82 + v.texcoord.xy);
			float4 tex2DNode77 = tex2Dlod( _WaveNoise, float4( panner78, 0, 0.0) );
			v.vertex.xyz += ( ( 1.0 - ( _NAbywindPosition + ( _NAbyWindGradient * ase_vertex3Pos.z ) ) ) * tex2DNode77.r * ( float3(1,0,0) * _WaveDirectionMultiplier ) );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Border = i.uv_texcoord * _Border_ST.xy + _Border_ST.zw;
			float temp_output_68_0 = ( 1.0 - tex2D( _Border, uv_Border ).r );
			float2 uv_Apple = i.uv_texcoord * _Apple_ST.xy + _Apple_ST.zw;
			float lerpResult26 = lerp( 1.0 , tex2D( _Apple, uv_Apple ).r , _IconTranslation);
			float2 uv_Castle = i.uv_texcoord * _Castle_ST.xy + _Castle_ST.zw;
			float lerpResult23 = lerp( lerpResult26 , tex2D( _Castle, uv_Castle ).r , ( _IconTranslation - 1.0 ));
			float2 uv_Lion = i.uv_texcoord * _Lion_ST.xy + _Lion_ST.zw;
			float lerpResult17 = lerp( lerpResult23 , tex2D( _Lion, uv_Lion ).r , ( _IconTranslation - 2.0 ));
			float temp_output_29_0 = saturate( lerpResult17 );
			o.Albedo = ( ( temp_output_68_0 * _BorderColor ) + ( _OutsideColor * ( temp_output_29_0 - temp_output_68_0 ) ) + ( ( 1.0 - temp_output_29_0 ) * _InsideColor ) ).rgb;
			float2 appendResult82 = (float2(0.0 , _WaveSpeed));
			float2 panner78 = ( 1.0 * _Time.y * appendResult82 + i.uv_texcoord);
			float4 tex2DNode77 = tex2D( _WaveNoise, panner78 );
			o.Occlusion = tex2DNode77.r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15301
878;92;720;655;1105.928;1236.962;4.339218;False;False
Node;AmplifyShaderEditor.CommentaryNode;76;-788.8801,-717.2262;Float;False;2096.104;1146.268;Banner Color;29;1;24;26;2;27;23;3;25;17;64;68;29;72;69;41;73;42;71;43;40;18;75;50;20;74;44;22;96;97;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;1;-738.5805,-413.587;Float;True;Property;_Apple;Apple;6;0;Create;True;0;0;False;0;2c7727759a664e44caf2c1c4f8b5fe06;2c7727759a664e44caf2c1c4f8b5fe06;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;24;-715.6856,167.5308;Float;False;Property;_IconTranslation;Icon Translation;5;0;Create;True;0;0;False;0;0;2;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;26;-420.8569,-407.2697;Float;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;27;-414.4797,-121.7088;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-737.7571,-221.4877;Float;True;Property;_Castle;Castle;7;0;Create;True;0;0;False;0;None;ba0ebd90503bdc44c86bafd9edc0a921;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;23;-226.2361,-217.8711;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;25;-215.8943,170.0059;Float;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-736.6725,-27.93476;Float;True;Property;_Lion;Lion;8;0;Create;True;0;0;False;0;None;51da93608d9ca5d4f802d38a416fc186;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;17;-48.58583,-27.08326;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;29;302.1339,-116.8335;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;98;-169.1879,443.8919;Float;False;1478.756;840.4384;Waves;15;89;83;88;82;92;79;90;78;91;77;87;85;93;86;94;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WireNode;41;437.1339,15.16644;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;83;-15.86895,946.1246;Float;False;Property;_WaveSpeed;Wave Speed;14;0;Create;True;0;0;False;0;0;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;64;-738.8801,-667.2262;Float;True;Property;_Border;Border;10;0;Create;True;0;0;False;0;0c97d7fa79d45e546940fcc50dc08fc8;0c97d7fa79d45e546940fcc50dc08fc8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;89;-119.1878,493.8918;Float;False;Property;_NAbyWindGradient;NA by Wind Gradient;17;0;Create;True;0;0;False;0;0.81;0.15;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;42;261.1339,31.16638;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;68;302.6241,-642.2675;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;82;175.1307,926.1246;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PosVertexDataNode;88;-69.71329,568.0342;Float;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;79;83.13096,789.1246;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;92;214.3761,518.0895;Float;False;Property;_NAbywindPosition;NA by wind Position;16;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;69;488.0128,-568.0048;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;40;300.0045,86.05959;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;78;348.1307,852.1246;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;282.3371,591.9786;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;72;257.0312,-569.7204;Float;False;Property;_BorderColor;Border Color;13;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;91;552.3763,568.0895;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;580.1981,-642.6936;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;71;491.6937,-128.9631;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;96;452.3651,148.7191;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;87;450.6501,1169.331;Float;False;Property;_WaveDirectionMultiplier;Wave Direction Multiplier;15;0;Create;True;0;0;False;0;0;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;85;526.5065,1026.245;Float;False;Constant;_WaveDirection;Wave Direction;11;0;Create;True;0;0;False;0;1,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;77;581.682,824.5036;Float;True;Property;_WaveNoise;Wave Noise;9;0;Create;True;0;0;False;0;3e9b0a1a80fa21f4c9144cd923f9a3bc;611322fa830f08a439a04fa2f0521d43;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;733.6498,1068.33;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;93;788.7282,570.054;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;103;1306.557,724.0189;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;97;479.5341,173.7677;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;50;548.4494,-117.1288;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;43;250.0908,162.0416;Float;False;Property;_InsideColor;Inside Color;11;0;Create;True;0;0;False;0;0,0.751724,1,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;75;982.677,-596.1453;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;18;238.1339,-292.8335;Float;False;Property;_OutsideColor;Outside Color;12;0;Create;True;0;0;False;0;1,0,0,0;1,0.616,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;1186.56,828.5587;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;104;1335.763,642.4176;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;840.4426,-144.3206;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;838.8761,151.6472;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;74;1010.908,-521.851;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;22;1071.224,6.110026;Float;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;100;1373.998,784.9488;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;102;1342.031,196.7777;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1424.191,7.449587;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;New AmplifyShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;0;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;40;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;0;0;0;0;False;0;0;0;False;-1;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;26;1;1;1
WireConnection;26;2;24;0
WireConnection;27;0;24;0
WireConnection;23;0;26;0
WireConnection;23;1;2;1
WireConnection;23;2;27;0
WireConnection;25;0;24;0
WireConnection;17;0;23;0
WireConnection;17;1;3;1
WireConnection;17;2;25;0
WireConnection;29;0;17;0
WireConnection;41;0;29;0
WireConnection;42;0;41;0
WireConnection;68;0;64;1
WireConnection;82;1;83;0
WireConnection;69;0;68;0
WireConnection;40;0;42;0
WireConnection;78;0;79;0
WireConnection;78;2;82;0
WireConnection;90;0;89;0
WireConnection;90;1;88;3
WireConnection;91;0;92;0
WireConnection;91;1;90;0
WireConnection;73;0;68;0
WireConnection;73;1;72;0
WireConnection;71;0;69;0
WireConnection;96;0;40;0
WireConnection;77;1;78;0
WireConnection;86;0;85;0
WireConnection;86;1;87;0
WireConnection;93;0;91;0
WireConnection;103;0;77;1
WireConnection;97;0;96;0
WireConnection;50;0;29;0
WireConnection;50;1;71;0
WireConnection;75;0;73;0
WireConnection;94;0;93;0
WireConnection;94;1;77;1
WireConnection;94;2;86;0
WireConnection;104;0;103;0
WireConnection;20;0;18;0
WireConnection;20;1;50;0
WireConnection;44;0;97;0
WireConnection;44;1;43;0
WireConnection;74;0;75;0
WireConnection;22;0;74;0
WireConnection;22;1;20;0
WireConnection;22;2;44;0
WireConnection;100;0;94;0
WireConnection;102;0;104;0
WireConnection;0;0;22;0
WireConnection;0;5;102;0
WireConnection;0;11;100;0
ASEEND*/
//CHKSM=035C822543AE0D9FB77F9DE7ED2E188A763EF4CF