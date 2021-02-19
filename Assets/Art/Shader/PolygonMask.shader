Shader "Hidden/PolygonMask"
{
	Properties
	{
		_Color("Color ", Color) = (0, 0, 0, 1)
		_Ratio("Ratio", Range(0, 1)) = 0.25 // 屏幕比例
		_Angle("Angle", Range(0, 360)) = 0 // 旋转角
		_VertexNum("VertexNum", Range(3, 50)) = 3
		_Length("Length", Range(0, 1)) = 0.5
		_Smooth ("Smooth", Range(0, 0.5)) = 0
	}
	SubShader
	{
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 _Color;
			float _Ratio;
			float _Length;
			float _Smooth;
			int _VertexNum;
			float _Angle;
			
			fixed4 frag(v2f input) : SV_Target
			{
				float ave = 360 / floor(_VertexNum);
				
				float2 uv = input.uv.xy - float2(0.5, 0.5);
				
				float2 polygoUv = uv;

				polygoUv.x /= _Ratio;
				
				float dis = distance(polygoUv, float2(0, 0));

				float mindis = 1;
	
				for (int i = 0; i < _VertexNum; i++)
				{
					float degress = radians(ave * i + _Angle);
			
					// N边形
					float2 pos = _Length * float2(cos(degress), sin(degress));

					float d = distance(polygoUv, pos);

					mindis = min(mindis, d);
				}

				//_Color.a = step(mindis, dis);

				_Color.a = step(dis, mindis);

				return _Color;
			}
			ENDCG
		}
	}
}