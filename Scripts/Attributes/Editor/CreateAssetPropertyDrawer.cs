namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(CreateAssetAttribute))]
	public class CreateAssetPropertyDrawer : PropertyDrawer {


		#region Unity Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			label = EditorGUI.BeginProperty(position, label, property);

			// Pending creation command
			if (m_CreateAssetCommand != null) {
				if (m_CreateAssetCommand.Asset != null) {
					// Assign the created asset to the property
					property.objectReferenceValue = m_CreateAssetCommand.Asset;
					property.serializedObject.ApplyModifiedProperties();
				}
				m_CreateAssetCommand = null;
			}

			// Check if the property is null
			if (property.objectReferenceValue == null) {

				// Draw a smaller property field 
				var width = 60;
				var fieldRect = position;
				fieldRect.xMax -= width + 2;
				EditorGUI.PropertyField(fieldRect, property, label);

				// Create a button next to the property field
				var buttonRect = new Rect(position.xMax - width, position.y, width, position.height);
				if (GUI.Button(buttonRect, "Create")) {
					
					// Get the type of the ScriptableObject
					Type objectType = fieldInfo.FieldType.GetElementType() ?? fieldInfo.FieldType;
					
					// Store data for the creation of the asset on next frame to prevent editor errors
					m_CreateAssetCommand = new CreateAssetCommand(objectType);
					EditorApplication.update += EditorApplication_update;

				}

			} else {
				EditorGUI.PropertyField(position, property, label);
			}

			EditorGUI.EndProperty();

		}

		#endregion


		#region Event Handlers

		private void EditorApplication_update() {
			EditorApplication.update -= EditorApplication_update;
			m_CreateAssetCommand.Excecute();
		}

		#endregion


		#region Private Fields

		private CreateAssetCommand m_CreateAssetCommand;

		#endregion


		#region Support Classes

		private class CreateAssetCommand {

			public ScriptableObject Asset => m_Asset;

			public CreateAssetCommand(Type type) {
				m_Type = type;
			}

			public void Excecute() {

				// Create a new ScriptableObject instance
				m_Asset = ScriptableObject.CreateInstance(m_Type);

				// Save the asset to the root of the Assets folder
				string path = EditorUtility.SaveFilePanelInProject(
					"Create New ScriptableObject",
					"New" + m_Type.Name + ".asset",
					"asset",
					"Create a new ScriptableObject",
					"Assets"
				);

				if (!string.IsNullOrEmpty(path)) {
					AssetDatabase.CreateAsset(m_Asset, path);
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
				}

			}
			
			private Type m_Type;

			private ScriptableObject m_Asset;

		}

		#endregion


	}

}