Shader "MyFirstShader"
{

	//��������
	Properties
	{
		_Color("Color",color) = (1,1,0,1)
		_BaseMap("Base Map",2D) = "white" {}
		_Shininess("Shininess",float) = 32
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

			half4 _Color;
			sampler2D _BaseMap;
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
			};

			//���� ����ֵ���ζ�����ɫ������Ĳ��пռ�λ��Ҳ��Ҫ��SV_POSITION������
			struct Varyings
			{
				float4 positionCS: SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normalWS : NORMAL;
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
				OUT.uv = IN.uv;
				OUT.positionWS = mul(UNITY_MATRIX_M, float4(IN.vertex.xyz, 1.0));
				return OUT;
			}
			//������ɫ�� (ֻ������ɫһ��half����) 
			half4 Pixel(Varyings IN) :SV_TARGET
			{
				half4 color;
				Light light = GetMainLight();

				//�����ؾ���ģ��
				float normalWS = normalize(IN.normalWS);
				float NoL = max(0, dot(IN.normalWS , /*_MainLightPosition.xyz*/ light.direction)); //_MainLightPostion��ƽ�й⳯�� ������λ��normalWS��ƹ���
				half3 gi = SampleSH(IN.normalWS) * 0.08;// ��г���� SampleSH ����������Ƶ��Ϣ ��Ϊ��������

				//Phong����ģ��
				// ��ȡ�۲췽��
				//float3 viewDir = normalize(IN.positionWS - _WorldSpaceCameraPos.xyz );
				 
				
				//blinn-phongģ�Ͳ���Ҫʹ��reflect���㷴������ ��ʡ���� Ч�����
				float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - IN.positionWS);
				float3 hVec = normalize(viewDir + light.direction);
				float spec = max(0, dot( hVec, normalize(IN.normalWS)));

				//��õƹ�ķ�������
				//float3 reflDir = reflect(light.direction, normalize(IN.normalWS));
				//����������ƹ�������˻�ø߹�ӳ�
				//float spec = max(0, dot(viewDir, reflDir));
				spec = pow(spec, _Shininess);

				

				color.rgb = tex2D(_BaseMap,IN.uv).rgb * _Color * NoL * /*_MainLightColor.rgb*/light.color + gi + spec;
				color.a = 1.0;
				return color;
			}

			ENDHLSL
		}
	}
}