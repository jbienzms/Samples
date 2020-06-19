﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.LightingTools
{
	[CustomEditor(typeof(LightCapture))]
	public class LightCaptureEditor : UnityEditor.Editor
	{
		// SerializedProperties representing LightCapture fields.
		private SerializedProperty mapResolution;
		private SerializedProperty singleStampOnly;
		private SerializedProperty stampFovMultiplier;
		private SerializedProperty stampExpireDistance;
		private SerializedProperty useDirectionalLight;
		private SerializedProperty maxLightColorSaturation;
		private SerializedProperty lightAngleAdjustPerSecond;
		private SerializedProperty cameraOrientation;
		private SerializedProperty probe;
		private SerializedProperty directionalLight;

		private void OnEnable()
		{
			mapResolution       = serializedObject.FindProperty("mapResolution");
			singleStampOnly     = serializedObject.FindProperty("singleStampOnly");
			stampFovMultiplier  = serializedObject.FindProperty("stampFovMultiplier");
			stampExpireDistance = serializedObject.FindProperty("stampExpireDistance");
			useDirectionalLight = serializedObject.FindProperty("useDirectionalLight");
			maxLightColorSaturation = serializedObject.FindProperty("maxLightColorSaturation");
			lightAngleAdjustPerSecond = serializedObject.FindProperty("lightAngleAdjustPerSecond");
			cameraOrientation   = serializedObject.FindProperty("cameraOrientation");
			probe               = serializedObject.FindProperty("probe");
			directionalLight    = serializedObject.FindProperty("directionalLight");
		}
		public override void OnInspectorGUI()
		{
			EditorGUILayout.LabelField("Quality Presets", EditorStyles.boldLabel);
			int preset = EditorGUILayout.Popup("Apply preset:", 0, new string[] { "...", "Super Low", "Fast", "High", "Very High"});
			if (preset == 1)  // Low
			{
				mapResolution      .intValue = 128;
				singleStampOnly    .boolValue = true;
				stampFovMultiplier .floatValue = 1.5f;
				stampExpireDistance.floatValue = 0;
			}
			else if (preset == 2) // Medium
			{ 
				mapResolution      .intValue = 128;
				singleStampOnly    .boolValue = false;
				stampFovMultiplier .floatValue = 1.5f;
				stampExpireDistance.floatValue = 0;
			}
			else if (preset == 3) // Normal
			{ 
				mapResolution      .intValue = 128;
				singleStampOnly    .boolValue = false;
				stampFovMultiplier .floatValue = 1f;
				stampExpireDistance.floatValue = 0;
			}
			else if (preset == 4) // High
			{ 
				mapResolution      .intValue = 256;
				singleStampOnly    .boolValue = false;
				stampFovMultiplier .floatValue = 1f;
				stampExpireDistance.floatValue = 8;
			}
		
			EditorGUILayout.PropertyField(mapResolution);
			EditorGUILayout.PropertyField(singleStampOnly);
			EditorGUILayout.PropertyField(stampFovMultiplier);
			EditorGUILayout.PropertyField(stampExpireDistance);
			EditorGUILayout.PropertyField(useDirectionalLight);
			EditorGUILayout.PropertyField(maxLightColorSaturation);
			EditorGUILayout.PropertyField(lightAngleAdjustPerSecond);
			EditorGUILayout.PropertyField(cameraOrientation);
			EditorGUILayout.PropertyField(probe);
			EditorGUILayout.PropertyField(directionalLight);

			serializedObject.ApplyModifiedProperties();
		}
	}
}