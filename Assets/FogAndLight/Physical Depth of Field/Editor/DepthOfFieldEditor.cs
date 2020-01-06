using UnityEngine;
using UnityEditor;

namespace gbFactory
{
	[CustomEditor(typeof (DepthOfField))]
	public class DepthOfFieldEditor : Editor
	{
		private SerializedObject t;

		private SerializedProperty model;
		private SerializedProperty m_downsample;

		private SerializedProperty m_focusPlaneZ;
		private SerializedProperty m_lensRadius;

		private SerializedProperty m_farBlurryPlaneZ;
		private SerializedProperty m_farSharpPlaneZ;
		private SerializedProperty m_nearSharpPlaneZ;
		private SerializedProperty m_nearBlurryPlaneZ;
		private SerializedProperty m_boostCoverage;
		private SerializedProperty m_maxCoCRadiusPixels;
		[SerializeField] private bool pressedPhysical = true;
		[SerializeField] private bool pressedArtistic = false;


		private void OnEnable()
		{
			t = new SerializedObject(target);
			((DepthOfField) target).shader = Shader.Find("Hidden/DepthOfField");
			model = t.FindProperty("model");
			m_downsample = t.FindProperty("m_downsample");

			m_focusPlaneZ = t.FindProperty("m_focusPlaneZ");
			m_lensRadius = t.FindProperty("m_lensRadius");

			m_farBlurryPlaneZ = t.FindProperty("m_farBlurryPlaneZ");
			m_farSharpPlaneZ = t.FindProperty("m_farSharpPlaneZ");
			m_nearSharpPlaneZ = t.FindProperty("m_nearSharpPlaneZ");
			m_nearBlurryPlaneZ = t.FindProperty("m_nearBlurryPlaneZ");
			m_boostCoverage = t.FindProperty("m_boostCoverage");
			m_maxCoCRadiusPixels = t.FindProperty("m_maxCoCRadiusPixels");
			if (model.enumValueIndex == 0)
			{
				pressedArtistic = false;
				pressedPhysical = true;
			}
			else
			{
				pressedPhysical = false;
				pressedArtistic = true;
			}
		}

		public override void OnInspectorGUI()
		{
			t.Update();
			//EditorGUILayout.ObjectField(((DepthOfField)target).shader,typeof(Shader),true);
			EditorGUILayout.LabelField("Depth of Field Model");
			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Toggle(pressedPhysical, "Physical", "Button"))
			{
				model.enumValueIndex = 0;
				pressedArtistic = false;
				pressedPhysical = true;
			}
			if (GUILayout.Toggle(pressedArtistic, "Artistic", "Button"))
			{
				model.enumValueIndex = 1;
				pressedPhysical = false;
				pressedArtistic = true;
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.PrefixLabel("Downsample");
			m_downsample.intValue = EditorGUILayout.IntSlider(m_downsample.intValue, 2, 8);
			m_boostCoverage.floatValue = EditorGUILayout.FloatField("Coverage Boost", m_boostCoverage.floatValue);
			m_maxCoCRadiusPixels.intValue = EditorGUILayout.IntSlider("CoC Radius", m_maxCoCRadiusPixels.intValue,1,16);
			EditorGUILayout.Space();
			if (pressedPhysical)
			{
				EditorGUILayout.LabelField("Physical DoF Settings");
				EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Focus Plane Z");
				m_focusPlaneZ.floatValue =
					-EditorGUILayout.Slider(Mathf.Abs(m_focusPlaneZ.floatValue), Camera.main.nearClipPlane, Camera.main.farClipPlane);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Lens Radius");
				m_lensRadius.floatValue = EditorGUILayout.Slider(m_lensRadius.floatValue, 0.00001f, 2.0f);
				EditorGUILayout.EndHorizontal();
			}
			if (pressedArtistic)
			{
				EditorGUILayout.LabelField("Artistic DoF Settings");
				EditorGUILayout.Space();
				EditorGUILayout.PrefixLabel("Sharp Plane Range");
				float tempFar = m_farSharpPlaneZ.floatValue;
				float tempNear = m_nearSharpPlaneZ.floatValue;
				EditorGUILayout.MinMaxSlider(ref tempFar, ref tempNear, -Camera.main.farClipPlane,
					-Camera.main.nearClipPlane);
				m_farSharpPlaneZ.floatValue = tempFar;
				m_nearSharpPlaneZ.floatValue = tempNear;

				EditorGUILayout.PrefixLabel("Blurry Plane Range");
				float tempBFar = m_farBlurryPlaneZ.floatValue;
				float tempBNear = m_nearBlurryPlaneZ.floatValue;
				EditorGUILayout.MinMaxSlider(ref tempBFar, ref tempBNear, -Camera.main.farClipPlane,
					-Camera.main.nearClipPlane);
				m_farBlurryPlaneZ.floatValue = tempBFar;
				m_nearBlurryPlaneZ.floatValue = tempBNear;
			}

			t.ApplyModifiedProperties();
		}
	}
}