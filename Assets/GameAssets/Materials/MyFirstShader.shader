Shader "MyFirstShader"
{

	//定义属性
	Properties
	{
		_Color("Color",color) = (1,1,0,1)
		_BaseMap("Base Map",2D) = "white" {}
		_NormalMap("Normal Map",2D) = "bump" {}
		_Shininess("Shininess",float) = 32
		[Toggle(USE_NORMALMAP)]_UseRed("USE Normal Map",float) = 0
	}
	//一个shader有一个或多个subshader
	SubShader
	{
		

		//pass 块里是工作的地方
		Pass
		{
			//表示要使用哪种语言来编写shader
			HLSLPROGRAM

//预编译指定顶点与像素着色器的入口
#pragma vertex Vertex
#pragma fragment Pixel

//宏定义一个开关 keyword
#pragma multi_compile _ USE_NORMALMAP

			half4 _Color;
			sampler2D _BaseMap;
			sampler2D _NormalMap;
			half _Shininess;

//导入矩阵文件，方便进行转换
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			
			//创建结构体
			//顶点函数 Vertex 模型上所有顶点都会被其处理 vertex为顶点位置 POSITION也叫语义(semantic) 用:将参数vertex与之关联  
			//UV 为贴图坐标，对应语义TEXCOORD0
			struct Attributes
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;  
				#ifdef USE_NORMALMAP 
					float4 tangent : TANGENT;
				#endif
				
			};

			//变体 返回值修饰顶点着色器输出的裁切空间位置也需要与SV_POSITION做关联
			struct Varyings
			{
				float4 positionCS: SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normalWS : NORMAL;
				#ifdef USE_NORMALMAP 
					float4 tangentWS : TANGENT;
				#endif
	
				float3 positionWS : TEXCOORD01; //世界空间的位置
			};

			//顶点着色器 返回值给像素着色器
			Varyings Vertex(Attributes IN)
			{
				//mvp float4 w 传入1 表示一个点 与TransformObjectToHClip函数效果一样
				/*float4 positionWS = mul(UNITY_MATRIX_M,float4(vertex.xyz,1.0));
				float4 positionCS = mul(UNITY_MATRIX_VP, float4(positionWS.xyz, 1.0));*/

				//return positionCS;
				//return TransformObjectToHClip(vertex);

				Varyings OUT;
				OUT.positionCS = TransformObjectToHClip(IN.vertex.xyz);
				OUT.normalWS = TransformObjectToWorldNormal(IN.normal);
				//如果需要法线贴图才计算
				#ifdef USE_NORMALMAP 
					//法线不能直接单向量变换的 所以用TransformObjectToWorldDir
					OUT.tangentWS.xyz = TransformObjectToWorldDir(IN.tangent.xyz).xyz;
					OUT.tangentWS.w = IN.tangent.w;
				#endif
		
				OUT.uv = IN.uv;
				OUT.positionWS = mul(UNITY_MATRIX_M, float4(IN.vertex.xyz, 1.0)).xyz;
				return OUT;
			}
			//像素着色器 (只表现颜色一般half即可) 
			half4 Pixel(Varyings IN) :SV_TARGET
			{
				half4 color;
				Light light = GetMainLight();

				#ifdef USE_NORMALMAP
					// 负切线 bitangent IN.tangentWS.w相当于一个标志，如果模型有镜像使用 w是负数 正好取反
					float3 bitangent = cross(IN.normalWS,IN.tangentWS.xyz) * IN.tangentWS.w;
					// tbn矩阵 float3x3表示 3x3矩阵
					float3x3 TBN = float3x3(IN.tangentWS.xyz,bitangent,IN.normalWS);
					float3 normal = UnpackNormal(tex2D(_NormalMap,IN.uv));
					float3 worldSpaceNormal = TransformTangentToWorld(normal,TBN);
				#else
					float3 worldSpaceNormal = IN.normalWS;
				#endif

				

				
				//float3 normal = UnpackNormal(tex2D(_NormalMap,IN.uv));
				//float3 worldSpaceNormal = TransformObjectToWorldNormal(normal);
				

				//兰伯特经验模型
				//float3 normalWS = normalize(IN.normalWS);
				float NoL = max(0, dot(worldSpaceNormal , /*_MainLightPosition.xyz*/ light.direction)); //_MainLightPostion是平行光朝向， 将法线位置normalWS与灯光点乘
				half3 gi = SampleSH(worldSpaceNormal) * 0.08;// 球谐函数 SampleSH 采样环境低频信息 作为环境光结果

				//Phong经验模型
				// 获取观察方向
				//float3 viewDir = normalize(IN.positionWS - _WorldSpaceCameraPos.xyz );
				 
				
				//blinn-phong模型不需要使用reflect计算反射向量 节省开销 效果差不多
				float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - IN.positionWS);
				float3 hVec = normalize(viewDir + light.direction);
				float spec = max(0, dot( hVec, normalize(worldSpaceNormal)));

				//获得灯光的反射向量
				//float3 reflDir = reflect(light.direction, normalize(worldSpaceNormal));
				//反射向量与灯光向量点乘获得高光加成
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