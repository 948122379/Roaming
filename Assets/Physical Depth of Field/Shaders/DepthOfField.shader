// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/DepthOfField"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}

		CGINCLUDE
		#include "UnityCG.cginc"
		uniform float4 _MainTex_TexelSize;
		#pragma target 3.0

	struct VertexInput
	{
		float4 pos : POSITION;
		float2 texCoords : TEXCOORD0;
	};

	struct PixelInput
	{
		float4 pos : POSITION;
		float2 texCoords : TEXCOORD0;
		float2 texCoords1 : TEXCOORD1;
	};

	PixelInput vert(VertexInput v)
	{
		PixelInput o;
		o.pos = UnityObjectToClipPos(v.pos);
		o.texCoords = v.texCoords;
#if UNITY_UV_STARTS_AT_TOP 
		o.texCoords1 = v.texCoords;
		if (_MainTex_TexelSize.y < 0)
			o.texCoords.y = 1 - v.texCoords.y;
#else

#endif

		return o;
	}

	struct fragBlurOutput
	{
		float4 nearResult : COLOR0;
		float4 blurResult : COLOR1;
	};

	uniform sampler2D _MainTex;
	sampler2D _CameraDepthTexture;
	float4 _CameraDepthTexture_ST;

#if PHYSICAL
	uniform float       focusPlaneZ;
	uniform float       scale;
#endif
#if ARTIST
	uniform float       nearBlurryPlaneZ;
	uniform float       nearSharpPlaneZ;
	uniform float       farSharpPlaneZ;
	uniform float       farBlurryPlaneZ;

	/** Scales the near-field z distance to the fraction of the maximum
	blur radius in either field. */
	uniform float       nearScale;

	/** Scales the far-field z distance to the fraction of the maximum
	blur radius in either field. */
	uniform float       farScale;
#endif

	/** Maximum blur radius for any point in the scene, in pixels.  Used to
	reconstruct the CoC radius from the normalized CoC radius. */
	uniform int         maxCoCRadiusPixels;

	/** Source image in RGB, normalized CoC in A. */
	//uniform sampler2D	_MainTex;

	uniform float         nearBlurRadiusPixels;
	uniform float       invNearBlurRadiusPixels;

	uniform float2		writeScaleBias;
	uniform float       coverageBoost;

	uniform float2 direction;

#if HORIZONTAL
//#define HORIZONTAL 1
#endif
#if VERTICAL
//#define VERTICAL 1
#endif


#if VERTICAL
	/** For the second pass, the output of the previous near-field blur pass. */
	uniform sampler2D  nearSourceBuffer;
#endif


	// Boost the coverage of the near field by this factor.  Should always be >= 1
	//
	// Make this larger if near-field objects seem too transparent
	//
	// Make this smaller if an obvious line is visible between the near-field blur and the mid-field sharp region
	// when looking at a textured ground plane.

	uniform float	farRadiusRescale;

	uniform float2    packedBufferInvSize;
	uniform sampler2D blurBuffer;
	uniform sampler2D nearBuffer;

	float4 fragCoC(const PixelInput i) : COLOR
	{
#if UNITY_UV_STARTS_AT_TOP 
		float3 color = tex2D(_MainTex, i.texCoords1).rgb;
#else
		float3 color = tex2D(_MainTex, i.texCoords).rgb;
#endif

		float radius = 0.0;

		float z = -LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.texCoords));

#if PHYSICAL
		radius = -((z - focusPlaneZ) * scale);
#endif
#if ARTIST
		if (z > nearSharpPlaneZ)
		{
			radius = (min(z, nearBlurryPlaneZ) - nearSharpPlaneZ) * nearScale;
		}
		else if (z > farSharpPlaneZ)
		{
			// In the focus field
			radius = 0.0;
		}
		else
		{
			// Produce a negative value
			radius = (max(z, farBlurryPlaneZ) - farSharpPlaneZ) * farScale;
		}

#endif
#if NONE
		radius = 0.0;
#endif
		return float4(color, radius * writeScaleBias.x + writeScaleBias.y);
	}

		bool inNearField(float radiusPixels)
	{
		return radiusPixels > 0.25;
	}

	fragBlurOutput fragBlur(const PixelInput i)
	{
		fragBlurOutput output;
		const int KERNEL_TAPS = 6;
		float kernel[KERNEL_TAPS + 1];

		// 11 x 11 separated kernel weights.  This does not dictate the 
		// blur kernel for depth of field; it is scaled to the actual
		// kernel at each pixel.
		//    Custom disk-like // vs. Gaussian
		kernel[6] = 0.00; //0.00000000000000;  // Weight applied to outside-radius values
		kernel[5] = 0.50;//0.04153263993208;
		kernel[4] = 0.60;//0.06352050813141;
		kernel[3] = 0.75;//0.08822292796029;
		kernel[2] = 0.90;//0.11143948794984;
		kernel[1] = 1.00;//0.12815541114232;
		kernel[0] = 1.00;//0.13425804976814;

		// Accumulate the blurry image color
		output.blurResult.rgb = float3(0, 0, 0);
		float blurWeightSum = 0.0f;

		// Accumulate the near-field color and coverage
		output.nearResult = float4(0, 0, 0, 0);
		float nearWeightSum = 0.000f;

		// Location of the central filter tap (i.e., "this" pixel's location)
		// Account for the scaling down to 25% of original dimensions during blur
		float2 A = float2(i.texCoords.xy);// *(direction * 3 + float2(1, 1)));

			float packedA = tex2D(_MainTex, A).a;
		float r_A = (packedA * 2.0 - 1.0) * maxCoCRadiusPixels;

		// Map r_A << 0 to 0, r_A >> 0 to 1
		float nearFieldness_A = saturate(r_A * 4.0);

		//[loop]
		for (int delta = -maxCoCRadiusPixels; delta <= maxCoCRadiusPixels; ++delta)
		{
			// Tap location near A
			float2   B = A + ((direction * _MainTex_TexelSize.xy) * delta);
				// Packed values
				float4 blurInput = tex2Dlod(_MainTex, float4(clamp(B, float2(0, 0), float2(1, 1) - _MainTex_TexelSize.xy), 0, 0));

				// Signed kernel radius at this tap, in pixels
				float r_B = (blurInput.a * 2.0 - 1.0) * float(maxCoCRadiusPixels);

			/////////////////////////////////////////////////////////////////////////////////////////////
			// Compute blurry buffer

			float weight = 0.0;

			float wNormal =
				// Only consider mid- or background pixels (allows inpainting of the near-field)
				float(!inNearField(r_B)) *

				// Only blur B over A if B is closer to the viewer (allow 0.5 pixels of slop, and smooth the transition)
				// This term avoids "glowy" background objects but leads to aliasing under 4x downsampling
				//saturate(abs(r_A) - abs(r_B) + 1.5) *

				// Stretch the kernel extent to the radius at pixel B.
				kernel[clamp(int(float(abs(delta) * (KERNEL_TAPS - 1)) / (0.001 + abs(r_B * 0.8))), 0, KERNEL_TAPS)];

			weight = lerp(wNormal, 1.0, nearFieldness_A);

			// far + mid-field output 
			blurWeightSum += weight;
			output.blurResult.rgb += blurInput.rgb * weight;

			///////////////////////////////////////////////////////////////////////////////////////////////
			// Compute near-field super blurry buffer

			float4 nearInput;
#if HORIZONTAL
			// On the first pass, extract coverage from the near field radius
			// Note that the near field gets a box-blur instead of a kernel 
			// blur; we found that the quality improvement was not worth the 
			// performance impact of performing the kernel computation here as well.

			// Curve the contribution based on the radius.  We tuned this particular
			// curve to blow out the near field while still providing a reasonable
			// transition into the focus field.
			nearInput.a = float(abs(delta) <= r_B) * saturate(r_B * invNearBlurRadiusPixels * 4.0);
			// Optionally increase edge fade contrast in the near field
			nearInput.a *= nearInput.a; nearInput.a *= nearInput.a;

			// Compute premultiplied-alpha color
			nearInput.rgb = blurInput.rgb * nearInput.a;
#endif
#if VERTICAL
			// On the second pass, use the already-available alpha values
			nearInput = tex2Dlod(nearSourceBuffer, float4(clamp(B, float2(0, 0), float2(1, 1) - _MainTex_TexelSize.xy), 0, 0));
#endif

			// We subsitute the following efficient expression for the more complex:
			//weight = kernel[clamp(int(float(abs(delta) * (KERNEL_TAPS - 1)) * invNearBlurRadiusPixels), 0, KERNEL_TAPS)];
			weight = float(abs(delta) < nearBlurRadiusPixels);
			output.nearResult += nearInput * weight;
			nearWeightSum += weight;
		}

#if HORIZONTAL
		// Retain the packed radius on the first pass.  On the second pass it is not needed.
		output.blurResult.a = packedA;
#endif
#if VERTICAL
		output.blurResult.a = 1.0;
#endif

		// Normalize the blur
		output.blurResult.rgb /= blurWeightSum;
		output.nearResult /= max(nearWeightSum, 0.00001);
		return output;
	}

		float4 fragComposite(const PixelInput i) : COLOR
	{
		const float2 cocReadScaleBias = float2(2.0, -1.0);
		//const float coverageBoost = 0.3;
		float2 A = i.texCoords.xy;

			float4 pack = tex2D(_MainTex, A);
			float3 sharp = pack.rgb;
			float3 blurred = tex2D(blurBuffer, A).rgb;
			float4 near = tex2D(nearBuffer, A);//

			// Signed, normalized radius of the circle of confusion.
			// |normRadius| == 1.0 corresponds to camera->maxCircleOfConfusionRadiusPixels()
			float normRadius = pack.a *cocReadScaleBias.x + cocReadScaleBias.y;

		// Fix the far field scaling factor so that it remains independent of the 
		// near field settings
		normRadius *= (normRadius < 0.0) ? farRadiusRescale : 1.0;

		// Boost the blur factor
		normRadius = clamp(normRadius * 2.0, -1.0, 1.0);

		if (coverageBoost != 1.0) {
			float a = saturate(coverageBoost * near.a);
			near.rgb = near.rgb * (a / max(near.a, 0.001f));
			near.a = a;
		}

		// Decrease sharp image's contribution rapidly in the near field
		// (which has positive normRadius)
		if (normRadius > 0.1) {
			normRadius = min(normRadius * 1.5, 1.0);
		}

		// Two lerps, the second of which has a premultiplied alpha
		return float4(lerp(sharp, blurred, abs(normRadius)) * (1.0 - near.a) + near.rgb, 0);
	}


		ENDCG

		SubShader
	{
		// (0) Circle of Confusion Calculation
		Pass
		{
			ZTest Off Cull Off ZWrite Off Blend Off
			Fog{ Mode off }
			CGPROGRAM
#pragma vertex vert
#pragma fragment fragCoC
#pragma fragmentoption ARB_precision_hint_fastest
#pragma exclude_renderers flash gles
#pragma multi_compile PHYSICAL ARTIST NONE
			ENDCG
		}

		// (1) Blur DoF
		Pass
			{
				ZTest Off Cull Off ZWrite Off Blend Off
				Fog{ Mode off }
				CGPROGRAM
#pragma vertex vert
#pragma fragment fragBlur
#pragma fragmentoption ARB_precision_hint_fastest 		
#pragma multi_compile HORIZONTAL VERTICAL
				ENDCG
			}

			// (2) Composite
			Pass
				{
					ZTest Always Cull Off ZWrite Off Blend Off
					Fog{ Mode off }
					CGPROGRAM
#pragma vertex vert
#pragma fragment fragComposite
#pragma fragmentoption ARB_precision_hint_fastest 		
					ENDCG
				}
	}
	FallBack off
}