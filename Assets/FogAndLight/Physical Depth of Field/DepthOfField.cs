using System;
using UnityEngine;

namespace gbFactory
{
	[ExecuteInEditMode]
	[RequireComponent(typeof (Camera))]
	[AddComponentMenu("Image Effects/DepthOfField")]
	public class DepthOfField : MonoBehaviour
	{
		public enum DoFModel
		{
			Physical = 0,
			Artistic = 1,
		}

		private bool isSupported;
		private float m_farRadiusFraction = 0.005f;
		private float m_farRadiusRescale = 1f;
		public int m_maxCoCRadiusPixels = 6;
		private float m_nearBlurRadiusPixels = 1;
		private float m_nearRadiusFraction = 0.015f;
		private float m_scale = 0.1f;
		public Material material;
		[SerializeField] public Shader shader;

		public float m_farBlurryPlaneZ = -100.0f;
		public float m_farSharpPlaneZ = -40.0f;
		public float m_nearSharpPlaneZ = -1.0f;
		public float m_nearBlurryPlaneZ = -0.25f;
		public float m_focusPlaneZ = -10;
		public float m_lensRadius = 0.01f;
		public float m_boostCoverage = 0.1f;
		public DoFModel model = DoFModel.Physical;
		[Range(2, 8)] public int m_downsample = 4;


		public float focusPlaneZ
		{
			get { return m_focusPlaneZ; }
			set
			{
				if (value < 0)
					m_focusPlaneZ = value;
			}
		}

		private void OnEnable()
		{
			GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
		}

		private void Start()
		{
			isSupported = true;

			if (!material)
			{
				material = new Material(shader);
			}

			if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures ||
			    !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
			{
				isSupported = false;
			}
		}

		private void OnDisable()
		{
			if (material)
				DestroyImmediate(material);
		}

		private static Material CreateMaterial(Shader shader)
		{
			if (!shader)
				return null;
			var m = new Material(shader);
			m.hideFlags = HideFlags.DontSave;
			return m;
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!isSupported)
			{
				Graphics.Blit(source, destination);
				return;
			}

			if (!material)
			{
				material = new Material(shader);
			}

			material.hideFlags = HideFlags.HideAndDontSave;
			//m_maxCoCRadiusPixels = (int) Mathf.Ceil(maxCircleOfConfusionRadiusPixels());
			m_nearBlurRadiusPixels =
				Mathf.Ceil(model == DoFModel.Artistic ? m_nearRadiusFraction*Screen.width : maxPhysicalBlurRadius());

			if (model == DoFModel.Physical)
			{
				m_scale = (imagePlanePixelsPerMeter()*m_lensRadius)/(m_focusPlaneZ*m_maxCoCRadiusPixels);
				//Debug.Log("m_scale -" +m_scale);
				m_farRadiusRescale = 1.0f;

				material.SetFloat("focusPlaneZ", m_focusPlaneZ);
				material.SetFloat("scale", m_scale);
				material.EnableKeyword("PHYSICAL");
				material.DisableKeyword("ARTIST");
				material.DisableKeyword("NONE");
			}
			else if (model == DoFModel.Artistic)
			{
				// This is a positive number
				float nearScale = m_nearRadiusFraction/(m_nearBlurryPlaneZ - m_nearSharpPlaneZ);

				// This is a positive number
				float farScale = m_farRadiusFraction/(m_farSharpPlaneZ - m_farBlurryPlaneZ);

				m_farRadiusRescale =
					Mathf.Max(m_farRadiusFraction, m_nearRadiusFraction)/
					Mathf.Max(m_farRadiusFraction, 0.0001f);

				material.SetFloat("nearBlurryPlaneZ", m_nearBlurryPlaneZ);
				material.SetFloat("nearSharpPlaneZ", m_nearSharpPlaneZ);
				material.SetFloat("farSharpPlaneZ", m_farSharpPlaneZ);
				material.SetFloat("farBlurryPlaneZ", m_farBlurryPlaneZ);
				material.SetFloat("nearScale", nearScale*Screen.width/m_maxCoCRadiusPixels);
				material.SetFloat("farScale", farScale*Screen.width/m_maxCoCRadiusPixels);
				material.DisableKeyword("PHYSICAL");
				material.EnableKeyword("ARTIST");
				material.DisableKeyword("NONE");
			}

			material.SetFloat("farRadiusRescale", m_farRadiusRescale);
			material.SetVector("writeScaleBias", new Vector2(0.5f, 0.5f));
			material.SetFloat("coverageBoost", m_boostCoverage);
			RenderTexture m_packedBuffer = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, source.format);
			RenderTexture m_tempBlurBuffer = RenderTexture.GetTemporary(Screen.width/m_downsample, Screen.height, 0,
				source.format);
			RenderTexture m_tempNearBuffer = RenderTexture.GetTemporary(Screen.width/m_downsample, Screen.height, 0,
				source.format);
			RenderTexture m_nearBuffer = RenderTexture.GetTemporary(Screen.width/m_downsample, Screen.height/m_downsample, 0,
				source.format);
			RenderTexture m_blurBuffer = RenderTexture.GetTemporary(Screen.width/m_downsample, Screen.height/m_downsample, 0,
				source.format);

			Graphics.Blit(source, m_packedBuffer, material, 0);

			material.EnableKeyword("HORIZONTAL");
			material.DisableKeyword("VERTICAL");
			material.SetVector("direction",
				material.IsKeywordEnabled("HORIZONTAL") ? new Vector2(1.0f, 0.0f) : new Vector2(0.0f, 1.0f));

			material.SetFloat("nearBlurRadiusPixels", m_nearBlurRadiusPixels);
			material.SetFloat("invNearBlurRadiusPixels", 1.0f/Math.Max((m_nearBlurRadiusPixels), 0.0001f));
			material.SetVector("packedBufferInvSize", new Vector2(1.0f/m_packedBuffer.width, 1.0f/m_packedBuffer.height));

			material.SetInt("maxCoCRadiusPixels", m_maxCoCRadiusPixels);
			material.SetTexture("_MainTex", m_packedBuffer);

			RenderTexture[] tempR = {m_tempNearBuffer, m_tempBlurBuffer};
			MultiTargetBlit(tempR, material, 1);

			material.DisableKeyword("HORIZONTAL");
			material.EnableKeyword("VERTICAL");
			material.SetVector("direction",
				material.IsKeywordEnabled("HORIZONTAL") ? new Vector2(1.0f, 0.0f) : new Vector2(0.0f, 1.0f));

			material.SetTexture("_MainTex", m_tempBlurBuffer);
			material.SetTexture("nearSourceBuffer", m_tempNearBuffer);

			RenderTexture[] tempR2 = {m_nearBuffer, m_blurBuffer};
			MultiTargetBlit(tempR2, material, 1);

			material.SetTexture("blurBuffer", m_blurBuffer);
			material.SetTexture("nearBuffer", m_nearBuffer);
			Graphics.Blit(m_packedBuffer, destination, material, 2);

			RenderTexture.ReleaseTemporary(m_packedBuffer);
			RenderTexture.ReleaseTemporary(m_tempBlurBuffer);
			RenderTexture.ReleaseTemporary(m_blurBuffer);
			RenderTexture.ReleaseTemporary(m_tempNearBuffer);
			RenderTexture.ReleaseTemporary(m_nearBuffer);
		}

		public static void MultiTargetBlit(RenderTexture[] multiRT, Material mat, int pass)
		{
			var rb = new RenderBuffer[multiRT.Length];

			for (int i = 0; i < multiRT.Length; i++)
				rb[i] = multiRT[i].colorBuffer;

			Graphics.SetRenderTarget(rb, multiRT[0].depthBuffer);

			GL.Clear(true, true, Color.clear);

			GL.PushMatrix();
			GL.LoadOrtho();

			mat.SetPass(pass);

			GL.Begin(GL.TRIANGLES);
			GL.TexCoord2(0, 1);
			GL.Vertex3(0, 1, 0.1f);
			GL.TexCoord2(0, -1);
			GL.Vertex3(0, -1, 0.1f);
			GL.TexCoord2(2, 1);
			GL.Vertex3(2, 1, 0.1f);
			GL.End();

			GL.PopMatrix();
		}

		private void OnValidate()
		{
			setNearBlurryPlaneZ(m_nearBlurryPlaneZ);
			setNearSharpPlaneZ(m_nearSharpPlaneZ);
			setFarSharpPlaneZ(m_farSharpPlaneZ);
			setFarBlurryPlaneZ(m_farBlurryPlaneZ);
		}

		private void setNearBlurryPlaneZ(float z)
		{
			m_nearBlurryPlaneZ = z;
			m_nearSharpPlaneZ = Mathf.Min(m_nearBlurryPlaneZ - 0.001f, m_nearSharpPlaneZ);
			m_farSharpPlaneZ = Mathf.Min(m_nearSharpPlaneZ - 0.001f, m_farSharpPlaneZ);
			m_farBlurryPlaneZ = Mathf.Min(m_farSharpPlaneZ - 0.001f, m_farBlurryPlaneZ);
		}

		private void setNearSharpPlaneZ(float z)
		{
			m_nearSharpPlaneZ = z;
			m_nearBlurryPlaneZ = Mathf.Max(m_nearBlurryPlaneZ, 0.001f + m_nearSharpPlaneZ);
			m_farSharpPlaneZ = Mathf.Min(m_nearSharpPlaneZ - 0.001f, m_farSharpPlaneZ);
			m_farBlurryPlaneZ = Mathf.Min(m_farSharpPlaneZ - 0.001f, m_farBlurryPlaneZ);
		}

		private void setFarSharpPlaneZ(float z)
		{
			m_farSharpPlaneZ = z;
			m_farBlurryPlaneZ = Mathf.Min(m_farSharpPlaneZ - 0.001f, m_farBlurryPlaneZ);
			m_nearSharpPlaneZ = Mathf.Max(m_farSharpPlaneZ + 0.001f, m_nearSharpPlaneZ);
			m_nearBlurryPlaneZ = Mathf.Max(m_nearBlurryPlaneZ, 0.001f + m_nearSharpPlaneZ);
		}

		private void setFarBlurryPlaneZ(float z)
		{
			m_farBlurryPlaneZ = z;
			m_farSharpPlaneZ = Mathf.Max(m_farSharpPlaneZ, 0.001f + m_farBlurryPlaneZ);
			m_nearSharpPlaneZ = Mathf.Max(m_farSharpPlaneZ + 0.001f, m_nearSharpPlaneZ);
			m_nearBlurryPlaneZ = Mathf.Max(m_nearBlurryPlaneZ, 0.001f + m_nearSharpPlaneZ);
		}

		private float circleOfConfusionRadiusPixels(float z, float imagePlanePixelsPerMeter, float screenPixelSize)
		{
			switch (model)
			{
				case DoFModel.Physical:
				{
					// Circle of confusion at z, in meters
					float rzmeters = (z - m_focusPlaneZ)*m_lensRadius/m_focusPlaneZ;

					// Project
					float rimeters = rzmeters/-z;

					// Convert to pixels
					float ripixels = rimeters*imagePlanePixelsPerMeter;

					return ripixels;
				}

				case DoFModel.Artistic:
				{
					// Radius relative to screen dimension
					float r = 0;

					if (z >= m_nearSharpPlaneZ)
					{
						// Near field

						// Blurriness fraction
						float a = (Mathf.Min(z, m_nearBlurryPlaneZ) - m_nearSharpPlaneZ)/(m_nearSharpPlaneZ - m_nearBlurryPlaneZ);

						r = Mathf.Lerp(0.0f, m_nearRadiusFraction, a);
					}
					else if (z <= m_farSharpPlaneZ)
					{
						return 0.0f;
					}
					else
					{
						// Far field
						float a = (m_farSharpPlaneZ - Math.Max(z, m_farBlurryPlaneZ))/(m_farBlurryPlaneZ - m_farSharpPlaneZ);
						r = Mathf.Lerp(0.0f, m_farRadiusFraction, a);
					}

					if (r >= 0 && r <= 1.0f) Debug.Log("Illegal circle of confusion radius");
					return r*screenPixelSize;
				}

				default:
					return 0.0f;
			}
		}

		private float maxCircleOfConfusionRadiusPixels()
		{
			switch (model)
			{
				case DoFModel.Artistic:

					float dimension = Screen.width;
					return Mathf.Max(m_nearRadiusFraction, m_farRadiusFraction*dimension);

				case DoFModel.Physical:

					return
						Mathf.Min(
							Mathf.Max(circleOfConfusionRadiusPixels(Camera.main.nearClipPlane, imagePlanePixelsPerMeter(), Screen.width),
								circleOfConfusionRadiusPixels(Camera.main.nearClipPlane, imagePlanePixelsPerMeter(), Screen.width)),
							Screen.width*0.02f);

				default:
					return 0.0f;
			}
		}

		private float imagePlanePixelsPerMeter()
		{
			float scale = Mathf.Abs(-2.0f*Mathf.Tan((Mathf.Deg2Rad*Camera.main.fieldOfView)*0.5f));
			return Screen.height/scale;
		}

		/** Limit the maximum radius allowed for physical blur to 1% of the viewport or 12 pixels */
		private float maxPhysicalBlurRadius()
		{
			return Mathf.Max(Screen.width/100.0f, 12.0f);
		}
	}
}