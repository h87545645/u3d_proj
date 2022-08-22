Shader "MyFirstShader"
{

	//��������
	Properties
	{
		_Color("Color",color) = (1,1,0,1)
		_BaseMap("Base Map",2D) = "white" {}
		_NormalMap("Normal Map",2D) = "bump" {}
		_Shininess("Shininess",float) = 32
		[Toggle(USE_NORMALMAP)]_UseRed("USE Normal Map",float) = 0
	}
	//һ��shader��һ������subshader
	SubShader
	{
		

		//pass �����ǹ����ĵط�
		Pass
		{
			//��ʾҪʹ��������������дshader
			HLSLPROGRAM

//Ԥ����ָ��������������ɫ�������
#pragma vertex Vertex
#pragma fragment Pixel

//�궨��һ������ keyword
#pragma multi_compile _ USE_NORMALMAP

			half4 _Color;
			sampler2D _BaseMap;
			sampler2D _NormalMap;
			half _Shininess;

//��������ļ����������ת��
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			
			//�����ṹ��
			//���㺯�� Vertex ģ�������ж��㶼�ᱻ�䴦�� vertexΪ����λ�� POSITIONҲ������(semantic) ��:������vertex��֮����  
			//UV Ϊ��ͼ���꣬��Ӧ����TEXCOORD0
			struct Attributes
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;  
				#ifdef USE_NORMALMAP 
					float4 tangent : TANGENT;
				#endif
				
			};

			//���� ����ֵ���ζ�����ɫ������Ĳ��пռ�λ��Ҳ��Ҫ��SV_POSITION������
			struct Varyings
			{
				float4 positionCS: SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normalWS : NORMAL;
				#ifdef USE_NORMALMAP 
					float4 tangentWS : TANGENT;
				#endif
	
				float3 positionWS : TEXCOORD01; //����ռ��λ��
			};

			//������ɫ�� ����ֵ��������ɫ��
			Varyings Vertex(Attributes IN)
			{
				//mvp float4 w ����1 ��ʾһ���� ��TransformObjectToHClip����Ч��һ��
				/*float4 positionWS = mul(UNITY_MATRIX_M,float4(vertex.xyz,1.0));
				float4 positionCS = mul(UNITY_MATRIX_VP, float4(positionWS.xyz, 1.0));*/

				//return positionCS;
				//return TransformObjectToHClip(vertex);

				Varyings OUT;
				OUT.positionCS = TransformObjectToHClip(IN.vertex.xyz);
				OUT.normalWS = TransformObjectToWorldNormal(IN.normal);
				//�����Ҫ������ͼ�ż���
				#ifdef USE_NORMALMAP 
					//���߲���ֱ�ӵ������任�� ������TransformObjectToWorldDir
					OUT.tangentWS.xyz = TransformObjectToWorldDir(IN.tangent.xyz).xyz;
					OUT.tangentWS.w = IN.tangent.w;
				#endif
		
				OUT.uv = IN.uv;
				OUT.positionWS = mul(UNITY_MATRIX_M, float4(IN.vertex.xyz, 1.0)).xyz;
				return OUT;
			}
			//������ɫ�� (ֻ������ɫһ��half����) 
			half4 Pixel(Varyings IN) :SV_TARGET
			{
				half4 color;
				Light light = GetMainLight();

				#ifdef USE_NORMALMAP
					// ������ bitangent IN.tangentWS.w�൱��һ����־�����ģ���о���ʹ�� w�Ǹ��� ����ȡ��
					float3 bitangent = cross(IN.normalWS,IN.tangentWS.xyz) * IN.tangentWS.w;
					// tbn���� float3x3��ʾ 3x3����
					float3x3 TBN = float3x3(IN.tangentWS.xyz,bitangent,IN.normalWS);
					float3 normal = UnpackNormal(tex2D(_NormalMap,IN.uv));
					float3 worldSpaceNormal = TransformTangentToWorld(normal,TBN);
				#else
					float3 worldSpaceNormal = IN.normalWS;
				#endif

				

				
				//float3 normal = UnpackNormal(tex2D(_NormalMap,IN.uv));
				//float3 worldSpaceNormal = TransformObjectToWorldNormal(normal);
				

				//�����ؾ���ģ��
				//float3 normalWS = normalize(IN.normalWS);
				float NoL = max(0, dot(worldSpaceNormal , /*_MainLightPosition.xyz*/ light.direction)); //_MainLightPostion��ƽ�й⳯�� ������λ��normalWS��ƹ���
				half3 gi = SampleSH(worldSpaceNormal) * 0.08;// ��г���� SampleSH ����������Ƶ��Ϣ ��Ϊ��������

				//Phong����ģ��
				// ��ȡ�۲췽��
				//float3 viewDir = normalize(IN.positionWS - _WorldSpaceCameraPos.xyz );
				 
				
				//blinn-phongģ�Ͳ���Ҫʹ��reflect���㷴������ ��ʡ���� Ч�����
				float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - IN.positionWS);
				float3 hVec = normalize(viewDir + light.direction);
				float spec = max(0, dot( hVec, normalize(worldSpaceNormal)));

				//��õƹ�ķ�������
				//float3 reflDir = reflect(light.direction, normalize(worldSpaceNormal));
				//����������ƹ�������˻�ø߹�ӳ�
				//float spec = max(0, dot(viewDir, reflDir));
				spec = pow(spec, _Shininess);

				

				color.rgb = tex2D(_BaseMap,IN.uv).rgb * _Color.rgb * NoL * /*_MainLightColor.rgb*/light.color + gi + spec;

				//#ifdef USE_RED
				//	color.rgb = half3(1,0,0);
				//#else
				//	color.rgb = half3(0,1,0);
				//#endif

				color.a = 1.0;
				return color;
			}

			ENDHLSL
		}
	}
}