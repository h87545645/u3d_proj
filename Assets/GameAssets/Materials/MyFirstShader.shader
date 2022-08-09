Shader "MyFirstShader"
{

	//定义属性
	Properties
	{
		_Color("Color",color) = (1,1,0,1)
		_BaseMap("Base Map",2D) = "white" {}
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

			half4 _Color;
			sampler2D _BaseMap;

//导入矩阵文件，方便进行转换
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			
			//创建结构体
			//顶点函数 Vertex 模型上所有顶点都会被其处理 vertex为顶点位置 POSITION也叫语义(semantic) 用:将参数vertex与之关联  
			//UV 为贴图坐标，对应语义TEXCOORD0
			struct Attributes
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			//变体 返回值修饰顶点着色器输出的裁切空间位置也需要与SV_POSITION做关联
			struct Varyings
			{
				float4 positionCS: SV_POSITION;
				float2 uv : TEXCOORD0;
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
				OUT.uv = IN.uv;
				return OUT;
			}
			//像素着色器
			half4 Pixel(Varyings IN) :SV_TARGET
			{
				half4 color;
				color.rgb = tex2D(_BaseMap,IN.uv).rgb * _Color;
				color.a = 1.0;
				return color;
			}

			ENDHLSL
		}
	}
}