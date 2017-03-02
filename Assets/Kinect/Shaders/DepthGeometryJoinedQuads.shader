Shader "Custom/DepthGeometryJoinedQuads" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_WorldScale ("WorldScale", Float) = 1.0
	}

	SubShader 
	{
		Pass
		{
			Tags { "RenderType"="Opaque" }
			LOD 200
		
			CGPROGRAM
				#pragma target 5.0
				#include "UnityCG.cginc" 
				#include "KinectCommon.cginc"
				#pragma vertex VS_Empty
				#pragma geometry GS_Main			
				#pragma fragment FS_Passthrough

				float _WorldScale;
				Texture2D _MainTex;
				
				[maxvertexcount(4)]
				void GS_Main(point EMPTY_INPUT p[1], uint primID : SV_PrimitiveID, inout TriangleStream<POSCOLOR_INPUT> triStream)
				{
					POSCOLOR_INPUT output;							
				
				    int3 textureCoordinates = int3(primID % DepthWidth, primID / DepthWidth, 0);	
											
					float4 depths;
					depths[0] = DepthFromPacked4444(_MainTex.Load(textureCoordinates + textureOffsets4Samples[0]));
					depths[1] = DepthFromPacked4444(_MainTex.Load(textureCoordinates + textureOffsets4Samples[1]));
					depths[2] = DepthFromPacked4444(_MainTex.Load(textureCoordinates + textureOffsets4Samples[2]));
					depths[3] = DepthFromPacked4444(_MainTex.Load(textureCoordinates + textureOffsets4Samples[3]));
					
					// don't output quads for pixels with invalid depth data
					if (depths[0] < MinDepthMM || depths[0] > MaxDepthMM)
					{
						return;
					}									
									
					float4 avgDepth = (depths[0] + depths[1] + depths[2] + depths[3]) * 0.25; 
					
					// test the difference between each of the depth values and the average
					// if any deviate by the cutoff or more, don't generate this quad
					static const float joinCutoff = 10.0;
					float4 depthDev = abs(depths - avgDepth);
					float4 branch = step(joinCutoff, depthDev);

					if ( any(branch) )
					{
						return;
					}
									
					// convert to meters and scale to world						
					float4 worldScaledDepths = depths * MillimetersToMetersScale * 1000;				
					
					// color based on depth
					output.color.rgb = 1.0 - ((depths[0] - MinDepthMM) / (MaxDepthMM - MinDepthMM));
					output.color.a = 1.0;					
				
					float4 worldPos = float4(0.0, 0.0, 0.0, 1.0);

					for (int i = 0; i < 4; ++i)
					{
						worldPos.xy = float2(textureCoordinates.x, DepthHeight - textureCoordinates.y);
						
						worldPos += quadOffsets[i];

						// Center coordinates such that 0,0 is the center in the world					
						worldPos.xy -= DepthHalfWidthHeight;			

						// Data is projected onto the 2D sensor, but knowing the depth and field of view we can reconstruct world space position
						worldPos.xy *= XYSpread * worldScaledDepths[i];					
						
						worldPos.z = worldScaledDepths[i];
							 
						output.pos = mul(UNITY_MATRIX_VP, worldPos);
						triStream.Append(output);			
					}
				}

			ENDCG
		}
	} 
}
